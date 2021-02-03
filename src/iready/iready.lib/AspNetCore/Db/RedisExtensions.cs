namespace iready.lib.AspNetCore.Db
{
    public static class RedisExtensions
    {
        public static void AddRedis(this IServiceCollection services)
        {
            if (!ConnectionHelper.Redis.Any())
                return;
            var mamCache = RedisProvider.Build(ConnectionHelper.Redis);
            services.AddSingleton<IMAMDistributedCache>(mamCache);
        }
    }
}