# RedisDataTypesBenchmark

## Results for running all togheter

In REDIS host, there are some peaks of CPU usage with an average of 64% while writing with Hash and Sets.

Assuming we have 63 requests / min, 2 reasons, 4 removed entities:

### with 252 totalKeys with 2 total Reasons and each reason 4 entity ids
```
BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17763.475 (1809/October2018Update/Redstone5)
Intel Core i7-3632QM CPU 2.20GHz (Ivy Bridge), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100-preview3-010431
  [Host]     : .NET Core 2.0.9 (CoreCLR 4.6.26614.01, CoreFX 4.6.26614.01), 64bit RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 2.0.9 (CoreCLR 4.6.26614.01, CoreFX 4.6.26614.01), 64bit RyuJIT


|               Method |       Mean |     Error |    StdDev | Rank |    Gen 0 |   Gen 1 | Gen 2 |  Allocated |
|--------------------- |-----------:|----------:|----------:|-----:|---------:|--------:|------:|-----------:|
|          O3_Set_Hash |   6.653 ms | 0.1300 ms | 0.3750 ms |    1 | 187.5000 | 46.8750 |     - |  704.92 KB |
|          O4_Set_Sets |   8.383 ms | 0.2161 ms | 0.6201 ms |    2 |  93.7500 | 23.4375 |     - |  588.01 KB |
|          O4_Get_Sets | 260.697 ms | 5.1407 ms | 6.1197 ms |    3 |        - |       - |     - |  771.46 KB |
|     O1_Get_Delimited | 261.739 ms | 5.2203 ms | 6.9689 ms |    3 |        - |       - |     - |  972.13 KB |
| O2_Set_NetJsonString | 261.943 ms | 5.1786 ms | 7.4271 ms |    3 |        - |       - |     - |  916.58 KB |
| O2_Get_NetJsonString | 261.991 ms | 5.0568 ms | 4.9664 ms |    3 | 500.0000 |       - |     - | 1876.72 KB |
| O2_Get_JilJsonString | 262.065 ms | 5.1454 ms | 7.0431 ms |    3 |        - |       - |     - | 1969.38 KB |
|     O1_Set_Delimited | 262.341 ms | 4.5285 ms | 4.2359 ms |    3 |        - |       - |     - |  623.59 KB |
|    O2_Set_JsonString | 264.668 ms | 5.2340 ms | 6.6193 ms |    3 |        - |       - |     - | 1273.27 KB |
|          O3_Get_Hash | 264.929 ms | 5.1826 ms | 8.5152 ms |    3 |        - |       - |     - |   603.3 KB |
|    O2_Get_JsonString | 265.101 ms | 5.2027 ms | 7.2934 ms |    3 | 500.0000 |       - |     - |  2550.5 KB |
| O2_Set_JilJsonString | 265.379 ms | 5.2339 ms | 6.4278 ms |    3 |        - |       - |     - | 1130.64 KB |
```

## Results for running only gets

Reading cache for 252 keys have similar performance and datatype won't impact so much, but the best memory allocations are Hashes and Sets:

```
|               Method |     Mean |    Error |   StdDev | Rank |    Gen 0 | Gen 1 | Gen 2 |  Allocated |
|--------------------- |---------:|---------:|---------:|-----:|---------:|------:|------:|-----------:|
|          O4_Get_Sets | 253.0 ms | 3.891 ms | 3.640 ms |    1 |        - |     - |     - |  771.64 KB |
|     O1_Get_Delimited | 253.9 ms | 4.869 ms | 5.607 ms |    1 |        - |     - |     - |   972.5 KB |
|          O3_Get_Hash | 254.3 ms | 4.995 ms | 4.906 ms |    1 |        - |     - |     - |  603.16 KB |
| O2_Get_JilJsonString | 257.3 ms | 5.085 ms | 9.298 ms |    1 |        - |     - |     - | 1969.45 KB |
| O2_Get_NetJsonString | 260.0 ms | 5.182 ms | 6.554 ms |    1 | 500.0000 |     - |     - |  1876.8 KB |
|    O2_Get_JsonString | 260.6 ms | 5.025 ms | 6.172 ms |    1 | 500.0000 |     - |     - | 2550.53 KB |
```

## Results for running only sets

Saving cache for 252 keys as Hash and Sets datatypes seem faster than other approaches. Memory allocations are better with Sets and Delimited Text and Hashes:

```
|               Method |       Mean |     Error |    StdDev | Rank |    Gen 0 |   Gen 1 | Gen 2 |  Allocated |
|--------------------- |-----------:|----------:|----------:|-----:|---------:|--------:|------:|-----------:|
|          O3_Set_Hash |   6.392 ms | 0.1277 ms | 0.3582 ms |    1 | 171.8750 | 46.8750 |     - |  704.64 KB |
|          O4_Set_Sets |   7.920 ms | 0.1574 ms | 0.3710 ms |    2 |  93.7500 | 23.4375 |     - |  587.72 KB |
| O2_Set_NetJsonString | 253.513 ms | 4.9173 ms | 5.2615 ms |    3 |        - |       - |     - |  916.72 KB |
|     O1_Set_Delimited | 257.307 ms | 4.9737 ms | 5.7277 ms |    3 |        - |       - |     - |  623.99 KB |
| O2_Set_JilJsonString | 260.235 ms | 5.2489 ms | 8.9131 ms |    3 |        - |       - |     - | 1130.32 KB |
|    O2_Set_JsonString | 260.661 ms | 5.0551 ms | 6.5731 ms |    3 |        - |       - |     - | 1273.13 KB |
```