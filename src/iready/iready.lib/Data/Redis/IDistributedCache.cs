using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace iready.lib.Data.Redis
{
    public interface IDistributedCache
    {
        Task<byte[]> GetAsync(string key);
        Task<List<byte[]>> GetAsync(params string[] keys);
        void Refresh(string key);
        Task RefreshAsync(string key);
        void Remove(string key);
        Task RemoveAsync(string key);
        Task SetAsync(string key, byte[] value, TimeSpan? expiry);
        IDistributedCache Copy();
    }
}