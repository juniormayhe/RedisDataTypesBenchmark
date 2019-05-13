namespace RedisShared
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
                IDictionary<string, IEnumerable<string>> removedEntitiesByReason = new Dictionary<string, IEnumerable<string>>();
                List<string> reasons = fixture.CreateMany<string>(totalReasons).ToList();
                foreach (var reason in reasons)
                {
                    var removed = fixture.CreateMany<int>(totalRemovedEntities).Select(x => x.ToString());
                    removedEntitiesByReason.Add(reason, removed);
                }

                RoutingLog routingLog = fixture.Build<RoutingLog>().With(x => x.RemovedEntitiesByReason, removedEntitiesByReason).Create();

                logs.Add(routingLog);
            }

            return logs;
        }
    }
}
