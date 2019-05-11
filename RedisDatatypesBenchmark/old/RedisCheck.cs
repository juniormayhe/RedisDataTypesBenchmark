namespace RedisDatatypesBenchmark
{
    using Infrastructure.CrossCutting.Cache;

    using System;

    public class RedisCheck
    {
        private readonly ICacheStore cache;

        public RedisCheck(ICacheStore cache)
        {
            this.cache = cache;
        }

        public string Description { get; set; }

        public void Check()
        {
            Description = "Could not check REDIS. Check your connection";

            try
            {
                var id = Guid.NewGuid();

                var key = $"BM_RoutingService_HealthCheckTest_{id}";

                this.cache.JsonSet(key, id, TimeSpan.FromSeconds(5));

                var savedInfo = this.cache.GetJson<Guid>(key);

                if (id != savedInfo)
                {
                    throw new ApplicationException("Incorrect value in Redis Cache");
                }

                this.cache.Remove(key);

                savedInfo = this.cache.GetJson<Guid>(key);

                if (savedInfo != default(Guid))
                {
                    throw new ApplicationException("Error removing info from Redis Cache");
                }

                this.Description = "Redis is working fine";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in Redis Cache", ex);
            }

            Console.WriteLine(this.Description);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
