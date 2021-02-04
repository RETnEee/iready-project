using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace iready.lib.AspNetCore.Db
{
    public static class DbExtensions
    {
        public static void AddDbService(this IServiceCollection services)
        {
            services.AddRedis();
        }
    }
}