# RedisDataTypesBenchmark

## Results for running all datatypes tests

In REDIS host, there are some peaks of CPU usage with an average of 64% while writing with Hash and Sets.

Cached data schema:

```
\ key
  |__ field 1 : value 
  |__ field 2 : value
```
- key is regular string
- field is regular string
- value is either delimited text or JSON (4 random numbers)

### 1000 records

```
BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17763.475 (1809/October2018Update/Redstone5)
Intel Core i7-3632QM CPU 2.20GHz (Ivy Bridge), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100-preview3-010431
  [Host]     : .NET Core 2.0.9 (CoreCLR 4.6.26614.01, CoreFX 4.6.26614.01), 64bit RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 2.0.9 (CoreCLR 4.6.26614.01, CoreFX 4.6.26614.01), 64bit RyuJIT

|               Method |        Mean |      Error |    StdDev | Rank |     Gen 0 |    Gen 1 | Gen 2 | Allocated |
|--------------------- |------------:|-----------:|----------:|-----:|----------:|---------:|------:|----------:|
|          O3_Set_Hash |    25.29 ms |  0.5044 ms |  1.311 ms |    1 |  718.7500 | 156.2500 |     - |   2.75 MB | <-- lower alloc
|          O4_Set_Sets |    31.25 ms |  0.6188 ms |  1.694 ms |    2 |  375.0000 |  93.7500 |     - |    2.3 MB | <-- lower alloc
| O2_Set_NetJsonString | 1,005.82 ms | 13.4893 ms | 11.958 ms |    3 | 1000.0000 |        - |     - |   3.57 MB |
|     O1_Set_Delimited | 1,006.11 ms | 10.8458 ms | 10.145 ms |    3 |         - |        - |     - |   2.44 MB | <-- lower alloc
| O2_Set_JilJsonString | 1,019.29 ms | 13.0463 ms | 10.894 ms |    3 | 1000.0000 |        - |     - |    4.4 MB |
|    O2_Set_JsonString | 1,029.41 ms | 19.6352 ms | 19.284 ms |    3 | 1000.0000 |        - |     - |   4.96 MB |

|               Method |    Mean |    Error |   StdDev |  Median | Rank |     Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------------------- |--------:|---------:|---------:|--------:|-----:|----------:|------:|------:|----------:|
|          O4_Get_Sets | 1.006 s | 0.0119 s | 0.0105 s | 1.002 s |    1 | 1000.0000 |     - |     - |   3.03 MB | <-- lower alloc
|          O3_Get_Hash | 1.014 s | 0.0190 s | 0.0169 s | 1.013 s |    1 |         - |     - |     - |   2.36 MB | <-- lower alloc
| O2_Get_NetJsonString | 1.016 s | 0.0135 s | 0.0126 s | 1.013 s |    1 | 2000.0000 |     - |     - |    7.3 MB |
| O2_Get_JilJsonString | 1.017 s | 0.0121 s | 0.0101 s | 1.021 s |    1 | 2000.0000 |     - |     - |   7.66 MB |
|     O1_Get_Delimited | 1.032 s | 0.0204 s | 0.0477 s | 1.013 s |    1 | 1000.0000 |     - |     - |   3.82 MB | <-- lower alloc
|    O2_Get_JsonString | 1.034 s | 0.0137 s | 0.0128 s | 1.032 s |    1 | 3000.0000 |     - |     - |   9.91 MB |
```

### 5000 records

```
|               Method |       Mean |     Error |    StdDev | Rank |     Gen 0 |    Gen 1 | Gen 2 | Allocated |
|--------------------- |-----------:|----------:|----------:|-----:|----------:|---------:|------:|----------:|
|          O3_Set_Hash |   130.1 ms |  2.674 ms |  7.884 ms |    1 | 4250.0000 | 500.0000 |     - |  13.76 MB | <-- lower alloc
|          O4_Set_Sets |   143.2 ms |  3.413 ms | 10.064 ms |    2 | 2000.0000 | 500.0000 |     - |  11.52 MB | <-- lower alloc
|     O1_Set_Delimited | 5,051.6 ms | 34.608 ms | 32.372 ms |    3 | 4000.0000 |        - |     - |  12.25 MB | <-- lower alloc
| O2_Set_NetJsonString | 5,088.1 ms | 67.037 ms | 62.707 ms |    3 | 5000.0000 |        - |     - |  17.89 MB |
| O2_Set_JilJsonString | 5,088.8 ms | 50.550 ms | 47.284 ms |    3 | 7000.0000 |        - |     - |  22.05 MB |
|    O2_Set_JsonString | 5,151.9 ms | 48.104 ms | 42.643 ms |    3 | 8000.0000 |        - |     - |  24.81 MB |

|               Method |    Mean |    Error |   StdDev | Rank |      Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------------------- |--------:|---------:|---------:|-----:|-----------:|------:|------:|----------:|
|     O1_Get_Delimited | 5.008 s | 0.0373 s | 0.0349 s |    1 |  6000.0000 |     - |     - |  19.19 MB | <-- lower alloc
|          O3_Get_Hash | 5.053 s | 0.0331 s | 0.0293 s |    1 |  4000.0000 |     - |     - |  11.85 MB | <-- lower alloc
| O2_Get_NetJsonString | 5.065 s | 0.0540 s | 0.0479 s |    1 | 12000.0000 |     - |     - |  36.53 MB |
| O2_Get_JilJsonString | 5.070 s | 0.0311 s | 0.0275 s |    1 | 13000.0000 |     - |     - |  38.32 MB |
|    O2_Get_JsonString | 5.107 s | 0.0528 s | 0.0494 s |    1 | 17000.0000 |     - |     - |  49.59 MB |
|          O4_Get_Sets | 5.118 s | 0.1012 s | 0.0994 s |    1 |  5000.0000 |     - |     - |   15.2 MB | <-- lower alloc
```

## Narrowing approach of using hashes

### 5000 records

```
|                     Method |      Mean |    Error |   StdDev | Rank |     Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|--------------------------- |----------:|---------:|---------:|-----:|----------:|---------:|---------:|----------:|
| O3_Set_Hash_AllFieldsInKey |  75.75 ms | 1.507 ms | 3.610 ms |    1 | 2428.5714 | 714.2857 | 142.8571 |  14.01 MB |
|                O3_Set_Hash | 100.41 ms | 2.566 ms | 7.443 ms |    2 | 2833.3333 | 666.6667 |        - |  16.93 MB |

|                     Method |    Mean |    Error |   StdDev | Rank |     Gen 0 |     Gen 1 | Gen 2 | Allocated |
|--------------------------- |--------:|---------:|---------:|-----:|----------:|----------:|------:|----------:|
| O3_Get_Hash_AllFieldsInKey | 4.485 s | 0.0994 s | 0.1021 s |    1 | 2000.0000 | 1000.0000 |     - |  10.21 MB |
|                O3_Get_Hash | 4.517 s | 0.0888 s | 0.1273 s |    1 | 3000.0000 | 1000.0000 |     - |  10.12 MB |
```
## Conclusions

- Reading cache have similar performance and datatype won't impact so much, but the best memory allocations are Hashes and Sets.
- Saving cache as Hash and Sets datatypes seem faster than other approaches. 
- Memory allocations are lower with Sets, Delimited Text and Hashes.
