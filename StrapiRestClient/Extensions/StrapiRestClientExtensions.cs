using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using StrapiRestClient.Registration;


//using Microsoft.Extensions.Http;
using StrapiRestClient.RestClient;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;

namespace StrapiRestClient.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring the Strapi REST client.
    /// </summary>
    public static class StrapiRestClientExtensions
    {
        /// <summary>
        /// Adds and configures the Strapi REST client to the service collection with block configuration.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> containing application settings.</param>
        /// <param name="configureBlocks">Optional action to configure custom block types.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the "StrapiRestClient:BaseUrl" is not configured.</exception>
        public static IServiceCollection AddStrapiRestClient(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IBlockRegistration>? configureBlocks = null)
        {
            var baseUrl = configuration["StrapiRestClient:BaseUrl"];
            var apiKey = configuration["StrapiRestClient:ApiKey"];

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("StrapiRestClient BaseUrl is not configured.");
            }

            // Configure HTTP client
            services.AddHttpClient<IStrapiRestClient, RestClient.StrapiRestClient>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                if (!string.IsNullOrWhiteSpace(apiKey))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", apiKey);
                }
            })
            .AddPolicyHandler(GetRetryPolicy());

            // Configure block types if provided
            if (configureBlocks != null)
            {
                var registration = new BlockRegistration();
                configureBlocks(registration);
            }

            return services;
        }

        /// <summary>
        /// Configures a retry policy for transient HTTP errors.
        /// </summary>
        /// <returns>An <see cref="IAsyncPolicy{HttpResponseMessage}"/> for retrying HTTP requests.</returns>
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
                .Or<SocketException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => 
                        TimeSpan.FromSeconds(Math.Pow(2.0, retryAttempt))
                );
        }
        
    }
}