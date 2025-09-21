using System.Collections.Concurrent;

public sealed class InMemoryDbStore : IDbStore
{
    private readonly ConcurrentDictionary<string, List<ReplicaDescriptor>> _data = new();

    public void Upsert(string cluster, IReadOnlyList<ReplicaDescriptor> replicas)
        => _data[cluster] = replicas.ToList();

    public IReadOnlyList<ReplicaDescriptor> Get(string cluster)
        => _data.TryGetValue(cluster, out var list) ? list : Array.Empty<ReplicaDescriptor>();
}