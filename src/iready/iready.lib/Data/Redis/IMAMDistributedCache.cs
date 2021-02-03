namespace iready.lib.Data.Redis
{
    public interface IMAMDistributedCache
    {
        public string Site { get; set; }
        Task<byte[]> GetAsync(string key);
        Task<List<byte[]>> GetAsync(params string[] keys);
        void Refresh(string key);
        Task RefreshAsync(string key);
        void Remove(string key);
        Task RemoveAsync(string key);
        Task SetAsync(string key, byte[] value, TimeSpan? expiry);
        IMAMDistributedCache Copy();
    }
}