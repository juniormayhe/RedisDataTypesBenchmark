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

## Narrowing observation of using hashes

Evaluated Hash value arrangements:

RequestIdInKey
```
 Key - RequestId_GUID
 |__ Field - ProductId:INT_VariantId:GUID_Reason:STRING, Value - Entities
 |__ Field - ProductId:INT_VariantId:GUID_Reason:STRING, Value - Entities
```

RequestIdAndProductIdInKey
```
 Key - RequestId_GUID:ProductId:INT
 |__ Field - VariantId:GUID_Reason:STRING, Entities, Value - Entities
 |__ Field - VariantId:GUID_Reason:STRING, Entities, Value - Entities
```
AllFieldsInKey
```
 Key - RequestId_GUID:ProductId_INT:VariantId_GUID
 |__ Field - Reason:STRING, Entities, Value - Entities
 |__ Field - Reason:STRING, Entities, Value - Entities
```

Where Entities are semi colon delimited string

### 5000 records on local REDIS

```
|                               Method |     Mean |    Error |    StdDev |   Median | Rank |     Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|------------------------------------- |---------:|---------:|----------:|---------:|-----:|----------:|---------:|---------:|----------:|
|            O3_SetHash_AllFieldsInKey | 77.59 ms | 2.288 ms |  6.565 ms | 76.01 ms |    1 | 2500.0000 | 625.0000 | 125.0000 |     14 MB |
| O2_SetHash_RequestIdAndProductdInKey | 90.45 ms | 4.070 ms | 11.413 ms | 87.26 ms |    2 | 2428.5714 | 571.4286 |        - |  14.82 MB |
|            O1_SetHash_RequestIdInKey | 92.71 ms | 2.483 ms |  7.043 ms | 92.02 ms |    3 | 2800.0000 | 600.0000 |        - |  16.92 MB |

|                                 Method |    Mean |    Error |   StdDev | Rank |     Gen 0 |     Gen 1 | Gen 2 | Allocated |
|--------------------------------------- |--------:|---------:|---------:|-----:|----------:|----------:|------:|----------:|
|             O1_ReadHash_RequestIdInKey | 4.203 s | 0.0828 s | 0.1472 s |    1 | 3000.0000 | 1000.0000 |     - |  10.12 MB |
|             O3_ReadHash_AllFieldsInKey | 4.236 s | 0.0845 s | 0.2240 s |    1 | 2000.0000 | 1000.0000 |     - |  10.21 MB |
| O2_ReadHash_RequestIdAndProductIdInKey | 4.263 s | 0.0827 s | 0.1312 s |    1 | 2000.0000 | 1000.0000 |     - |   9.59 MB |
```

### 5000 records with remote REDIS (with network latency)
```
|                               Method |     Mean |     Error |   StdDev | Rank |     Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|------------------------------------- |---------:|----------:|---------:|-----:|----------:|---------:|---------:|----------:|
|            O3_SetHash_AllFieldsInKey | 48.60 ms | 0.9857 ms | 2.764 ms |    1 | 2300.0000 | 900.0000 |        - |  13.98 MB |
| O2_SetHash_RequestIdAndProductdInKey | 56.03 ms | 1.3944 ms | 4.023 ms |    2 | 2625.0000 | 750.0000 | 125.0000 |  14.81 MB |
|            O1_SetHash_RequestIdInKey | 59.35 ms | 1.2333 ms | 2.628 ms |    3 | 2888.8889 | 888.8889 | 111.1111 |  16.91 MB |

|                                 Method |    Mean |    Error |   StdDev | Rank |     Gen 0 |     Gen 1 | Gen 2 | Allocated |
|--------------------------------------- |--------:|---------:|---------:|-----:|----------:|----------:|------:|----------:|
|             O1_ReadHash_RequestIdInKey | 6.589 s | 0.2001 s | 0.5837 s |    1 | 3000.0000 | 1000.0000 |     - |  10.12 MB |
|             O3_ReadHash_AllFieldsInKey | 6.642 s | 0.2005 s | 0.5849 s |    1 | 2000.0000 | 1000.0000 |     - |  10.21 MB |
| O2_ReadHash_RequestIdAndProductIdInKey | 7.039 s | 0.2700 s | 0.7960 s |    2 | 2000.0000 | 1000.0000 |     - |   9.59 MB |
```

## Narrowing observation of using hashes

Evaluated Set value arrangements:

RequestIdInKey
```
 Key - RequestId_GUID
 |__ Value - ProductId:INT|VariantId:GUID|Reason:STRING:Entities
 |__ Value - ProductId:INT|VariantId:GUID|Reason:STRING:Entities
```
where both Reason is a string and Entities is a semi colon delimited string
 

