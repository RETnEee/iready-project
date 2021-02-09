using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace iready.lib.AspNetCore.StaticFile
{
    public static class ConfigurationExtensions
    {
        public static IHostBuilder AddConfiguration(this IHostBuilder builder)
        {
            builder
                .ConfigureAppConfiguration((context, builder)=>
                {
                    builder
                        .SetBasePath(context.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: false)
                        .Build();
                });
            return builder;
        }
    }
}