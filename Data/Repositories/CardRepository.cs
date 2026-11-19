
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using OrisProject.Models;
using OrisProject.ORM;
using OrisProject.Data.Repositories.Interfaces;

namespace OrisProject.Data.Repositories;

public class CardRepository : ICardRepository
{
    private readonly string _connectionString;
    private readonly OrmContext _orm;

    public CardRepository(string connectionString)
    {
        _connectionString = connectionString;
        _orm = new OrmContext(connectionString);
    }

    public List<Card> GetAllCards()
    {
        var cards = new List<Card>();

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        string sql = @"
            SELECT c.id, c.card_code, c.card_name, c.price,
                   c.times_viewed, c.image_url,
                   h.name AS hero_name,
                   r.name AS rarity_name
            FROM cards c
            LEFT JOIN heroes h ON c.hero_id = h.id
            LEFT JOIN rarities r ON c.rarity_id = r.id
            ORDER BY c.id;";

        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            cards.Add(new Card
            {
                Id = reader.GetInt32(0),
                CardCode = reader.GetString(1),
                CardName = reader.GetString(2),
                Price = reader.GetDecimal(3),
                TimesViewed = reader.GetInt32(4),
                ImageUrl = reader.GetString(5),
                HeroName = reader.IsDBNull(6) ? null : reader.GetString(6),
                RarityName = reader.IsDBNull(7) ? null : reader.GetString(7)
            });
        }

        return cards;
    }

    public Card? GetCardByCode(string code)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        string sql = @"
            SELECT c.id, c.card_code, c.card_name, c.price,
                   c.times_viewed, c.image_url,
                   h.name AS hero_name,
                   r.name AS rarity_name
            FROM cards c
            LEFT JOIN heroes h ON c.hero_id = h.id
            LEFT JOIN rarities r ON c.rarity_id = r.id
            WHERE c.card_code = @code;";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("code", code);

        using var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return new Card
            {
                Id = reader.GetInt32(0),
                CardCode = reader.GetString(1),
                CardName = reader.GetString(2),
                Price = reader.GetDecimal(3),
                TimesViewed = reader.GetInt32(4),
                ImageUrl = reader.GetString(5),
                HeroName = reader.IsDBNull(6) ? null : reader.GetString(6),
                RarityName = reader.IsDBNull(7) ? null : reader.GetString(7)
            };
        }

        return null;
    }

    public async Task AddCardAsync(
        string code,
        string name,
        decimal price,
        string imageUrl,
        int heroId,
        int rarityId,
        string description)
    {
        Card card = new Card
        {
            CardCode = code,
            CardName = name,
            Price = price,
            TimesViewed = 0,
            ImageUrl = imageUrl,
            HeroId = heroId,
            RarityId = rarityId,
            Description = description
        };

        await _orm.InsertAsync("cards", card);
    }

    public async Task DeleteCardAsync(string code)
    {
        Card? card = GetCardByCode(code);

        if (card == null)
        {
            return;
        }

        await _orm.DeleteAsync("cards", card.Id);
    }
}
