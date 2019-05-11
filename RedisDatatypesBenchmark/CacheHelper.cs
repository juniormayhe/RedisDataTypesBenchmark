

namespace RedisDatatypesBenchmark
{
    using Infrastructure.CrossCutting.Cache;
    using Infrastructure.CrossCutting.Cache.Redis;
    using Infrastructure.CrossCutting.Settings;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class CacheHelper
    {

        public static ICacheStore GetCacheStore()
        {
            RedisSettings redisSettings = GetRedisSettings();

            var serviceProvider = new ServiceCollection()
                            // Application Dependency Injection
                            .AddSingleton<IRedisConnectionWrapper, RedisConnectionWrapper>(resolver => new RedisConnectionWrapper(redisSettings))
                            .AddSingleton<ICacheStore>(sp => new RedisCacheStore(sp.GetRequiredService<IRedisConnectionWrapper>(), redisSettings))
                            .BuildServiceProvider();

            // is redis alive?
            return serviceProvider.GetRequiredService<ICacheStore>();
        }

        private static RedisSettings GetRedisSettings()
        {
            var redisSettings = new RedisSettings();
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            config.GetSection(nameof(RedisSettings)).Bind(redisSettings);
            return redisSettings;
        }
    }
}
