
using System;
using System.Collections.Generic;
using System.IO;
using OrisProject.Data;
using OrisProject.Models;

namespace OrisProject.Controllers;

public class HomeController
{
    private readonly Func<List<Card>> _getAllCards;
    private readonly string _indexTemplatePath;

    // Обычный конструктор для работы сайта
    public HomeController(Database database)
    {
        _getAllCards = database.GetAllCards;

        _indexTemplatePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Views",
            "index.html");
    }

    // Дополнительный конструктор для тестов
    public HomeController(
        Func<List<Card>> getAllCards,
        string indexTemplatePath)
    {
        _getAllCards = getAllCards;
        _indexTemplatePath = indexTemplatePath;
    }

    public string GetIndexPage()
    {
        var cards = _getAllCards();

        string html = File.ReadAllText(_indexTemplatePath);

        html = TemplateEngine.RenderList(
            html,
            "Cards",
            cards,
            (card) =>
            {
                string cardTemplate = @"
                    <div class='card'
                         onclick='openModal(this.getAttribute(""data-code""))'
                         data-code='{{CardCode}}'
                         data-views='{{TimesViewed}}'>

                        <div class='card-top-badge'
                             data-rarity='{{RarityName}}'>
                            {{RarityName}}
                        </div>

                        <img
                            src='{{ImageUrl}}'
                            alt='{{CardName}}'
                            onerror='this.src=""https://via.placeholder.com/200x280?text=No+Image""'>

                        <div class='card-overlay'>
                            <h3 class='card-title'>{{CardName}}</h3>

                            <div class='card-hero'>
                                {{HeroName}} • {{CardCode}}
                            </div>

                            <div class='card-bottom'>
                                <div class='card-price'>${{Price}}</div>
                                <div class='card-views'>
                                    👁 {{TimesViewed}}
                                </div>
                            </div>
                        </div>
                    </div>";

                var cardData = new Dictionary<string, string>
                {
                    { "ImageUrl", card.ImageUrl },
                    { "CardName", card.CardName },
                    { "HeroName", card.HeroName ?? "" },
                    { "RarityName", card.RarityName ?? "" },
                    { "Price", card.Price.ToString("0.00") },
                    { "CardCode", card.CardCode },
                    { "TimesViewed", card.TimesViewed.ToString() }
                };

                return TemplateEngine.Render(cardTemplate, cardData);
            });

        var data = new Dictionary<string, string>
        {
            { "CardsCount", cards.Count.ToString() }
        };

        return TemplateEngine.Render(html, data);
    }
}
