using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StrapiRestClient.Blocks.DataModels;
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
            services.AddStrapiRestClient(configuration, blocks =>
            {
                blocks.RegisterBlock<RichTextBlockComponent>("shared.rich-text")
                  .RegisterBlock<QuoteBlockComponent>("shared.quote")
                  .RegisterBlock<MediaBlockComponent>("shared.media")
                  .RegisterBlock<SliderBlockComponent>("shared.slider");
            });
        }
    }
}
