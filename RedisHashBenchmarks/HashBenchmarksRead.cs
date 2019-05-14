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
    public class HashBenchmarksRead
    {
        public IEnumerable<RoutingLog> ListForReading { get; set; }

        public ICacheStore Cache { get; set; }

        [GlobalSetup]
        public void InitialData()
        {
            this.Cache = CacheHelper.GetCacheStore();

            // warmup cache for further reading
            this.ListForReading = Seed.BuildReasons(totalKeys: 5000, totalReasons: 2, totalRemovedEntities: 4);
            this.WarmUpCacheForReadingWithRequestIdAsKey();
            this.WarmUpCacheForReadingWithAllFieldsAsKey();
        }

        /**
         * Structure for hashes could be
         * 
         * Key - RequestId
         * |__ Field - ProductId:INT_VariantId:GUID_Reason:STRING, Value - semi colon delimited string
         * |__ Field - ProductId:INT_VariantId:GUID_Reason:STRING, Value - semi colon delimited string
         */
        [Benchmark]
        public void O3_Get_Hash()
        {
            var reasons = new Dictionary<string, IEnumerable<string>>();
            foreach (var item in ListForReading)
            {
                string key = $"o3_hash:RequestId_{item.RequestId}";
                
                IDictionary<string, string> values = Cache.HashGet(key);

                List<string> items = new List<string>();
                foreach (var kvp in values)
                {
                    string productAndVariantAndReason = kvp.Key;
                    string reasonAndRemovedEntities = kvp.Value;
                    items.Add($"{productAndVariantAndReason}:{reasonAndRemovedEntities}");
                }
                reasons.Add(key, items);
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
        public void O3_Get_Hash_AllFieldsInKey()
        {
            var reasons = new Dictionary<string, IEnumerable<string>>();
            foreach (var item in this.ListForReading)
            {
                string key = $"o3_hash:{item.GetFullKey()}";
                
                IDictionary<string, string> values = this.Cache.HashGet(key);

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

        #region private methods
        private void WarmUpCacheForReadingWithRequestIdAsKey()
        {
            foreach (var item in this.ListForReading)
            {
                string key = $"o3_hash:RequestId_{item.RequestId}";

                var entriesForHash = new Dictionary<string, string>();

                foreach (var removedEntityByReason in item.RemovedEntitiesByReason)
                {
                    string productVariantReasonKey = $"{item.GetProductVariantKey()},Reason_{removedEntityByReason.Key}";
                    string entityIds = string.Join(",", removedEntityByReason.Value);

                    entriesForHash.Add(productVariantReasonKey, entityIds);
                }
                
                this.Cache.HashSet(key, entriesForHash);
            }
        }

        private void WarmUpCacheForReadingWithAllFieldsAsKey()
        {
            foreach (var item in this.ListForReading)
            {
                string key = $"o3_hash:{item.GetFullKey()}";

                var entriesForHash = new Dictionary<string, string>();

                foreach (var removedEntityByReason in item.RemovedEntitiesByReason)
                {
                    string reasonCode = removedEntityByReason.Key;
                    string entityIds = string.Join(",", removedEntityByReason.Value);

                    entriesForHash.Add(reasonCode, entityIds);
                }

                this.Cache.HashSet(key, entriesForHash);
            }
        }

        #endregion
    }
}
