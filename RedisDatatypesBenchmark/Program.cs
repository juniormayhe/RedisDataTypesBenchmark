namespace RedisDatatypesBenchmark
{
    using BenchmarkDotNet.Running;

    using Infrastructure.CrossCutting.Cache;

    using System;

    public class Program
    {
        private static ICacheStore cacheStore;
        
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting application");

            var writeSummary = BenchmarkRunner.Run<RedisBenchmarksWrite>();

            var readSummary = BenchmarkRunner.Run<RedisBenchmarksRead>();

            Console.WriteLine("The end");

            Console.ReadLine();
        }

    }
}
