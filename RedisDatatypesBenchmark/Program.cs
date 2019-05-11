namespace RedisDatatypesBenchmark
{
    using BenchmarkDotNet.Running;

    using Infrastructure.CrossCutting.Cache;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class Program
    {
        private static ICacheStore cacheStore;
        
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting application");

            //string key = "o1_delimited::RequestId_c5434ee9-e1a1-4dbc-a358-0bad1a68adb7:ProductId_233:VariantId_641799f9-39a2-4651-aba3-1c2b0f11b258";
            //var cache = CacheHelper.GetCacheStore();
            //string kct = cache.StringGet(key);

            //string s = "key2c7c83de-6c2f-4ce4-868a-60b2a9e54c50:163,173,203,230|key9aebf3a3-2183-4e1a-9a04-50c297486b6e:163,173,203,230";
            //IEnumerable<string> rows = s.Split("|");
            //var result = new Dictionary<string, IEnumerable<string>>();
            //foreach (string row in rows)
            //{
            //    string[] v = row.Split(":");
            //    result.Add(v[0], v[1].Split(","));
            //}

            var summary = BenchmarkRunner.Run<RedisBenchmarks>();

            Console.WriteLine("The end");

            Console.ReadLine();
        }
    }
}
