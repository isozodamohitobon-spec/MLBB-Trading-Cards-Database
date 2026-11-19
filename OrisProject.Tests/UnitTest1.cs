
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using NUnit.Framework;
using OrisProject.Data;
using OrisProject.ORM;
using OrisProject.Server;
using System.IO;
using OrisProject.Controllers;
using System.Threading.Tasks;

namespace OrisProject.Tests;

// =====================================
// ТЕСТЫ ШАБЛОНИЗАТОРА
// =====================================

[TestFixture]
public class TemplateEngineTests
{
    [Test]
    public void Render_ReplacesVariable()
    {
        string template = "Привет, {{Name}}!";

        Dictionary<string, string> data = new()
        {
            { "Name", "Мохитобон" }
        };

        string result = TemplateEngine.Render(template, data);

        Assert.That(result, Is.EqualTo("Привет, Мохитобон!"));
    }

    [Test]
    public void Render_ShowsContentWhenConditionIsTrue()
    {
        string template = "{{#if Name}}Привет!{{/if}}";

        Dictionary<string, string> data = new()
        {
            { "Name", "Мохитобон" }
        };

        string result = TemplateEngine.Render(template, data);

        Assert.That(result, Is.EqualTo("Привет!"));
    }

    [Test]
    public void RenderList_RendersAllItems()
    {
        string template = "{{#each Cards}}Карточка{{/each}}";

        List<string> cards = new()
        {
            "Первая",
            "Вторая",
            "Третья"
        };

        string result = TemplateEngine.RenderList(
            template,
            "Cards",
            cards,
            card => card
        );

        Assert.That(result, Is.EqualTo("ПерваяВтораяТретья"));
    }
}

// =====================================
// ТЕСТЫ ORM
// =====================================

[TestFixture]
public class OrmTests
{
    [Test]
    public void Constructor_CreatesOrmContext()
    {
        string connectionString =
            "Host=localhost;Database=oris_db;Username=postgres;Password=test";

        OrmContext orm = new OrmContext(connectionString);

        Assert.That(orm, Is.Not.Null);
    }

    [Test]
    public void InsertAsync_ThrowsExceptionWhenEntityIsNull()
    {
        OrmContext orm = new OrmContext(
            "Host=localhost;Database=oris_db"
        );

        Assert.ThrowsAsync<ArgumentNullException>(
            async () => await orm.InsertAsync<object>("cards", null!)
        );
    }

    [Test]
    public void UpdateAsync_ThrowsExceptionWhenEntityIsNull()
    {
        OrmContext orm = new OrmContext(
            "Host=localhost;Database=oris_db"
        );

        Assert.ThrowsAsync<ArgumentNullException>(
            async () => await orm.UpdateAsync<object>("cards", 1, null!)
        );
    }

    [Test]
    public void InsertAsync_RejectsInvalidTableName()
    {
        OrmContext orm = new OrmContext(
            "Host=localhost;Database=oris_db"
        );

        Assert.ThrowsAsync<ArgumentException>(
            async () => await orm.InsertAsync(
                "cards; DROP TABLE cards",
                new OrisProject.Models.Card()
            )
        );
    }

    [Test]
    public void GetAllAsync_RejectsInvalidTableName()
    {
        OrmContext orm = new OrmContext(
            "Host=localhost;Database=oris_db"
        );

        Assert.ThrowsAsync<ArgumentException>(
            async () => await orm.GetAllAsync<OrisProject.Models.Card>(
                "cards; DROP TABLE cards"
            )
        );
    }

    [Test]
    public void DeleteAsync_RejectsInvalidTableName()
    {
        OrmContext orm = new OrmContext(
            "Host=localhost;Database=oris_db"
        );

        Assert.ThrowsAsync<ArgumentException>(
            async () => await orm.DeleteAsync(
                "cards; DROP TABLE cards",
                1
            )
        );
    }

}

// =====================================
// ТЕСТЫ HTTP-СЕРВЕРА
// =====================================


