using Microsoft.Extensions.Logging;
using StrapiRestClient.Extensions;
using StrapiRestClient.Models;
using StrapiRestClient.Request;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace StrapiRestClient.RestClient
{
    /// <summary>
    /// Represents a client for interacting with a Strapi API.
    /// </summary>
    public class StrapiRestClient : IStrapiRestClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StrapiRestClient> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="StrapiRestClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client to use for requests.</param>
        /// <param name="logger">The logger to use for logging.</param>
        public StrapiRestClient(HttpClient httpClient, ILogger<StrapiRestClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<StrapiResponse<T>> ExecuteAsync<T>(StrapiRequest request, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                using var response = await SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var test = await response.Content.ReadAsStringAsync();

                    var data = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
                    return new StrapiResponse<T> { Data = data, StatusCode = response.StatusCode };
                }
                else
                {
                    var error = await response.Content.ReadFromJsonAsync<StrapiErrorResponse>(JsonOptions, cancellationToken);
                    return new StrapiResponse<T> { Error = error?.Error, StatusCode = response.StatusCode };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request to Strapi failed: {Endpoint}", request.ContentType);
                return new StrapiResponse<T> { Error = new StrapiError { Message = ex.Message }, StatusCode = System.Net.HttpStatusCode.InternalServerError };
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse JSON response from Strapi at {Endpoint}", request.ContentType);
                return new StrapiResponse<T> { Error = new StrapiError { Message = ex.Message }, StatusCode = System.Net.HttpStatusCode.InternalServerError };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during Strapi request to {Endpoint}", request.ContentType);
                return new StrapiResponse<T> { Error = new StrapiError { Message = ex.Message }, StatusCode = System.Net.HttpStatusCode.InternalServerError };
            }
        }

        private async Task<HttpResponseMessage> SendAsync(StrapiRequest request, CancellationToken cancellationToken)
        {
            var url = UrlBuilder.Create(_httpClient.BaseAddress.ToString(), request);
            _logger.LogDebug("Making request to Strapi: {Url}", url);

            var httpRequest = new HttpRequestMessage(new HttpMethod(request.Method.ToString()), url);

            if (request.Body != null)
            {
                httpRequest.Content = JsonContent.Create(request.Body, options: JsonOptions);
            }

            return await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        }
    }
}
