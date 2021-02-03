namespace iready.lib.Data.Redis
{
    public class MAMRedisDistributedCache : IMAMDistributedCache
    {
        private int _databaseIndex;
        private ConfigurationOptions _config { get; set; }
        private ConnectionMultiplexer _connectionMultiplexer { get; set; }

        public void Set(int db, ConfigurationOptions config)
        {
            _databaseIndex = db;
            _config = config;
            _connectionMultiplexer = ConnectionMultiplexer.Connect(config);
        }

        private string site;
        public string Site
        {
            get
            {
                if (!string.IsNullOrEmpty(site))
                    return site;
                return UserHelper.GetCurrentUser().SiteCode;
            }
            set
            {
                site = value;
            }
        }

        public IDatabase Database
        {
            get
            {
                return _connectionMultiplexer.GetDatabase(_databaseIndex);
            }
        }

        public async Task<byte[]> GetAsync(string key)
        {
            key = MAMRedisHelper.FormatKey(Site, key);
            RedisValue value = await Database.StringGetAsync(key);
            return value;
        }

        public async Task<List<byte[]>> GetAsync(params string[] keys)
        {
            var redisKeys = new RedisKey[] { };
            for (int i = 0; i < keys.Length; i++)
            {
                redisKeys.SetValue(MAMRedisHelper.FormatKey(Site, keys[i]), i);
            }
            RedisValue[] values = await Database.StringGetAsync(redisKeys);

            var result = new List<byte[]>();
            foreach (var val in values)
            {
                result.Add(val);
            }

            return result;
        }

        public void Refresh(string key)
        {
            throw new NotImplementedException();
        }

        public Task RefreshAsync(string key)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            key = MAMRedisHelper.FormatKey(Site, key);
            Database.KeyDelete(key);
        }

        public Task RemoveAsync(string key)
        {
            key = MAMRedisHelper.FormatKey(Site, key);
            return Database.KeyDeleteAsync(key);
        }

        public Task SetAsync(string key, byte[] value, TimeSpan? expiry)
        {
            key = MAMRedisHelper.FormatKey(Site, key);
            return Database.StringSetAsync(key, value, expiry);
        }

        public IMAMDistributedCache Copy()
        {
            var copyRedis = new MAMRedisDistributedCache();
            copyRedis.Set(_databaseIndex, _config);
            return copyRedis;
        }
    }
}