RequestIdAndProductdInKey
```
 Key - RequestId_GUID:ProductId:INT
 |__ Value - VariantId:GUID|Reason:STRING:Entities
 |__ Value - VariantId:GUID|Reason:STRING:Entities
```
where both Reason is a string and Entities is a semi colon delimited string


AllFieldsInKey
```
  Key - RequestId_GUID:ProductId_INT:VariantId_GUID
 |__ Value - Reason:Entities
 |__ Value - Reason:Entities
```
where both Reason is a string and Entities is a semi colon delimited string

### 5000 records on local REDIS

```
|                                 Method |     Mean |    Error |   StdDev | Rank |     Gen 0 |    Gen 1 | Gen 2 | Allocated |
|--------------------------------------- |---------:|---------:|---------:|-----:|----------:|---------:|------:|----------:|
|            O3_SetAddAll_AllFieldsInKey | 91.99 ms | 1.818 ms | 2.299 ms |    1 | 2000.0000 | 500.0000 |     - |   11.8 MB |
|            O1_SetAddAll_RequestIdInKey | 95.30 ms | 1.827 ms | 2.501 ms |    2 | 2600.0000 | 600.0000 |     - |  14.64 MB |
| O2_SetAddAll_RequestIdAndProductdInKey | 95.31 ms | 1.846 ms | 2.929 ms |    2 | 2333.3333 | 666.6667 |     - |  13.83 MB |

|                                Method |    Mean |    Error |   StdDev | Rank |     Gen 0 |     Gen 1 | Gen 2 | Allocated |
|-------------------------------------- |--------:|---------:|---------:|-----:|----------:|----------:|------:|----------:|
|             O1_ReadSet_RequestIdInKey | 4.199 s | 0.0759 s | 0.1309 s |    1 | 3000.0000 | 1000.0000 |     - |  18.43 MB |
| O2_ReadSet_RequestIdAndProductIdInKey | 4.246 s | 0.0846 s | 0.0869 s |    1 | 3000.0000 | 1000.0000 |     - |  17.62 MB |
|             O3_ReadSet_AllFieldsInKey | 4.256 s | 0.0838 s | 0.0897 s |    1 | 3000.0000 | 1000.0000 |     - |  15.81 MB |
```

### 5000 records with remote REDIS (with network latency)
```
|                                 Method |     Mean |    Error |   StdDev | Rank |     Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|--------------------------------------- |---------:|---------:|---------:|-----:|----------:|---------:|---------:|----------:|
|            O3_SetAddAll_AllFieldsInKey | 56.94 ms | 1.764 ms | 5.060 ms |    1 | 2000.0000 | 555.5556 |        - |  11.77 MB |
| O2_SetAddAll_RequestIdAndProductdInKey | 67.74 ms | 2.321 ms | 6.585 ms |    2 | 2444.4444 | 666.6667 | 111.1111 |   13.8 MB |
|            O1_SetAddAll_RequestIdInKey | 70.23 ms | 2.420 ms | 7.020 ms |    3 | 2555.5556 | 777.7778 | 111.1111 |   14.6 MB |

|                                Method |    Mean |    Error |   StdDev | Rank |     Gen 0 |     Gen 1 | Gen 2 | Allocated |
|-------------------------------------- |--------:|---------:|---------:|-----:|----------:|----------:|------:|----------:|
|             O3_ReadSet_AllFieldsInKey | 6.773 s | 0.1866 s | 0.5109 s |    1 | 4000.0000 | 1000.0000 |     - |  15.81 MB |
| O2_ReadSet_RequestIdAndProductIdInKey | 6.919 s | 0.2076 s | 0.5991 s |    1 | 4000.0000 | 1000.0000 |     - |  17.61 MB |
|             O1_ReadSet_RequestIdInKey | 7.523 s | 0.2643 s | 0.7411 s |    2 | 4000.0000 | 1000.0000 |     - |  18.43 MB |
```

## Conclusions

- Reading cache have similar performance and datatype won't impact so much, but the best memory allocations are Hashes and Sets.
- Saving cache as Hash and Sets datatypes seems faster than other approaches. 
- Memory allocations are lower with Sets, Delimited Text and Hashes.

## Useful links
[REDIS memory footprint](https://redis.io/topics/faq)
[Expensive commands in REDIS](https://docs.microsoft.com/en-us/azure/azure-cache-for-redis/cache-how-to-troubleshoot#expensive-commands)
[Rules about REDIS keys](https://redis.io/topics/data-types-intro)
[Benchmarking REDIS server](https://redis.io/topics/benchmarks)
