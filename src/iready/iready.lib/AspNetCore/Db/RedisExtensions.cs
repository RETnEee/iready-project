using System.Linq;
using iready.lib.Data.Redis;
using iready.lib.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace iready.lib.AspNetCore.Db
{
    public static class RedisExtensions
    {
        public static void AddRedis(this IServiceCollection services)
        {
            if (!ConnectionHelper.Redis.Any())
                return;
            var cache = RedisProvider.Build(ConnectionHelper.Redis);
            services.AddSingleton<IDistributedCache>(cache);
        }
    }
}