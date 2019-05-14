namespace RedisSetBenchmarks
{
    using BenchmarkDotNet.Running;
    using Infrastructure.CrossCutting.Cache;
    using RedisShared;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            //quickTest();

            Console.WriteLine("Starting application");

            var writeSummary = BenchmarkRunner.Run<SetBenchmarksWrite>();
            var readSummary = BenchmarkRunner.Run<SetBenchmarksRead>();

            // clean up data
            // CacheHelper.GetCacheStore().Truncate(new []{ "o3_*"});

            Console.WriteLine("The end");

            Console.ReadLine();
        }

        private static void quickTest()
        {
            ICacheStore Cache = CacheHelper.GetCacheStore();

            IEnumerable<RoutingLog> ListForReading = Seed.BuildReasons(totalKeys: 4, totalReasons: 2, totalRemovedEntities: 4);
            // warm
            foreach (var item in ListForReading)
            {
                string key = item.GetFullKey();

                var entriesForHash = new Dictionary<string, string>();

                foreach (var removedEntityByReason in item.RemovedEntitiesByReason)
                {
                    string reasonCode = removedEntityByReason.Key;
                    string entityIds = string.Join(",", removedEntityByReason.Value);

                    entriesForHash.Add(reasonCode, entityIds);
                }

                Cache.HashSet(key: $"o3_hash:{key}", entriesForHash);
            }

            //read
            var reasons = new Dictionary<string, IEnumerable<string>>();
            foreach (var item in ListForReading)
            {
                string key = $"o3_hash:{item.GetFullKey()}";
                // field and comma delimited entity ids
                IDictionary<string, string> values = Cache.HashGet(key);

                List<string> items = new List<string>();
                foreach (var kvp in values)
                {
                    string reason = kvp.Key;
                    string reasonAndRemovedEntities = kvp.Value;
                    items.Add($"{reason}:{reasonAndRemovedEntities}");
                }
                reasons.Add(key, items);
            }
        }


    }
}
