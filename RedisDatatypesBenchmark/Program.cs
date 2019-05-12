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

            var summary = BenchmarkRunner.Run<RedisBenchmarks>();

            Console.WriteLine("The end");

            Console.ReadLine();
        }

    }
}
