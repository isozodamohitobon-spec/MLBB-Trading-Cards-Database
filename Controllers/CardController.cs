
using System;
using System.Threading.Tasks;
using OrisProject.Data;
using OrisProject.Models;

namespace OrisProject.Controllers;

public class CardController
{
    private readonly Func<string, Card?> _getCard;
    private readonly Func<string, Task> _deleteCard;

    public CardController(Database database)
    {
        _getCard = database.GetCardByCode;
        _deleteCard = database.DeleteCardAsync;
    }

    // Конструктор для тестирования
    public CardController(
        Func<string, Card?> getCard,
        Func<string, Task> deleteCard)
    {
        _getCard = getCard;
        _deleteCard = deleteCard;
    }

    // Получение информации о карточке
    public Card? GetCard(string code)
    {
        return _getCard(code);
    }

    // Удаление карточки
    public async Task DeleteCardAsync(string code)
    {
        await _deleteCard(code);
    }
}
