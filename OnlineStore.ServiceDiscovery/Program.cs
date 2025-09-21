using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// gRPC на 7000 строго по HTTP/2 (h2c)
builder.WebHost.ConfigureKestrel(k =>
{
    k.ListenAnyIP(7000, o => o.Protocols = HttpProtocols.Http2);
    k.ListenAnyIP(7002, o => o.Protocols = HttpProtocols.Http1); // health h1
});

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<SdGrpcService>();   // gRPC тут
app.MapGet("/", () => "ServiceDiscovery is up"); // простой health на том же порту (вернёт h2)

app.Run();