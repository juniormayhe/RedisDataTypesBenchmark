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
    public class RedisBenchmarks
    {
        public IEnumerable<RoutingLog> ListForReading { get; set; }
        public IEnumerable<RoutingLog> ListForWriting { get; set; }
        public ICacheStore Cache { get; set; }

        [GlobalSetup]
        public void InitialData()
        {
            this.Cache = CacheHelper.GetCacheStore();

            //3800 req/hour ~ 63 req/min, 
            //each request with 2 bag items may use cache twice 63 x 2 = 126 keys x 2 services = 252 keys
            //reasons per product has a maximum of 17

            // warmup cache for further reading
            this.ListForReading = Seed.BuildReasons(totalKeys: 252, totalReasons: 2, totalRemovedEntities: 4);
            this.WarmUpCacheForReading();

            this.ListForWriting = Seed.BuildReasons(totalKeys: 252, totalReasons: 2, totalRemovedEntities: 4);
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

        [Benchmark]
        public void O1_Get_Delimited()
        {
            foreach (var item in this.ListForReading)
            {
                string key = $"o1_delimited{item.GetKey()}";
                IEnumerable<string> rows = this.Cache.StringGet(key).Split("|");
                var result = new Dictionary<string, IEnumerable<string>>();
                foreach (string row in rows)
                {
                    string[] v = row.Split(":");
                    result.Add(v[0], v[1].Split(","));
                }
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

        [Benchmark]
        public void O2_Get_JsonString()
        {
            foreach (var item in ListForReading)
            {
                string key = $"o2_json{item.GetKey()}";
                string result = this.Cache.GetJson<string>(key);
                result = System.Text.RegularExpressions.Regex.Unescape(result);
                IDictionary<string, IEnumerable<string>> reasons = JsonConvert.DeserializeObject<IDictionary<string, IEnumerable<string>>>(result);
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

        [Benchmark]
        public void O2_Get_JilJsonString()
        {
            foreach (var item in this.ListForReading)
            {
                string key = $"o2_jiljson{item.GetKey()}";
                string result = this.Cache.GetJson<string>(key);
                result = System.Text.RegularExpressions.Regex.Unescape(result);
                IDictionary<string, IEnumerable<string>> reasons = Jil.JSON.Deserialize<IDictionary<string, IEnumerable<string>>>(result);
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

        [Benchmark]
        public void O2_Get_NetJsonString()
        {
            foreach (var item in this.ListForReading)
            {
                string key = $"o2_netjson{item.GetKey()}";
                string result = this.Cache.GetJson<string>(key);
                result = System.Text.RegularExpressions.Regex.Unescape(result);
                IDictionary<string, IEnumerable<string>> reasons = NetJSON.Deserialize<IDictionary<string, IEnumerable<string>>>(result);
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

        [Benchmark]
        public void O4_Get_Sets()
        {
            foreach (var item in this.ListForReading)
            {
                string key = $"o4_set{item.GetKey()}";

                IEnumerable<string> rows = this.Cache.SetGet(key);
                var result = new Dictionary<string, IEnumerable<string>>();
                foreach (string row in rows)
                {
                    string[] v = row.Split(":");
                    result.Add(v[0], v[1].Split(","));
                }
            }
        }
        #endregion

        #region private methods
        private void WarmUpCacheForReading()
        {
            foreach (var item in this.ListForReading)
            {
                string key = item.GetKey();

                var hashEntries = new Dictionary<string, string>();
                var plainTexts = new List<string>();
                foreach (var kvp in item.RemovedEntitiesByReason)
                {
                    //add fields for Reason and RemovedEntityIds
                    hashEntries.Add(kvp.Key, string.Join(",", kvp.Value));
                    // delimited
                    plainTexts.Add($"{kvp.Key}:{string.Join(",", kvp.Value)}");
                    this.Cache.SetAddAll(key: $"o4_set{key}", plainTexts);
                }
                this.Cache.StringSet(key: $"o1_delimited{key}", string.Join("|", plainTexts));
                this.Cache.HashSet(key: $"o3_hash{key}", hashEntries);

                //jsons
                this.Cache.JsonSet(key: $"o2_json{key}", JsonConvert.SerializeObject(item.RemovedEntitiesByReason));
                this.Cache.JsonSet(key: $"o2_jiljson{key}", Jil.JSON.Serialize(item.RemovedEntitiesByReason));
                this.Cache.JsonSet(key: $"o2_netjson{key}", NetJSON.Serialize(item.RemovedEntitiesByReason));
            }
        }

        #endregion
    }
}