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
        public async Task<StrapiResult<T>> ExecuteAsync<T>(StrapiRequest request, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                using var response = await SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);
                    
                    // Try to parse as a collection response first (most common case)
                    if (TryParseAsCollectionResponse<T>(jsonString, out var collectionResult))
                    {
                        return new StrapiResult<T>
                        {
                            Data = collectionResult.Data,
                            Meta = collectionResult.Meta,
                            IsSuccess = true,
                            StatusCode = response.StatusCode
                        };
                    }
                    
                    // Try to parse as a single item response
                    if (TryParseAsSingleItemResponse<T>(jsonString, out var singleResult))
                    {
                        return new StrapiResult<T>
                        {
                            Data = singleResult.Data,
                            Meta = singleResult.Meta,
                            IsSuccess = true,
                            StatusCode = response.StatusCode
                        };
                    }
                    
                    // Fallback: parse directly as T
                    var data = JsonSerializer.Deserialize<T>(jsonString, JsonOptions);
                    return new StrapiResult<T>
                    {
                        Data = data,
                        IsSuccess = true,
                        StatusCode = response.StatusCode
                    };
                }
                else
                {
                    var errorJson = await response.Content.ReadAsStringAsync(cancellationToken);
                    var errorResponse = JsonSerializer.Deserialize<StrapiErrorResponse>(errorJson, JsonOptions);
                    
                    return new StrapiResult<T>
                    {
                        IsSuccess = false,
                        StatusCode = response.StatusCode,
                        Error = errorResponse?.Error,
                        ErrorMessage = errorResponse?.Error?.Message ?? $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}"
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request to Strapi failed: {Endpoint}", request.ContentType);
                return new StrapiResult<T>
                {
                    IsSuccess = false,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Error = new StrapiError { Message = ex.Message },
                    ErrorMessage = ex.Message
                };
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse JSON response from Strapi at {Endpoint}", request.ContentType);
                return new StrapiResult<T>
                {
                    IsSuccess = false,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Error = new StrapiError { Message = ex.Message },
                    ErrorMessage = $"JSON parsing error: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during Strapi request to {Endpoint}", request.ContentType);
                return new StrapiResult<T>
                {
                    IsSuccess = false,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Error = new StrapiError { Message = ex.Message },
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <inheritdoc />
        public async Task<string> GetRawJsonAsync(StrapiRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                using var response = await SendAsync(request, cancellationToken);
                var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);
                
                _logger.LogDebug("Raw JSON response from Strapi: {Json}", jsonString);
                return jsonString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get raw JSON from Strapi at {Endpoint}", request.ContentType);
                throw;
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

        private bool TryParseAsCollectionResponse<T>(string jsonString, out (T Data, Meta? Meta) result)
        {
            result = default;
            
            try
            {
                // Check if this looks like a collection response by looking for "data" array and "meta"
                if (jsonString.Contains("\"data\":[") && jsonString.Contains("\"meta\":"))
                {
                    // Parse the JSON as a generic object first to extract data and meta
                    using var document = JsonDocument.Parse(jsonString);
                    var root = document.RootElement;
                    
                    if (root.TryGetProperty("data", out var dataElement) && 
                        root.TryGetProperty("meta", out var metaElement))
                    {
                        // Deserialize the data array directly to T
                        var dataJson = dataElement.GetRawText();
                        var data = JsonSerializer.Deserialize<T>(dataJson, JsonOptions);
                        
                        // Deserialize the meta object
                        var metaJson = metaElement.GetRawText();
                        var meta = JsonSerializer.Deserialize<Meta>(metaJson, JsonOptions);
                        
                        if (data != null)
                        {
                            result = (data, meta);
                            return true;
                        }
                    }
                }
            }
            catch
            {
                // Not a collection response, continue to next parsing attempt
            }
            
            return false;
        }

        private bool TryParseAsSingleItemResponse<T>(string jsonString, out (T Data, Meta? Meta) result)
        {
            result = default;
            
            try
            {
                // Check if this looks like a single item response by looking for "data" object (not array)
                if (jsonString.Contains("\"data\":{"))
                {
                    // Parse the JSON to extract the data object
                    using var document = JsonDocument.Parse(jsonString);
                    var root = document.RootElement;
                    
                    if (root.TryGetProperty("data", out var dataElement))
                    {
                        // Deserialize the data object directly to T
                        var dataJson = dataElement.GetRawText();
                        var data = JsonSerializer.Deserialize<T>(dataJson, JsonOptions);
                        
                        // Try to get meta if it exists
                        Meta? meta = null;
                        if (root.TryGetProperty("meta", out var metaElement))
                        {
                            var metaJson = metaElement.GetRawText();
                            meta = JsonSerializer.Deserialize<Meta>(metaJson, JsonOptions);
                        }
                        
                        if (data != null)
                        {
                            result = (data, meta);
                            return true;
                        }
                    }
                }
            }
            catch
            {
                // Not a single item response, continue to fallback parsing
            }
            
            return false;
        }
    }
}
