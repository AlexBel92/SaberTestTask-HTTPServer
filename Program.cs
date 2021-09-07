using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using HTTPServer.Factories;
using HTTPServer.Routers;
using Microsoft.Extensions.Configuration;

namespace HTTPServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var configuration = BuildConfig();
                var baseUri = new Uri(configuration.GetSection("HttpBaseAddress").Value);
                var tcpServerSettings = configuration.GetSection(TcpServerSettings.Position).Get<TcpServerSettings>();

                var server = new HTTPServer(
                    new HttpListener(),
                    baseUri,
                    new ControllerFactory(tcpServerSettings));

                server.RunAsync();

                Console.WriteLine("Press <enter> to close...\n");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static IConfiguration BuildConfig()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true);
            return builder.Build();
        }
    }
}
