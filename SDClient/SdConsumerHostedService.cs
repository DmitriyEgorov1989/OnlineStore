using Grpc.Net.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Route256.ServiceDiscovery;

public sealed class SdConsumerHostedService(
    ILogger<SdConsumerHostedService> log,
    IConfiguration cfg,
    IDbStore store) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var sdAddr = cfg["SD:Address"] ?? "http://localhost:7000";
        var clusters = (cfg.GetSection("SD:Clusters").Get<string[]>() ?? Array.Empty<string>()).Distinct().ToArray();

        using var channel = GrpcChannel.ForAddress(sdAddr);
        var client = new SdService.SdServiceClient(channel);

        await Task.WhenAll(clusters.Select(c => ConsumeCluster(client, c, ct)));
    }

    private async Task ConsumeCluster(SdService.SdServiceClient client, string cluster, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                using var call = client.DbResources(new DbResourcesRequest { ClusterName = cluster }, cancellationToken: ct);
                await foreach (var msg in call.ResponseStream.ReadAllAsync(ct))
                {
                    var replicas = msg.Replicas.Select(r =>
                        new ReplicaDescriptor(r.Host, r.Port, r.Type.ToString(), r.Buckets.ToArray(), r.Weight)).ToList();

                    store.Upsert(cluster, replicas);
                }
            }
            catch (Exception ex) when (!ct.IsCancellationRequested)
            {
                Console.WriteLine($"[SD] stream '{cluster}' error: {ex.Message}");
                await Task.Delay(1000, ct);
            }
        }
    }
}