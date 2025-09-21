public interface IDbStore
{
    void Upsert(string cluster, IReadOnlyList<ReplicaDescriptor> replicas);
    IReadOnlyList<ReplicaDescriptor> Get(string cluster);
}

public sealed record ReplicaDescriptor(
    string Host, int Port, string Role, int[] Buckets, int Weight);