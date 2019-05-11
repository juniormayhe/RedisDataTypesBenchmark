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

            var summary = BenchmarkRunner.Run<RedisBenchmarks>();

            Console.WriteLine("The end");

            Console.ReadLine();
        }
    }
}
