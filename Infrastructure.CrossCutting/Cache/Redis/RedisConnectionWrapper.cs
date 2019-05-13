namespace Infrastructure.CrossCutting.Cache.Redis
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using Infrastructure.CrossCutting.Settings;
    using StackExchange.Redis;

    [ExcludeFromCodeCoverage]
    public class RedisConnectionWrapper : IRedisConnectionWrapper, IDisposable
    {
        private readonly RedisSettings redisSettings;
        private readonly Lazy<string> connectionString;
        private readonly object @lock = new object();
        private volatile ConnectionMultiplexer connection;

        public RedisConnectionWrapper(RedisSettings redisSettings)
        {
            this.redisSettings = redisSettings;
            this.connectionString = new Lazy<string>(() =>
            {
                return this.redisSettings.Server;
            });
        }

        public IDatabase Database(int? db = null)
        {
            var connection = this.GetConnection();
            if (connection != null)
            {
                return connection.GetDatabase(db ?? this.redisSettings.DefaultDb);
            }
            else
            {
                return null;
            }
        }

        public IServer Server(EndPoint endPoint)
        {
            return this.GetConnection().GetServer(endPoint);
        }

        public IServer FirstServer()
        {
            EndPoint[] endPoint = connection.GetEndPoints();
            return this.GetConnection().GetServer(endPoint[0]);
        }

        public EndPoint[] GetEndpoints()
        {
            return this.GetConnection().GetEndPoints();
        }

        public void FlushDb(int? db = null)
        {
            var endPoints = this.GetEndpoints();

            foreach (var endPoint in endPoints)
            {
                this.Server(endPoint).FlushDatabase(db ?? this.redisSettings.DefaultDb);
            }
        }

        public void Dispose()
        {
            this.connection?.Dispose();
        }

        #region Private Methods

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns></returns>
        private ConnectionMultiplexer GetConnection()
        {
            if (this.connection != null && this.connection.IsConnected)
            {
                return this.connection;
            }

            lock (this.@lock)
            {
                if (this.connection != null && this.connection.IsConnected)
                {
                    return this.connection;
                }

                try
                {
                    
                    this.connection = ConnectionMultiplexer.Connect(this.connectionString.Value);
                }
                catch (Exception ex)
                {
                    
                }
            }

            return this.connection;
        }

        public override bool Equals(object obj)
        {
            return obj is RedisConnectionWrapper wrapper &&
                   EqualityComparer<RedisSettings>.Default.Equals(redisSettings, wrapper.redisSettings) &&
                   EqualityComparer<Lazy<string>>.Default.Equals(connectionString, wrapper.connectionString) &&
                   EqualityComparer<object>.Default.Equals(@lock, wrapper.@lock) &&
                   EqualityComparer<ConnectionMultiplexer>.Default.Equals(connection, wrapper.connection);
        }

        public override int GetHashCode()
        {
            var hashCode = 1076618128;
            hashCode = hashCode * -1521134295 + EqualityComparer<RedisSettings>.Default.GetHashCode(redisSettings);
            hashCode = hashCode * -1521134295 + EqualityComparer<Lazy<string>>.Default.GetHashCode(connectionString);
            hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(@lock);
            hashCode = hashCode * -1521134295 + EqualityComparer<ConnectionMultiplexer>.Default.GetHashCode(connection);
            return hashCode;
        }

        #endregion Private Methods
    }
}
