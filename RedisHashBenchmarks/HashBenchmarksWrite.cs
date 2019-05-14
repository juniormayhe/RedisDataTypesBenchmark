namespace RedisHashBenchmarks
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Order;

    using Infrastructure.CrossCutting.Cache;

    using RedisShared;

    using System.Collections.Generic;

    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    [MemoryDiagnoser]
    public class HashBenchmarksWrite
    {
        public IEnumerable<RoutingLog> ListForWriting { get; set; }
        public ICacheStore Cache { get; set; }

        [GlobalSetup]
        public void InitialData()
        {
            this.Cache = CacheHelper.GetCacheStore();

            this.ListForWriting = Seed.BuildReasons(totalKeys: 5000, totalReasons: 2, totalRemovedEntities: 4);
        }

        /**
         * Structure for hashes could be
         * 
         * Key - RequestId
         * |__ Field - ProductId:INT_VariantId:GUID_Reason:STRING, Value - semi colon delimited string
         * |__ Field - ProductId:INT_VariantId:GUID_Reason:STRING, Value - semi colon delimited string
         */
        [Benchmark]
        public void O3_Set_Hash()
        {
            foreach (var item in this.ListForWriting)
            {
                string key = $"o3_hash:RequestId_{item.RequestId}";
                IDictionary<string, string> entriesForHash = new Dictionary<string, string>();
                foreach (var removedEntityByReason in item.RemovedEntitiesByReason)
                {
                    string productVariantReasonKey = $"{item.GetProductVariantKey()},Reason_{removedEntityByReason.Key}";
                    string entityIds = string.Join(",", removedEntityByReason.Value);

                    entriesForHash.Add(productVariantReasonKey, entityIds);
                }
                this.Cache.HashSet(key, entriesForHash);
            }
        }

        /**
         * Structure for hashes could be
         * 
         * Key - RequestId:ProductId:INT_VariantId:GUID
         * |__ Field - Reason:STRING, Value - semi colon delimited string
         * |__ Field - Reason:STRING, Value - semi colon delimited string
         */
        [Benchmark]
        public void O3_Set_Hash_AllFieldsInKey()
        {
            foreach (var item in this.ListForWriting)
            {
                string key = $"o3_hash:{item.GetFullKey()}";
                IDictionary<string, string> entriesForHash = new Dictionary<string, string>();
                foreach (var removedEntityByReason in item.RemovedEntitiesByReason)
                {
                    //add fields for Reason and RemovedEntityIds
                    string reasonKey = removedEntityByReason.Key;
                    string entityIds = string.Join(",", removedEntityByReason.Value);
                    
                    entriesForHash.Add(reasonKey, entityIds);
                }
                this.Cache.HashSet(key, entriesForHash);
            }
        }
    }
}