[TestFixture]
public class HttpServerTests
{
    [Test]
    public async Task UnknownPage_Returns404()
    {
        string templatePath = Path.Combine(
            Path.GetTempPath(),
            "oris_http_test.html");

        File.WriteAllText(
            templatePath,
            "<h1>Тестовая главная страница</h1>");

        HomeController homeController = new HomeController(
            () => new List<OrisProject.Models.Card>(),
            templatePath);

        AdminController adminController = new AdminController(
            () => new List<KeyValuePair<int, string>>(),
            () => new List<KeyValuePair<int, string>>(),
            (code, name, price, image, hero, rarity, description)
                => Task.CompletedTask,
            templatePath);

        CardController cardController = new CardController(
            code => null,
            code => Task.CompletedTask);

        HttpServer? server = null;
        Task? serverTask = null;

        try
        {
            using TcpListener portFinder =
                new TcpListener(IPAddress.Loopback, 0);

            portFinder.Start();

            int port =
                ((IPEndPoint)portFinder.LocalEndpoint).Port;

            portFinder.Stop();

            server = new HttpServer(
                homeController,
                adminController,
                cardController,
                $"http://localhost:{port}/");

            serverTask = server.StartAsync();

            using HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.GetAsync(
                $"http://localhost:{port}/this-page-does-not-exist");

            Assert.That(
                response.StatusCode,
                Is.EqualTo(HttpStatusCode.NotFound));
        }
        finally
        {
            server?.Stop();

            if (serverTask != null)
                await serverTask;

            if (File.Exists(templatePath))
                File.Delete(templatePath);
        }
    }

    [Test]
    public async Task HomePage_Returns200AndHtml()
    {
        string templatePath = Path.Combine(
            Path.GetTempPath(),
            "oris_http_home_test.html");

        File.WriteAllText(
            templatePath,
            "<h1>Тестовая главная страница</h1>");

        HomeController homeController = new HomeController(
            () => new List<OrisProject.Models.Card>(),
            templatePath);

        AdminController adminController = new AdminController(
            () => new List<KeyValuePair<int, string>>(),
            () => new List<KeyValuePair<int, string>>(),
            (code, name, price, image, hero, rarity, description)
                => Task.CompletedTask,
            templatePath);

        CardController cardController = new CardController(
            code => null,
            code => Task.CompletedTask);

        HttpServer? server = null;
        Task? serverTask = null;

        try
        {
            using TcpListener portFinder =
                new TcpListener(IPAddress.Loopback, 0);

            portFinder.Start();

            int port =
                ((IPEndPoint)portFinder.LocalEndpoint).Port;

            portFinder.Stop();

            server = new HttpServer(
                homeController,
                adminController,
                cardController,
                $"http://localhost:{port}/");

            serverTask = server.StartAsync();

            using HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.GetAsync(
                $"http://localhost:{port}/");

            string content = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Does.Contain("Тестовая главная страница"));
        }
        finally
        {
            server?.Stop();

            if (serverTask != null)
                await serverTask;

            if (File.Exists(templatePath))
                File.Delete(templatePath);
        }
    }

    [Test]
    public async Task AdminPage_Returns200()
    {
        string templatePath = Path.Combine(
            Path.GetTempPath(),
            "oris_http_admin_test.html");

        File.WriteAllText(
            templatePath,
            "<h1>Тестовая админ-панель</h1>");

        HomeController homeController = new HomeController(
            () => new List<OrisProject.Models.Card>(),
            templatePath);

        AdminController adminController = new AdminController(
            () => new List<KeyValuePair<int, string>>(),
            () => new List<KeyValuePair<int, string>>(),
            (code, name, price, image, hero, rarity, description)
                => Task.CompletedTask,
            templatePath);

        CardController cardController = new CardController(
            code => null,
            code => Task.CompletedTask);

        HttpServer? server = null;
        Task? serverTask = null;

        try
        {
            using TcpListener portFinder =
                new TcpListener(IPAddress.Loopback, 0);

            portFinder.Start();

            int port =
                ((IPEndPoint)portFinder.LocalEndpoint).Port;

            portFinder.Stop();

            server = new HttpServer(
                homeController,
                adminController,
                cardController,
                $"http://localhost:{port}/");

            serverTask = server.StartAsync();

            using HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.GetAsync(
                $"http://localhost:{port}/admin");

            string content = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Does.Contain("Тестовая админ-панель"));
        }
        finally
        {
            server?.Stop();

            if (serverTask != null)
                await serverTask;

            if (File.Exists(templatePath))
                File.Delete(templatePath);
        }
    }

