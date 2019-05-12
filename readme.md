# RedisDataTypesBenchmark

```
BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17763.475 (1809/October2018Update/Redstone5)
Intel Core i7-3632QM CPU 2.20GHz (Ivy Bridge), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100-preview3-010431
  [Host]     : .NET Core 2.0.9 (CoreCLR 4.6.26614.01, CoreFX 4.6.26614.01), 64bit RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 2.0.9 (CoreCLR 4.6.26614.01, CoreFX 4.6.26614.01), 64bit RyuJIT
```

## Results for writing in cache

In REDIS host, there are some peaks of CPU usage with an average of 64% while writing with Hash and Sets.

Assuming we have 63 requests / min, 2 reasons, 4 removed entities:

### with ListForWriting = Seed.BuildReasons(totalKeys: 252, totalReasons: 2, totalRemovedEntities: 4);
```
|               Method |       Mean |     Error |     StdDev | Rank |    Gen 0 |   Gen 1 | Gen 2 |  Allocated |
|--------------------- |-----------:|----------:|-----------:|-----:|---------:|--------:|------:|-----------:|
|          O3_Set_Hash |   6.573 ms | 0.1367 ms |  0.3987 ms |    1 | 187.5000 | 31.2500 |     - |  704.79 KB |
|          O4_Set_Sets |   8.119 ms | 0.1620 ms |  0.4380 ms |    2 |  93.7500 | 23.4375 |     - |  587.93 KB |
|     O1_Set_Delimited | 258.493 ms | 4.9333 ms |  4.3732 ms |    3 |        - |       - |     - |  623.72 KB |
|    O2_Set_JsonString | 263.103 ms | 5.2556 ms |  8.1824 ms |    3 |        - |       - |     - | 1273.32 KB |
| O2_Set_JilJsonString | 273.843 ms | 5.6351 ms | 11.6375 ms |    4 |        - |       - |     - | 1131.08 KB |
```

### with ListForWriting = Seed.BuildReasons(totalKeys: 252, totalReasons: 2, totalRemovedEntities: 2000);
```
|               Method |       Mean |    Error |   StdDev | Rank |      Gen 0 |     Gen 1 |     Gen 2 | Allocated |
|--------------------- |-----------:|---------:|---------:|-----:|-----------:|----------:|----------:|----------:|
|          O3_Set_Hash |   473.2 ms | 10.30 ms | 30.05 ms |    1 | 16000.0000 | 2000.0000 |         - |  89.27 MB |
|          O4_Set_Sets |   511.7 ms | 10.12 ms | 27.89 ms |    2 | 20000.0000 | 4000.0000 |         - | 108.65 MB |
|     O1_Set_Delimited |   968.9 ms | 18.06 ms | 16.90 ms |    3 | 49000.0000 |         - |         - | 147.89 MB |
| O2_Set_JilJsonString | 1,514.0 ms | 28.71 ms | 29.49 ms |    4 | 38000.0000 | 7000.0000 | 3000.0000 | 153.78 MB |
|    O2_Set_JsonString | 1,531.3 ms | 19.26 ms | 18.02 ms |    4 | 38000.0000 | 7000.0000 | 3000.0000 | 153.91 MB |
```


