namespace RedisDatatypesBenchmark
{
    using System;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Order;

    using Infrastructure.CrossCutting.Cache;
    using Infrastructure.CrossCutting.Cache.Redis;
    using Infrastructure.CrossCutting.Settings;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    [MemoryDiagnoser]
    public class RedisBenchmarks
    {
        public IEnumerable<RoutingLog> ListForReading { get; set; }
        public IEnumerable<RoutingLog> ListForWriting { get; set; }

        public ICacheStore Cache { get; set; }


        [GlobalSetup]
        public void InitialData()
        {
            this.Cache = CacheHelper.GetCacheStore();

            // warm cache for further reading
            this.ListForReading = Seed.BuildReasons(totalKeys: 1, totalReasons: 2, totalRemovedEntities: 4);

            var cache = CacheHelper.GetCacheStore();
            foreach (var item in this.ListForReading)
            {
                string values = JsonConvert.SerializeObject(item.RemovedEntitiesByReason);
                string key = item.GetKey();
                cache.Set(key: $"o1_delimited{key}", values);
                cache.Set(key: $"o2_json{key}", values);
                cache.Set(key: $"o2_jiljson{key}", values);
                cache.Set(key: $"o2_hash{key}", values);
                cache.Set(key: $"o2_set{key}", values);
            }

            this.ListForWriting = Seed.BuildReasons(totalKeys: 1, totalReasons: 2, totalRemovedEntities: 4);
        }

        #region option 1 Each redis key has a delimited text, sequences of {"REASON IDENTIFIER":["REMOVED ID","REMOVED ID", ...]}
        //[Benchmark]
        //public void O1_Set_Delimited()
        //{
        //    foreach (var item in this.ListForWriting)
        //    {
        //        string key = $"o1_delimited:{item.GetKey()}";
        //        this.Cache.Set(key, item.RemovedEntitiesByReason);
        //    }
        //}

        //[Benchmark]
        //public void O1_Get_Delimited()
        //{
        //    foreach (var item in this.ListForReading)
        //    {
        //        string key = $"o1_delimited:{item.GetKey()}";
        //        IDictionary<string, IEnumerable<string>> result = this.Cache.Get<IDictionary<string, IEnumerable<string>>>(key);
        //    }
        //}

        #endregion

        #region option 2 Each redis key has a json string representing object (current routing way) 
        //we serialize for cache the minimum required info IDictionary<string,IEnumerable<string>> removed entities by Reason

        //[Benchmark]
        //public void O2_Set_JsonString()
        //{
        //    foreach (var item in ListForWriting)
        //    {
        //        string key = $"o2_json{item.GetKey()}";
        //        this.Cache.Set(key, JsonConvert.SerializeObject(item.RemovedEntitiesByReason));
        //    }
        //}

        //[Benchmark]
        //public void O2_Get_JsonString()
        //{
        //    foreach (var item in ListForReading)
        //    {
        //        string key = $"o2_json{item.GetKey()}";
        //        string result = this.Cache.Get<string>(key);
        //        result = System.Text.RegularExpressions.Regex.Unescape(result);
        //        IDictionary<string, IEnumerable<string>> reasons = JsonConvert.DeserializeObject<IDictionary<string, IEnumerable<string>>>(result);
        //    }
        //}

        #endregion

        #region option2 using jil json package
        //[Benchmark]
        //public void O2_Set_JilJsonString()
        //{
        //    foreach (var item in this.ListForWriting)
        //    {
        //        string key = $"o2_jiljson{item.GetKey()}";
        //        this.Cache.Set(key, Jil.JSON.Serialize(item.RemovedEntitiesByReason));
        //    }
        //}

        //[Benchmark]
        //public void O2_Get_JilJsonString()
        //{
        //    foreach (var item in this.ListForReading)
        //    {
        //        string key = $"o2_jiljson{item.GetKey()}";
        //        string result = this.Cache.Get<string>(key);
        //        result = System.Text.RegularExpressions.Regex.Unescape(result);
        //        IDictionary<string, IEnumerable<string>> reasons = Jil.JSON.Deserialize<IDictionary<string, IEnumerable<string>>>(result);
        //    }
        //}
        #endregion

        #region option 3 
        //redis has no option to set expiration for hashes :-O
        //https://stackoverflow.com/questions/16545321/how-to-expire-the-hset-child-key-in-redis

        [Benchmark]
        public void O3_Set_Hash()
        {
            foreach (var item in this.ListForWriting)
            {
                string key = $"o3_hash{item.GetKey()}";
                IDictionary<string, string> entries = new Dictionary<string, string>();
                foreach (var removedEntityByReason in item.RemovedEntitiesByReason)
                {
                    //add fields for Reason and RemovedEntityIds
                    entries.Add(removedEntityByReason.Key, string.Join(",", removedEntityByReason.Value));
                }
                this.Cache.HashSet(key, entries);
            }
        }

        [Benchmark]
        public void O3_Get_Hash()
        {
            foreach (var item in this.ListForReading)
            {
                string key = $"o3_hash{item.GetKey()}";
                // field and comma delimited entity ids
                IDictionary<string, string> values = this.Cache.HashGet(key);

                var reasons = new Dictionary<string, IEnumerable<string>>();
                foreach (var kvp in values)
                {
                    reasons.Add(kvp.Key, kvp.Value.Split(','));
                }
            }
        }
        #endregion

        #region private methods


        #endregion
    }
}