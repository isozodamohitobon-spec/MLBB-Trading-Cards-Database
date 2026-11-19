
using System.ComponentModel.DataAnnotations.Schema;

namespace OrisProject.Models;

public class Card
{
    [Column("id")]
    public int Id { get; set; }

    [Column("card_code")]
    public string CardCode { get; set; } = string.Empty;

    [Column("card_name")]
    public string CardName { get; set; } = string.Empty;

    [Column("price")]
    public decimal Price { get; set; }

    [Column("times_viewed")]
    public int TimesViewed { get; set; }

    [Column("image_url")]
    public string ImageUrl { get; set; } = string.Empty;

    [Column("rarity_id")]
    public int? RarityId { get; set; }

    [Column("hero_id")]
    public int? HeroId { get; set; }

    [NotMapped]
    public string? HeroName { get; set; }

    [NotMapped]
    public string? RarityName { get; set; }

    [Column("description")]
    public string? Description { get; set; }
}
