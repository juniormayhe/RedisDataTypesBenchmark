namespace Infrastructure.CrossCutting.Cache
{
    using System.Net;
    using StackExchange.Redis;

    public interface IRedisConnectionWrapper
    {
        IDatabase Database(int? db = null);

        IServer Server(EndPoint endPoint);

        EndPoint[] GetEndpoints();

        void FlushDb(int? db = null);
    }
}