[Test]
    public async Task GetCard_ReturnsCardAsJson()
    {
        OrisProject.Models.Card testCard =
            new OrisProject.Models.Card
            {
                Id = 1,
                CardCode = "MLBB001",
                CardName = "Test Card",
                Price = 25.50m,
                TimesViewed = 10,
                ImageUrl = "test.png",
                HeroName = "Layla",
                RarityName = "Rare"
            };

        string templatePath = Path.Combine(
            Path.GetTempPath(),
            "oris_http_api_test.html");

        File.WriteAllText(templatePath, "<h1>Test</h1>");

        HomeController homeController = new HomeController(
            () => new List<OrisProject.Models.Card>(),
            templatePath);

        AdminController adminController = new AdminController(
            () => new List<KeyValuePair<int, string>>(),
            () => new List<KeyValuePair<int, string>>(),
            (code, name, price, image, hero, rarity, description)
                => Task.CompletedTask,
            templatePath);

        CardController cardController = new CardController(
            code => code == "MLBB001" ? testCard : null,
            code => Task.CompletedTask);

        HttpServer? server = null;
        Task? serverTask = null;

        try
        {
            using TcpListener portFinder =
                new TcpListener(IPAddress.Loopback, 0);

            portFinder.Start();

            int port = ((IPEndPoint)portFinder.LocalEndpoint).Port;
            portFinder.Stop();

            server = new HttpServer(
                homeController,
                adminController,
                cardController,
                $"http://localhost:{port}/");

            serverTask = server.StartAsync();

            using HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.GetAsync(
                $"http://localhost:{port}/api/card/MLBB001");

            string content = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Does.Contain("MLBB001"));
            Assert.That(content, Does.Contain("Test Card"));
            Assert.That(content, Does.Contain("Layla"));
        }
        finally
        {
            server?.Stop();

            if (serverTask != null)
                await serverTask;

            if (File.Exists(templatePath))
                File.Delete(templatePath);
        }
    }

    [Test]
    public async Task DeleteCard_CallsDeleteController()
    {
        string? deletedCode = null;

        string templatePath = Path.Combine(
            Path.GetTempPath(),
            "oris_http_delete_test.html");

        File.WriteAllText(templatePath, "<h1>Test</h1>");

        HomeController homeController = new HomeController(
            () => new List<OrisProject.Models.Card>(),
            templatePath);

        AdminController adminController = new AdminController(
            () => new List<KeyValuePair<int, string>>(),
            () => new List<KeyValuePair<int, string>>(),
            (code, name, price, image, hero, rarity, description)
                => Task.CompletedTask,
            templatePath);

        CardController cardController = new CardController(
            code => null,
            code =>
            {
                deletedCode = code;
                return Task.CompletedTask;
            });

        HttpServer? server = null;
        Task? serverTask = null;

        try
        {
            using TcpListener portFinder =
                new TcpListener(IPAddress.Loopback, 0);

            portFinder.Start();

            int port = ((IPEndPoint)portFinder.LocalEndpoint).Port;
            portFinder.Stop();

            server = new HttpServer(
                homeController,
                adminController,
                cardController,
                $"http://localhost:{port}/");

            serverTask = server.StartAsync();

            using HttpClientHandler handler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };

            using HttpClient client = new HttpClient(handler);

            HttpResponseMessage response = await client.GetAsync(
                $"http://localhost:{port}/api/delete/MLBB001");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
            Assert.That(deletedCode, Is.EqualTo("MLBB001"));
        }
        finally
        {
            server?.Stop();

            if (serverTask != null)
                await serverTask;

            if (File.Exists(templatePath))
                File.Delete(templatePath);
        }
    }

}


