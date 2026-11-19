
using OrisProject.Data;
using OrisProject.Server;

namespace OrisProject;

class Program
{
    static async Task Main(string[] args)
    {
        Database db = new Database();

        string prefix = Environment.GetEnvironmentVariable("ORIS_HTTP_PREFIX")
            ?? "http://localhost:8080/";

        HttpServer server = new HttpServer(db, prefix);

        await server.StartAsync();
    }
}
