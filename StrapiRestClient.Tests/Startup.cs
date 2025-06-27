using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StrapiRestClient.Extensions;
using StrapiRestClient.RestClient;
using System.IO;

namespace StrapiRestClient.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddStrapiConnect(configuration);
        }
    }
}
