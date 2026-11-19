
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OrisProject.Data;
using OrisProject.Controllers;

namespace OrisProject.Server;

public class HttpServer
{
    private readonly HomeController _homeController;
    private readonly AdminController _adminController;
    private readonly CardController _cardController;
    private readonly HttpListener _listener;

    private bool _isStopping;

    public HttpServer(Database db, string prefix = "http://localhost:8080/")
        : this(
            new HomeController(db),
            new AdminController(db),
            new CardController(db),
            prefix)
    {
    }

    public HttpServer(
        HomeController homeController,
        AdminController adminController,
        CardController cardController,
        string prefix = "http://localhost:8080/")
    {
        _homeController = homeController;
        _adminController = adminController;
        _cardController = cardController;

        _listener = new HttpListener();
        _listener.Prefixes.Add(prefix);
    }

    public async Task StartAsync()
    {
        _isStopping = false;
        _listener.Start();

        Console.WriteLine("HTTP-сервер запущен!");

        while (!_isStopping && _listener.IsListening)
        {
            HttpListenerContext context;

            try
            {
                context = await _listener.GetContextAsync();
            }
            catch (HttpListenerException)
            {
                if (_isStopping)
                    break;

                throw;
            }
            catch (ObjectDisposedException)
            {
                if (_isStopping)
                    break;

                throw;
            }

            await ProcessRequestAsync(context);
        }
    }

    public void Stop()
    {
        _isStopping = true;

        if (_listener.IsListening)
            _listener.Stop();
    }

    public async Task ProcessRequestAsync(HttpListenerContext context)
    {
        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;

        try
        {
            string path = request.Url?.AbsolutePath ?? "/";
            string responseString = "";

            if (path == "/" || path == "/index.html")
            {
                responseString = _homeController.GetIndexPage();
                response.ContentType = "text/html; charset=utf-8";
            }
            else if (path == "/admin" && request.HttpMethod == "GET")
            {
                responseString = _adminController.GetAdminPage();
                response.ContentType = "text/html; charset=utf-8";
            }
            else if (path == "/admin" && request.HttpMethod == "POST")
            {
                using var reader = new StreamReader(
                    request.InputStream,
                    request.ContentEncoding);

                string body = await reader.ReadToEndAsync();

                bool success =
                    await _adminController.ProcessAddCardFormAsync(body);

                if (success)
                {
                    response.StatusCode = 302;
                    response.RedirectLocation = "/";
                }
                else
                {
                    response.StatusCode = 400;
                    responseString =
                        "<h1>Ошибка валидации</h1>" +
                        "<p>Проверьте введённые данные.</p>" +
                        "<a href='/admin'>Назад</a>";

                    response.ContentType = "text/html; charset=utf-8";
                }
            }
            else if (path.StartsWith("/api/card/"))
            {
                string code = path.Replace("/api/card/", "").Trim();

                var card = _cardController.GetCard(code);

                if (card != null)
                {
                    responseString =
                        System.Text.Json.JsonSerializer.Serialize(
                            new
                            {
                                cardCode = card.CardCode,
                                cardName = card.CardName,
                                heroName = card.HeroName,
                                rarityName = card.RarityName,
                                price = card.Price,
                                timesViewed = card.TimesViewed,
                                imageUrl = card.ImageUrl
                            });

                    response.ContentType =
                        "application/json; charset=utf-8";
                }
                else
                {
                    response.StatusCode = 404;
                    responseString = "{\"error\": \"Card not found\"}";
                    response.ContentType = "application/json";
                }
            }
            else if (path.StartsWith("/api/delete/"))
            {
                string code = path.Replace("/api/delete/", "").Trim();

                await _cardController.DeleteCardAsync(code);

                response.StatusCode = 302;
                response.RedirectLocation = "/";
            }
            else if (
                path.StartsWith("/images/") ||
                path.StartsWith("/css/") ||
                path.StartsWith("/js/"))
            {
                string filePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    path.TrimStart('/'));

                if (File.Exists(filePath))
                {
                    byte[] fileBytes =
                        await File.ReadAllBytesAsync(filePath);

                    if (path.StartsWith("/images/"))
                    {
                        response.ContentType =
                            filePath.EndsWith(".jpg") ||
                            filePath.EndsWith(".jpeg")
                                ? "image/jpeg"
                                : "image/png";
                    }
                    else if (path.StartsWith("/css/"))
                    {
                        response.ContentType = "text/css";
                    }
                    else
                    {
                        response.ContentType =
                            "application/javascript";
                    }

                    response.ContentLength64 = fileBytes.Length;

                    await response.OutputStream.WriteAsync(fileBytes);
                    return;
                }

                response.StatusCode = 404;
                responseString = "Файл не найден.";
            }
            else
            {
                response.StatusCode = 404;

                responseString =
                    "<h1>404 - Страница не найдена</h1>" +
                    "<a href='/'>На главную</a>";

                response.ContentType = "text/html; charset=utf-8";
            }

            byte[] buffer = Encoding.UTF8.GetBytes(responseString);

            response.ContentLength64 = buffer.Length;

            await response.OutputStream.WriteAsync(buffer);
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;

            byte[] buffer = Encoding.UTF8.GetBytes(
                $"<h1>Ошибка сервера: {WebUtility.HtmlEncode(ex.Message)}</h1>");

            response.ContentType = "text/html; charset=utf-8";
            response.ContentLength64 = buffer.Length;

            await response.OutputStream.WriteAsync(buffer);
        }
        finally
        {
            response.Close();
        }
    }
}
