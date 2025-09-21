using Grpc.Core;
using Microsoft.Extensions.Options;
using OnlineStore.ServiceDiscovery;

public sealed class SdOptions
{
    public int UpdateIntervalSeconds { get; set; } = 5;
    public Dictionary<string, List<ReplicaInfoDto>> Clusters { get; set; } = new();
    public sealed class ReplicaInfoDto
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; }
        public string Type { get; set; } = "MASTER";
        public int[]? Buckets { get; set; }
        public int Weight { get; set; } = 1;
    }
}

public sealed class SdGrpcService(IOptionsMonitor<SdOptions> options)
    : SdService.SdServiceBase
{
    private readonly IOptionsMonitor<SdOptions> _opt = options;

    public override async Task DbResources(DbResourcesRequest request,
        IServerStreamWriter<DbResourcesResponse> stream,
        ServerCallContext ctx)
    {
        var interval = TimeSpan.FromSeconds(_opt.CurrentValue.UpdateIntervalSeconds);

        while (!ctx.CancellationToken.IsCancellationRequested)
        {
            var resp = BuildResponse(request.ClusterName);
            await stream.WriteAsync(resp);
            await Task.Delay(interval, ctx.CancellationToken);
        }
    }

    private DbResourcesResponse BuildResponse(string cluster)
    {
        var resp = new DbResourcesResponse
        {
            ClusterName = cluster,
            LastUpdated = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow)
        };

        if (_opt.CurrentValue.Clusters.TryGetValue(cluster, out var list))
        {
            foreach (var x in list)
            {
                resp.Replicas.Add(new Replica
                {
                    Host = x.Host,
                    Port = x.Port,
                    Type = Enum.TryParse<ReplicaType>(x.Type, true, out var t) ? t : ReplicaType.Master,
                    Buckets = { x.Buckets ?? Array.Empty<int>() },
                    Weight = x.Weight
                });
            }
        }
        return resp;
    }
}