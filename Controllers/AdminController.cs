
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Globalization;
using System.Web;
using OrisProject.Data;

namespace OrisProject.Controllers;

public class AdminController
{
    private readonly Func<List<KeyValuePair<int, string>>> _getHeroes;
    private readonly Func<List<KeyValuePair<int, string>>> _getRarities;

    private readonly Func<string, string, decimal, string, int, int, string, Task>
        _addCard;

    private readonly string _adminTemplatePath;

    public AdminController(Database database)
    {
        _getHeroes = database.GetHeroes;
        _getRarities = database.GetRarities;
        _addCard = database.AddCardAsync;

        _adminTemplatePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Views",
            "admin.html");
    }

    // Конструктор для тестирования
    public AdminController(
        Func<List<KeyValuePair<int, string>>> getHeroes,
        Func<List<KeyValuePair<int, string>>> getRarities,
        Func<string, string, decimal, string, int, int, string, Task> addCard,
        string adminTemplatePath)
    {
        _getHeroes = getHeroes;
        _getRarities = getRarities;
        _addCard = addCard;
        _adminTemplatePath = adminTemplatePath;
    }

    // Получение страницы админ-панели
    public string GetAdminPage()
    {
        string html = File.ReadAllText(_adminTemplatePath);

        var heroes = _getHeroes();
        var rarities = _getRarities();

        string heroesOptions = "";

        foreach (var hero in heroes)
        {
            heroesOptions +=
                $"<option value='{hero.Key}'>{hero.Value}</option>";
        }

        string raritiesOptions = "";

        foreach (var rarity in rarities)
        {
            raritiesOptions +=
                $"<option value='{rarity.Key}'>{rarity.Value}</option>";
        }

        html = html.Replace("{{HeroOptions}}", heroesOptions);
        html = html.Replace("{{RarityOptions}}", raritiesOptions);

        return html;
    }

    // Добавление карточки
    public async Task AddCardAsync(
        string code,
        string name,
        decimal price,
        string imageUrl,
        int heroId,
        int rarityId,
        string description)
    {
        await _addCard(
            code,
            name,
            price,
            imageUrl,
            heroId,
            rarityId,
            description);
    }

    // Обработка отправленной формы
    public async Task<bool> ProcessAddCardFormAsync(string body)
    {
        var formData = HttpUtility.ParseQueryString(body);

        string code = formData["code"] ?? "";
        string name = formData["name"] ?? "";
        string priceStr = (formData["price"] ?? "0").Replace(',', '.');

        string imageUrl = formData["imageUrl"] ?? "";
        string description = formData["description"] ?? "";

        if (string.IsNullOrWhiteSpace(code) ||
            string.IsNullOrWhiteSpace(name) ||
            !decimal.TryParse(
                priceStr,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out decimal price))
        {
            return false;
        }

        if (!int.TryParse(formData["heroId"], out int heroId) ||
            !int.TryParse(formData["rarityId"], out int rarityId))
        {
            return false;
        }

        await AddCardAsync(
            code,
            name,
            price,
            imageUrl,
            heroId,
            rarityId,
            description);

        return true;
    }
}
