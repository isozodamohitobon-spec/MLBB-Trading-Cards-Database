
using System;
using System.Linq;
using System.Threading.Tasks;
using OrisProject.Models;
using OrisProject.ORM;

namespace OrisProject.Data
{
    public class OrmTest
    {
        public static async Task Run()
        {
            string connectionString =
                Environment.GetEnvironmentVariable("ORIS_DB_CONNECTION")
                ?? throw new InvalidOperationException(
                    "Не задана строка подключения ORIS_DB_CONNECTION");

            OrmContext orm = new OrmContext(connectionString);

            Console.WriteLine("Проверяем подключение ORM...");

            // READ: получаем все карточки
            var cards = await orm.GetAllAsync<Card>("cards");

            Console.WriteLine($"Всего карточек: {cards.Count}");

            // Создаём уникальный код тестовой карточки
            string testCode =
                "ML-AS-" + Guid.NewGuid().ToString("N")[..4].ToUpper();

            Card testCard = new Card
            {
                CardCode = testCode,
                CardName = "ORM Test Card",
                Price = 15.99m,
                TimesViewed = 0,
                ImageUrl = "",
                HeroId = 1,
                RarityId = 1,
                Description = "Temporary ORM test card"
            };

            // CREATE: добавляем карточку
            Console.WriteLine("Проверяем InsertAsync...");

            await orm.InsertAsync("cards", testCard);

            Console.WriteLine("Карточка добавлена!");

            // Находим добавленную карточку
            var addedCards = await orm.GetAllAsync<Card>("cards");

            Card? addedCard = addedCards
                .FirstOrDefault(c => c.CardCode == testCode);

            if (addedCard == null)
                throw new Exception("Не удалось найти добавленную карточку.");

            // UPDATE: изменяем цену
            Console.WriteLine("Проверяем UpdateAsync...");

            addedCard.Price = 25.99m;

            await orm.UpdateAsync("cards", addedCard.Id, addedCard);

            Console.WriteLine("Карточка изменена!");

            // DELETE: удаляем тестовую карточку
            Console.WriteLine("Проверяем DeleteAsync...");

            await orm.DeleteAsync("cards", addedCard.Id);

            Console.WriteLine("Тестовая карточка удалена!");

            Console.WriteLine("Проверка ORM завершена!");
        }
    }
}


