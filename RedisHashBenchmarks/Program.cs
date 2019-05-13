namespace RedisHashBenchmarks
{
    using BenchmarkDotNet.Running;

    using RedisShared;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting application");

            var writeSummary = BenchmarkRunner.Run<HashBenchmarksWrite>();
            var readSummary = BenchmarkRunner.Run<HashBenchmarksRead>();

            // clean up data
            // CacheHelper.GetCacheStore().Truncate(new []{ "o3_*"});

            Console.WriteLine("The end");

            Console.ReadLine();
        }
    }
}