[TestFixture]
public class HomeControllerTests
{
    [Test]
    public void GetIndexPage_DisplaysCardInformation()
    {
        List<OrisProject.Models.Card> cards = new()
        {
            new OrisProject.Models.Card
            {
                Id = 1,
                CardCode = "MLBB001",
                CardName = "Test Card",
                Price = 25.50m,
                TimesViewed = 10,
                ImageUrl = "test.png",
                HeroName = "Layla",
                RarityName = "Rare"
            }
        };

        string templatePath = Path.Combine(
            Path.GetTempPath(),
            "oris_test_index.html"
        );

        File.WriteAllText(
            templatePath,
            "<h1>Карточек: {{CardsCount}}</h1>{{#each Cards}}"
            + "{{/each}}"
        );

        try
        {
            HomeController controller = new HomeController(
                () => cards,
                templatePath
            );

            string result = controller.GetIndexPage();

            Assert.That(result, Does.Contain("Карточек: 1"));
            Assert.That(result, Does.Contain("Test Card"));
            Assert.That(result, Does.Contain("MLBB001"));
            Assert.That(result, Does.Contain("Layla"));
            Assert.That(result, Does.Contain("$25,50"));
        }
        finally
        {
            if (File.Exists(templatePath))
            {
                File.Delete(templatePath);
            }
        }
    }
}

[TestFixture]
public class CardControllerTests
{
    [Test]
    public void GetCard_ReturnsCard()
    {
        OrisProject.Models.Card expectedCard =
            new OrisProject.Models.Card
            {
                Id = 1,
                CardCode = "MLBB001",
                CardName = "Test Card",
                Price = 25.50m,
                TimesViewed = 10,
                ImageUrl = "test.png",
                HeroName = "Layla",
                RarityName = "Rare"
            };

        CardController controller = new CardController(
            code => expectedCard,
            code => Task.CompletedTask
        );

        var result = controller.GetCard("MLBB001");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.CardCode, Is.EqualTo("MLBB001"));
        Assert.That(result.CardName, Is.EqualTo("Test Card"));
    }

    [Test]
    public async Task DeleteCardAsync_CallsDeleteFunction()
    {
        string? deletedCode = null;

        CardController controller = new CardController(
            code => null,
            code =>
            {
                deletedCode = code;
                return Task.CompletedTask;
            }
        );

        await controller.DeleteCardAsync("MLBB001");

        Assert.That(deletedCode, Is.EqualTo("MLBB001"));
    }
}

[TestFixture]
public class AdminControllerTests
{
    [Test]
    public void GetAdminPage_DisplaysHeroesAndRarities()
    {
        string templatePath = Path.Combine(
            Path.GetTempPath(),
            "oris_test_admin.html"
        );

        File.WriteAllText(
            templatePath,
            "<select>{{HeroOptions}}</select>"
            + "<select>{{RarityOptions}}</select>"
        );

        try
        {
            AdminController controller = new AdminController(
                () => new List<KeyValuePair<int, string>>
                {
                    new(1, "Layla")
                },
                () => new List<KeyValuePair<int, string>>
                {
                    new(1, "Rare")
                },
                (code, name, price, image, hero, rarity, description)
                    => Task.CompletedTask,
                templatePath
            );

            string result = controller.GetAdminPage();

            Assert.That(result, Does.Contain("Layla"));
            Assert.That(result, Does.Contain("Rare"));
            Assert.That(result, Does.Contain("value='1'"));
        }
        finally
        {
            if (File.Exists(templatePath))
                File.Delete(templatePath);
        }
    }

    [Test]
    public async Task ProcessAddCardFormAsync_ValidData_ReturnsTrue()
    {
        string? savedCode = null;

        AdminController controller = new AdminController(
            () => new List<KeyValuePair<int, string>>(),
            () => new List<KeyValuePair<int, string>>(),
            (code, name, price, image, hero, rarity, description) =>
            {
                savedCode = code;
                return Task.CompletedTask;
            },
            ""
        );

        string form = "code=MLBB001&name=Test+Card"
            + "&price=25.50&imageUrl=test.png"
            + "&heroId=1&rarityId=1&description=Test";

        bool result = await controller.ProcessAddCardFormAsync(form);

        Assert.That(result, Is.True);
        Assert.That(savedCode, Is.EqualTo("MLBB001"));
    }

    [Test]
    public async Task ProcessAddCardFormAsync_InvalidData_ReturnsFalse()
    {
        AdminController controller = new AdminController(
            () => new List<KeyValuePair<int, string>>(),
            () => new List<KeyValuePair<int, string>>(),
            (code, name, price, image, hero, rarity, description)
                => Task.CompletedTask,
            ""
        );

        string form = "code=&name=Test+Card"
            + "&price=25.50&heroId=1&rarityId=1";

        bool result = await controller.ProcessAddCardFormAsync(form);

        Assert.That(result, Is.False);
    }
}



