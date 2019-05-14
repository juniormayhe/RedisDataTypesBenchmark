namespace RedisSetBenchmarks
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Order;

    using Infrastructure.CrossCutting.Cache;

    using RedisShared;

    using System.Collections.Generic;

    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    [MemoryDiagnoser]
    public class SetBenchmarksWrite
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
         * Key - RequestId_GUID
         * |__ Value - ProductId:INT|VariantId:GUID|Reason:STRING:Entities
         * |__ Value - ProductId:INT|VariantId:GUID|Reason:STRING:Entities
         * 
         * where both Reason is a string and Entities is a semi colon delimited string
         */
        [Benchmark]
        public void O1_SetAddAll_RequestIdInKey()
        {
            foreach (var item in this.ListForWriting)
            {
                string key = $"o4_set:RequestId_{item.RequestId}";
                var entriesForSet = new List<string>();
                foreach (var removedEntityByReason in item.RemovedEntitiesByReason)
                {
                    string productVariantReasonKey = $"ProductId_{item.ProductId}|VariantId_{item.VariantId}|Reason_{removedEntityByReason.Key}";
                    string entityIds = string.Join(",", removedEntityByReason.Value);

                    entriesForSet.Add($"{productVariantReasonKey}:{entityIds}");
                }
                this.Cache.SetAddAll(key, entriesForSet);
            }
        }

        /**
         * Structure for hashes could be
         * 
         * Key - RequestId_GUID:ProductId:INT
         * |__ Value - VariantId:GUID|Reason:STRING:Entities
         * |__ Value - VariantId:GUID|Reason:STRING:Entities
         * 
         * where both Reason is a string and Entities is a semi colon delimited string
         */
        [Benchmark]
        public void O2_SetAddAll_RequestIdAndProductdInKey()
        {
            foreach (var item in this.ListForWriting)
            {
                string key = $"o4_set:RequestId_{item.RequestId}:ProductId_{item.ProductId}";
                var entriesForSet = new List<string>();
                foreach (var removedEntityByReason in item.RemovedEntitiesByReason)
                {
                    string variantAndReason = $"VariantId_{item.VariantId}|Reason_{removedEntityByReason.Key}";
                    string entityIds = string.Join(",", removedEntityByReason.Value);

                    entriesForSet.Add($"{variantAndReason}:{entityIds}");
                }
                this.Cache.SetAddAll(key, entriesForSet);
            }
        }

        /**
         * Structure for hashes could be
         * 
         * Key - RequestId_GUID:ProductId_INT:VariantId_GUID
         * |__ Value - Reason:Entities
         * |__ Value - Reason:Entities
         * 
         * where both Reason is a string and Entities is a semi colon delimited string
         */
        [Benchmark]
        public void O3_SetAddAll_AllFieldsInKey()
        {
            foreach (var item in this.ListForWriting)
            {
                string key = $"o4_set:{item.GetFullKey()}";
                var entriesForSet = new List<string>();
                foreach (var removedEntityByReason in item.RemovedEntitiesByReason)
                {
                    //add fields for Reason and RemovedEntityIds
                    string reasonKey = removedEntityByReason.Key;
                    string entityIds = string.Join(",", removedEntityByReason.Value);
                    
                    entriesForSet.Add($"{reasonKey}:{entityIds}");
                }
                this.Cache.SetAddAll(key, entriesForSet);
            }
        }
    }
}
