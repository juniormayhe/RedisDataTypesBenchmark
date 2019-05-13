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
            this.ListForReading = Seed.BuildReasons(totalKeys: 4, totalReasons: 2, totalRemovedEntities: 4);
            this.WarmUpCacheForReading();
        }

        /**
         * Structure for hashes could be
         * 
         * Key - RequestId
         * |__ Field - ProductId:INT_VariantId:GUID_ReasonCode:STRING, Value - semi colon delimited string
         * |__ Field - ProductId:INT_VariantId:GUID_ReasonCode:STRING, Value - semi colon delimited string
         */

        [Benchmark]
        public void O3_Get_Hash()
        {
            foreach (var item in this.ListForReading)
            {
                string key = $"o3_hash_RequestId_{item.RequestId}";
                // field and comma delimited entity ids
                IDictionary<string, string> values = this.Cache.HashGet(key);

                var reasons = new Dictionary<string, IEnumerable<string>>();
                foreach (var kvp in values)
                {
                    reasons.Add(kvp.Key, kvp.Value.Split(','));
                }
            }
        }

        #region private methods
        private void WarmUpCacheForReading()
        {
            foreach (var item in this.ListForReading)
            {
                string key = item.RequestId.ToString();

                var entries = new Dictionary<string, string>();

                foreach (var removedEntityByReason in item.RemovedEntitiesByReason)
                {
                    string productVariantReasonKey = $"{item.GetProductVariantKey()},Reason_{removedEntityByReason.Key}";
                    string entityIds = string.Join(",", removedEntityByReason.Value);

                    entries.Add(productVariantReasonKey, entityIds);
                }
                
                this.Cache.HashSet(key: $"o3_hash_RequestId_{key}", entries);
            }
        }

        #endregion
    }
}
