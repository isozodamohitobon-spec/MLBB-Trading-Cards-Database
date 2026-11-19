
using System.Collections.Generic;
using System.Threading.Tasks;
using OrisProject.Models;

namespace OrisProject.Data.Repositories.Interfaces;

public interface ICardRepository
{
    List<Card> GetAllCards();

    Card? GetCardByCode(string code);

    Task AddCardAsync(
        string code,
        string name,
        decimal price,
        string imageUrl,
        int heroId,
        int rarityId,
        string description);

    Task DeleteCardAsync(string code);
}
