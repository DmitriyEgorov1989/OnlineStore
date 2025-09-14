using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

namespace OnlineStore.CustomerService
{
    public sealed class Program
    {
        public static async Task Main(string[] args)
        {
            await Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>()
                                                            .ConfigureKestrel(option =>
                                                            {
                                                                option.ListenPortByOptions(ProgramExtension.ONLINESTORE_GRPC_PORT, HttpProtocols.Http2);
                                                                option.ListenPortByOptions(ProgramExtension.ONLINESTORE_HTTP_PORT, HttpProtocols.Http1);
                                                            }))

                .Build()
                .RunAsync();

        }
    }
    public static class ProgramExtension
    {
        public const string ONLINESTORE_GRPC_PORT = "ONLINESTORE_GRPC_PORT";
        public const string ONLINESTORE_HTTP_PORT = "ONLINESTORE_HTTP_PORT";

        public static void ListenPortByOptions(
            this KestrelServerOptions option,
            string envOption,
            HttpProtocols httpProtocols)
        {
            var isHttpParsed = int.TryParse(Environment.GetEnvironmentVariable(envOption), out var httpPort);

            if (isHttpParsed)
            {
                option.Listen(IPAddress.Any, httpPort, option => option.Protocols = httpProtocols);
            }
        }
    }
}