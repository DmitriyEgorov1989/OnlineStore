namespace OnlineStore.CustomerService.ClientBalancing
{
    public sealed record DbEndPoint(string HostAndPort,DbReplicaType DbReplica, int[]Buckets);
}
