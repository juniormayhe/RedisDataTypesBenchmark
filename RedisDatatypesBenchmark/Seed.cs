namespace RedisDatatypesBenchmark
{
    using AutoFixture;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Seed
    {
        public static IEnumerable<RoutingLog> BuildReasons(int totalKeys, int totalReasons, int totalRemovedEntities)
        {
            Fixture fixture = new Fixture();

            var logs = new List<RoutingLog>();

            for (int i = 0; i < totalKeys; i++)
            {
                IEnumerable<string> removedEntities = fixture.CreateMany<int>(totalRemovedEntities).Select(entityId => Convert.ToString(entityId));

                IDictionary<string, IEnumerable<string>> removedEntitiesByReason = fixture.Build<KeyValuePair<string, IEnumerable<string>>>()
                    .CreateMany(totalReasons)
                    .ToDictionary(x => x.Key, x => removedEntities);

                RoutingLog routingLog = fixture.Build<RoutingLog>().With(x => x.RemovedEntitiesByReason, removedEntitiesByReason).Create();

                logs.Add(routingLog);
            }

            return logs;
        }
    }
}