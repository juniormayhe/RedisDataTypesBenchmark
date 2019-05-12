namespace Infrastructure.CrossCutting.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICacheStore
    {
        string StringGet(string key);

        void StringSet(string key, string value, TimeSpan? expiredIn = null);

        T GetJson<T>(string key);

        void JsonSet<T>(string key, T value, TimeSpan? expiredIn = null);

        void Remove(string key);

        Task<string> StringGetAsync(string key);

        Task SetAsync(string key, string value, TimeSpan? expiredIn = null);

        Task<T> GetJsonAsync<T>(string key);

        Task SetJsonAsync<T>(string key, T value, TimeSpan? expiredIn = null);

        Task RemoveAsync(string key);

        void HashSet(string key, IDictionary<string, string> values);

        IDictionary<string, string> HashGet(string key);

        Task<IDictionary<string,string>> HashGetAsync(string key);

        void SetAddAll(string key, IEnumerable<string> values);
        Task<IEnumerable<string>> SetGetAsync(string key);
        IEnumerable<string> SetGet(string key);
    }
}
