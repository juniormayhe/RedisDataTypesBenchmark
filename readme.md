# RedisDataTypesBenchmark

## Results for running all togheter

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

## 1000 records

```
|               Method |        Mean |      Error |    StdDev | Rank |     Gen 0 |    Gen 1 | Gen 2 | Allocated |
|--------------------- |------------:|-----------:|----------:|-----:|----------:|---------:|------:|----------:|
|          O3_Set_Hash |    25.29 ms |  0.5044 ms |  1.311 ms |    1 |  718.7500 | 156.2500 |     - |   2.75 MB |
|          O4_Set_Sets |    31.25 ms |  0.6188 ms |  1.694 ms |    2 |  375.0000 |  93.7500 |     - |    2.3 MB |
| O2_Set_NetJsonString | 1,005.82 ms | 13.4893 ms | 11.958 ms |    3 | 1000.0000 |        - |     - |   3.57 MB |
|     O1_Set_Delimited | 1,006.11 ms | 10.8458 ms | 10.145 ms |    3 |         - |        - |     - |   2.44 MB |
| O2_Set_JilJsonString | 1,019.29 ms | 13.0463 ms | 10.894 ms |    3 | 1000.0000 |        - |     - |    4.4 MB |
|    O2_Set_JsonString | 1,029.41 ms | 19.6352 ms | 19.284 ms |    3 | 1000.0000 |        - |     - |   4.96 MB |
```

```
|               Method |    Mean |    Error |   StdDev |  Median | Rank |     Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------------------- |--------:|---------:|---------:|--------:|-----:|----------:|------:|------:|----------:|
|          O4_Get_Sets | 1.006 s | 0.0119 s | 0.0105 s | 1.002 s |    1 | 1000.0000 |     - |     - |   3.03 MB |
|          O3_Get_Hash | 1.014 s | 0.0190 s | 0.0169 s | 1.013 s |    1 |         - |     - |     - |   2.36 MB |
| O2_Get_NetJsonString | 1.016 s | 0.0135 s | 0.0126 s | 1.013 s |    1 | 2000.0000 |     - |     - |    7.3 MB |
| O2_Get_JilJsonString | 1.017 s | 0.0121 s | 0.0101 s | 1.021 s |    1 | 2000.0000 |     - |     - |   7.66 MB |
|     O1_Get_Delimited | 1.032 s | 0.0204 s | 0.0477 s | 1.013 s |    1 | 1000.0000 |     - |     - |   3.82 MB |
|    O2_Get_JsonString | 1.034 s | 0.0137 s | 0.0128 s | 1.032 s |    1 | 3000.0000 |     - |     - |   9.91 MB |
```

## Conclusions

Reading cache for 252 keys have similar performance and datatype won't impact so much, but the best memory allocations are Hashes and Sets:

Saving cache for 252 keys as Hash and Sets datatypes seem faster than other approaches. Memory allocations are better with Sets and Delimited Text and Hashes:
