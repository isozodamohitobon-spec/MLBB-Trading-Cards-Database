
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using OrisProject.Models;
using OrisProject.Data.Repositories;
using OrisProject.Data.Repositories.Interfaces;

namespace OrisProject.Data;

public class Database
{
    private readonly string _connectionString;
    private readonly ICardRepository _cardRepository;

    public Database()
    {
        _connectionString =
            Environment.GetEnvironmentVariable("ORIS_DB_CONNECTION")
            ?? throw new InvalidOperationException(
                "Не задана строка подключения ORIS_DB_CONNECTION");

        _cardRepository = new CardRepository(_connectionString);
    }

    // Получение всех карточек
    public List<Card> GetAllCards()
    {
        return _cardRepository.GetAllCards();
    }

    // Получение карточки по коду
    public Card? GetCardByCode(string code)
    {
        return _cardRepository.GetCardByCode(code);
    }

    // Добавление карточки
    public Task AddCardAsync(
        string code,
        string name,
        decimal price,
        string imageUrl,
        int heroId,
        int rarityId,
        string description)
    {
        return _cardRepository.AddCardAsync(
            code, name, price, imageUrl, heroId, rarityId, description);
    }

    // Удаление карточки
    public Task DeleteCardAsync(string code)
    {
        return _cardRepository.DeleteCardAsync(code);
    }

    // Получение списка героев
    public List<KeyValuePair<int, string>> GetHeroes()
    {
        var list = new List<KeyValuePair<int, string>>();

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            "SELECT id, name FROM heroes ORDER BY id", conn);

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            list.Add(
                new KeyValuePair<int, string>(
                    reader.GetInt32(0),
                    reader.GetString(1)));
        }

        return list;
    }

    // Получение списка редкостей
    public List<KeyValuePair<int, string>> GetRarities()
    {
        var list = new List<KeyValuePair<int, string>>();

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            "SELECT id, name FROM rarities ORDER BY id", conn);

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            list.Add(
                new KeyValuePair<int, string>(
                    reader.GetInt32(0),
                    reader.GetString(1)));
        }

        return list;
    }
}
