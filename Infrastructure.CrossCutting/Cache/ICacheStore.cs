namespace Infrastructure.CrossCutting.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICacheStore
    {
        T Get<T>(string key);

        void Set<T>(string key, T value, TimeSpan? expiredIn = null);

        void Remove(string key);

        Task<T> GetAsync<T>(string key);

        Task SetAsync<T>(string key, T value, TimeSpan? expiredIn = null);

        Task RemoveAsync(string key);

        void HashSet(string key, IDictionary<string, string> values);

        IDictionary<string, string> HashGet(string key);

        Task<IDictionary<string,string>> HashGetAsync(string key);
    }
}
