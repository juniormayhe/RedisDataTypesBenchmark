namespace RedisHashBenchmarks
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Order;
    using Infrastructure.CrossCutting.Cache;
    using RedisShared;
    using System;
    using System.Collections.Generic;
    using System.Text;

    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    [MemoryDiagnoser]
    public class HashBenchmarks
    {
        public IEnumerable<RoutingLog> ListForWriting { get; set; }
        public ICacheStore Cache { get; set; }

        [GlobalSetup]
        public void InitialData()
        {
            this.Cache = CacheHelper.GetCacheStore();

            this.ListForWriting = Seed.BuildReasons(totalKeys: 4, totalReasons: 2, totalRemovedEntities: 4);
        }

        /**
         * Structure for hashes could be
         * 
         * Key - RequestId
         * |__ Field - ProductId_VariantId_ReasonCode, Value - semi colon delimited string
         * |__ Field - ProductId_VariantId_ReasonCode, Value - semi colon delimited string
         */

        [Benchmark]
        public void O3_Set_Hash()
        {
            foreach (var item in this.ListForWriting)
            {
                string key = $"o3_hash_RequestId_{item.RequestId}";
                IDictionary<string, string> entries = new Dictionary<string, string>();
                foreach (var removedEntityByReason in item.RemovedEntitiesByReason)
                {
                    string productVariantReasonKey = $"{item.GetProductVariantKey()},Reason_{removedEntityByReason.Key}";
                    string entityIds = string.Join(",", removedEntityByReason.Value);

                    entries.Add(productVariantReasonKey, entityIds);
                }
                this.Cache.HashSet(key, entries);
            }
        }
    }
}
