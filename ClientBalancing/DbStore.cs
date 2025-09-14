namespace OnlineStore.CustomerService.ClientBalancing
{
    public interface IDbStore
    {
        Task UpdateEndPointAsync(IReadOnlyCollection<DbEndPoint> dbEndpoints);
    }

    public sealed class DbStore : IDbStore
    {
        private const int START_INDEX = 0;

        private DbEndPoint[] _endPoints = Array.Empty<DbEndPoint>();
        public Task UpdateEndPointAsync(IReadOnlyCollection<DbEndPoint> dbEndpoints)
        {
            _endPoints = new DbEndPoint[dbEndpoints.Count];

            foreach (var (dbEndpoint, index) in dbEndpoints.Zip(Enumerable.Range(START_INDEX, dbEndpoints.Count)))
            {
                _endPoints[index] = dbEndpoint;
            }
            return Task.CompletedTask;
        }
    }
}
