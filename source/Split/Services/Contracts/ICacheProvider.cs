using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Split.Services
{
    public interface ICacheProvider
    {
        T Get<T>([CallerMemberName] string key = null);
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory);
        Task<T> GetOrCreateAsync<T>(string key, TimeSpan expiration, Func<Task<T>> factory);
        T GetOrCreate<T>(string key, Func<T> factory);
        T GetOrCreate<T>(string key, TimeSpan expiration, Func<T> factory);
        void Set<T>(T value, DateTimeOffset? absoluteExpiry = null, [CallerMemberName] string key = null);
        void Remove(string key);
        void Clear();
    }
}
