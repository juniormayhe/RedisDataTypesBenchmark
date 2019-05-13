namespace RedisDatatypesBenchmark
{
    using BenchmarkDotNet.Running;

    using Infrastructure.CrossCutting.Cache;
    using RedisShared;
    using System;

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting application");

            var writeSummary = BenchmarkRunner.Run<RedisBenchmarksWrite>();

            var readSummary = BenchmarkRunner.Run<RedisBenchmarksRead>();

            // clean up data
            string[] patterns = { "o1_*", "o2_*", "o3_*", "o4_*" };
            CacheHelper.GetCacheStore().Truncate(patterns);

            Console.WriteLine("The end");

            Console.ReadLine();
        }
    }
}
