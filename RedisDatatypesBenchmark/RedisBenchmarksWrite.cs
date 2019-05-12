namespace RedisDatatypesBenchmark
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Order;

    using Infrastructure.CrossCutting.Cache;

    using NetJSON;
    using Newtonsoft.Json;

    using System.Collections.Generic;

    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    [MemoryDiagnoser]
    public class RedisBenchmarksWrite
    {
        public IEnumerable<RoutingLog> ListForWriting { get; set; }
        public ICacheStore Cache { get; set; }

        [GlobalSetup]
        public void InitialData()
        {
            this.Cache = CacheHelper.GetCacheStore();

            this.ListForWriting = Seed.BuildReasons(totalKeys: 5000, totalReasons: 2, totalRemovedEntities: 4);
        }

        #region option 1 Each redis key has a delimited text, sequences of {"REASON IDENTIFIER":["REMOVED ID","REMOVED ID", ...]}
        [Benchmark]
        public void O1_Set_Delimited()
        {
            foreach (var item in this.ListForWriting)
            {
                string key = $"o1_delimited{item.GetKey()}";
                var values = new List<string>();
                foreach (var kvp in item.RemovedEntitiesByReason)
                {
                    values.Add($"{kvp.Key}:{string.Join(",", kvp.Value)}");
                }
                this.Cache.StringSet(key, string.Join("|", values));
            }
        }

        #endregion

        #region option 2 Newtonsoft - Each redis key has a json string representing object (current routing way) 
        //we serialize for cache the minimum required info IDictionary<string,IEnumerable<string>> removed entities by Reason

        [Benchmark]
        public void O2_Set_JsonString()
        {
            foreach (var item in ListForWriting)
            {
                string key = $"o2_json{item.GetKey()}";
                this.Cache.JsonSet<string>(key, JsonConvert.SerializeObject(item.RemovedEntitiesByReason));
            }
        }

        #endregion

        #region option 2 Jil - Each redis key has a json string representing object (current routing way) 
        [Benchmark]
        public void O2_Set_JilJsonString()
        {
            foreach (var item in this.ListForWriting)
            {
                string key = $"o2_jiljson{item.GetKey()}";
                this.Cache.JsonSet<string>(key, Jil.JSON.Serialize(item.RemovedEntitiesByReason));
            }
        }

        #endregion

        #region option 2 NetJSON - Each redis key has a json string representing object (current routing way) 
        [Benchmark]
        public void O2_Set_NetJsonString()
        {
            foreach (var item in this.ListForWriting)
            {
                string key = $"o2_netjson{item.GetKey()}";
                this.Cache.JsonSet<string>(key, NetJSON.Serialize(item.RemovedEntitiesByReason));
            }
        }

        #endregion

        #region option 3 Each redis key has a hash with field-value pairs of strings
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

        #endregion

        #region option 4 Each redis key has a set of unique plain text strings
        [Benchmark]
        public void O4_Set_Sets()
        {
            foreach (var item in this.ListForWriting)
            {
                string key = $"o4_set{item.GetKey()}";
                var values = new List<string>();
                foreach (var kvp in item.RemovedEntitiesByReason)
                {
                    values.Add($"{kvp.Key}:{string.Join(",", kvp.Value)}");
                }
                this.Cache.SetAddAll(key, values);
            }
        }

        #endregion
    }
}