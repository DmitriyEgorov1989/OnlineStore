using Grpc.Core;
using OnlineStore.ServiceDiscovery;

namespace OnlineStore.CustomerService.ClientBalancing
{
    public class SdConsumerHostedService : BackgroundService
    {
        private const int SD_TIME_TO_DELAY_MS = 1000;

        private readonly SdService.SdServiceClient _client;
        private readonly ILogger<SdConsumerHostedService> _logger;
        private readonly IDbStore _dbStore;

        public SdConsumerHostedService(SdService.SdServiceClient client,
            ILogger<SdConsumerHostedService> logger, 
            IDbStore dbStore)
        {
            _dbStore = dbStore;
            _client = client;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var request = new DbResourcesRequest
                    {
                        ClusterName = "cluster"
                    };

                    using var stream = _client.DbResources(request, cancellationToken: stoppingToken);

                    await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
                    {
                        _logger.LogInformation( "Get a new data from SD. Timestamp {Timestamp}",
                                                 response.LastUpdated.ToDateTime());

                        var endPoints = GetEndPoints(response).ToList();
                        
                        await _dbStore.UpdateEndPointAsync(endPoints);
                    }
                }
                catch (RpcException ex)
                {
                    _logger.LogError(ex, "SD throw exception.");
                    await Task.Delay(SD_TIME_TO_DELAY_MS, stoppingToken);
                }
            }
        }

        private static IEnumerable<DbEndPoint> GetEndPoints(DbResourcesResponse response)
        {
            return response.Replicas.Select(replica => new DbEndPoint(
            $"{replica.Host}:{replica.Port}",
            ToDbReplica(replica.Type),
            replica.Buckets.ToArray()
        ));
        }

        private static DbReplicaType ToDbReplica(ReplicaType replicaType) =>
        replicaType switch
        {
            ReplicaType.Master => DbReplicaType.Master,
            ReplicaType.Sync => DbReplicaType.Sync,
            ReplicaType.Async => DbReplicaType.Async,
            _ => throw new ArgumentOutOfRangeException(nameof(replicaType), replicaType, null)
        };
    }
}