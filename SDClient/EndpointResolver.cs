using System.Collections.Concurrent;

public enum PickStrategy { RoundRobin, Weighted }

public interface IEndpointResolver
{
    (string host, int port) Pick(string cluster, int? bucket = null, PickStrategy strategy = PickStrategy.RoundRobin);
}

public sealed class EndpointResolver(IDbStore store) : IEndpointResolver
{
    private readonly IDbStore _store = store;
    private readonly ConcurrentDictionary<string, int> _rr = new();

    public (string host, int port) Pick(string cluster, int? bucket = null, PickStrategy strategy = PickStrategy.RoundRobin)
    {
        var all = _store.Get(cluster);
        if (all.Count == 0) throw new InvalidOperationException($"No replicas for '{cluster}'");

        var pool = bucket.HasValue
            ? all.Where(r => r.Buckets?.Contains(bucket.Value) ?? false).ToList()
            : all.ToList();

        if (pool.Count == 0) pool = all.ToList();

        return strategy switch
        {
            PickStrategy.Weighted => PickWeighted(pool),
            _ => PickRoundRobin(cluster, pool)
        };
    }

    private (string host, int port) PickRoundRobin(string key, List<ReplicaDescriptor> pool)
    {
        var idx = _rr.AddOrUpdate(key, 0, (_, i) => (i + 1) % pool.Count);
        var r = pool[idx];
        return (r.Host, r.Port);
    }

    private (string host, int port) PickWeighted(List<ReplicaDescriptor> pool)
    {
        var total = pool.Sum(p => Math.Max(1, p.Weight));
        var roll = Random.Shared.Next(1, total + 1);
        var acc = 0;
        foreach (var p in pool)
        {
            acc += Math.Max(1, p.Weight);
            if (roll <= acc) return (p.Host, p.Port);
        }
        var last = pool[^1];
        return (last.Host, last.Port);
    }
}