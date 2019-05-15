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

        Task StringSetAsync(string key, string value, TimeSpan? expiredIn = null);

        Task<T> GetJsonAsync<T>(string key);

        Task SetJsonAsync<T>(string key, T value, TimeSpan? expiredIn = null);

        Task RemoveAsync(string key);

        void Truncate(string[] patterns);

        #region hashes

        void HashSet(string key, IDictionary<string, string> values, TimeSpan? expiredIn = null);

        void HashSetAsync(string key, IDictionary<string, string> values, TimeSpan? expiredIn = null);

        IDictionary<string, string> HashGet(string key);

        Task<IDictionary<string, string>> HashGetAsync(string key);
        #endregion

        #region sets

        void SetAddAll(string key, IEnumerable<string> values, TimeSpan? expiredIn = null);
        void SetAddAllAsync(string key, IEnumerable<string> values, TimeSpan? expiredIn = null);
        Task<IEnumerable<string>> SetGetAllAsync(string key);
        IEnumerable<string> SetGetAll(string key);

        #endregion
    }
}
