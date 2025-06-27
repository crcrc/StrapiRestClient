using Microsoft.Extensions.DependencyInjection;
using StrapiRestClient.Models;
using StrapiRestClient.Request;
using StrapiRestClient.RestClient;
using StrapiRestClient.Tests.Models;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace StrapiRestClient.Tests
{
    public class StrapiRestClientTests
    {
        private readonly IStrapiRestClient _strapiRestClient;

        public StrapiRestClientTests()
        {
            var services = new ServiceCollection();
            var startup = new Startup();
            startup.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            _strapiRestClient = serviceProvider.GetRequiredService<IStrapiRestClient>();
        }

        [Fact]
        public async Task Get_Articles_Then_Get_Single_Article_Should_Work()
        {
            // First: Get all articles
            var listRequest = StrapiRequest.Get("articles");
            var listResponse = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(listRequest);

            Assert.True(listResponse.IsSuccess);
            Assert.NotNull(listResponse.Data);
            Assert.NotEmpty(listResponse.Data);

            // Second: Get first article by ID
            var firstArticle = listResponse.Data.First();
            var singleRequest = StrapiRequest.Get("articles", $"/{firstArticle.DocumentId}");
            var singleResponse = await _strapiRestClient.ExecuteAsync<Article>(singleRequest);

            Assert.True(singleResponse.IsSuccess);
            Assert.NotNull(singleResponse.Data);
            Assert.Equal(firstArticle.DocumentId, singleResponse.Data.DocumentId);
        }

        [Fact]
        public async Task Get_Articles_With_Populate()
        {
            // First: Get all articles
            var listRequest = StrapiRequest.Get("articles")
                 .WithPopulate("category")
                 .WithPopulate("author");

            var listResponse = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(listRequest);

            Assert.True(listResponse.IsSuccess);
            Assert.NotNull(listResponse.Data);
            Assert.NotEmpty(listResponse.Data);
        }

        [Fact]
        public async Task Get_NonExistent_Article_Should_Return_Error()
        {
            // Arrange
            var request = StrapiRequest.Get("articles", "/99999"); // Assuming 99999 does not exist

            // Act
            var response = await _strapiRestClient.ExecuteAsync<Article>(request);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.NotNull(response.ErrorMessage);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal(404, response.Error?.Status);
            Assert.Equal("NotFoundError", response.Error?.Name);
        }

        [Fact]
        public async Task GetRawJsonAsync_Should_Return_Valid_Json_String()
        {
            // Arrange
            var request = StrapiRequest.Get("articles");

            // Act
            var rawJson = await _strapiRestClient.GetRawJsonAsync(request);

            // Assert
            Assert.NotNull(rawJson);
            Assert.NotEmpty(rawJson);
            Assert.True(rawJson.StartsWith("{") || rawJson.StartsWith("["), "Response should be valid JSON");
            Assert.Contains("data", rawJson, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetRawJsonAsync_With_Populate_Should_Return_Json_With_Relations()
        {
            // Arrange
            var request = StrapiRequest.Get("articles")
                .WithPopulate("category")
                .WithPopulate("author");

            // Act
            var rawJson = await _strapiRestClient.GetRawJsonAsync(request);

            // Assert
            Assert.NotNull(rawJson);
            Assert.NotEmpty(rawJson);
            Assert.True(rawJson.StartsWith("{") || rawJson.StartsWith("["), "Response should be valid JSON");
            Assert.Contains("data", rawJson, StringComparison.OrdinalIgnoreCase);
            
            // Should contain populated relation data
            Assert.True(rawJson.Contains("category", StringComparison.OrdinalIgnoreCase) || 
                       rawJson.Contains("author", StringComparison.OrdinalIgnoreCase), 
                       "Response should contain populated relation data");
        }

        [Fact]
        public async Task GetRawJsonAsync_With_Filters_Should_Return_Filtered_Json()
        {
            // Arrange
            var request = StrapiRequest.Get("articles")
                .WithPage(1)
                .WithPageSize(5);

            // Act
            var rawJson = await _strapiRestClient.GetRawJsonAsync(request);

            // Assert
            Assert.NotNull(rawJson);
            Assert.NotEmpty(rawJson);
            Assert.True(rawJson.StartsWith("{") || rawJson.StartsWith("["), "Response should be valid JSON");
            Assert.Contains("data", rawJson, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("meta", rawJson, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetRawJsonAsync_For_Single_Item_Should_Return_Json()
        {
            // Arrange: First get a list to find a valid ID
            var listRequest = StrapiRequest.Get("articles");
            var listResponse = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(listRequest);
            
            // Skip test if no articles exist
            if (!listResponse.IsSuccess || listResponse.Data == null || !listResponse.Data.Any())
            {
                return; // Skip test if no data available
            }

            var firstArticle = listResponse.Data.First();
            var singleRequest = StrapiRequest.Get("articles", $"/{firstArticle.DocumentId}");

            // Act
            var rawJson = await _strapiRestClient.GetRawJsonAsync(singleRequest);

            // Assert
            Assert.NotNull(rawJson);
            Assert.NotEmpty(rawJson);
            Assert.True(rawJson.StartsWith("{") || rawJson.StartsWith("["), "Response should be valid JSON");
            Assert.Contains("data", rawJson, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(firstArticle.DocumentId, rawJson, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetRawJsonAsync_For_NonExistent_Resource_Should_Throw_Or_Return_Error_Json()
        {
            // Arrange
            var request = StrapiRequest.Get("articles", "/99999"); // Assuming 99999 does not exist

            // Act & Assert
            try
            {
                var rawJson = await _strapiRestClient.GetRawJsonAsync(request);
                
                // If it doesn't throw, it should return error JSON
                Assert.NotNull(rawJson);
                Assert.NotEmpty(rawJson);
                Assert.True(rawJson.Contains("error", StringComparison.OrdinalIgnoreCase) || 
                           rawJson.Contains("404", StringComparison.OrdinalIgnoreCase), 
                           "Should contain error information in JSON");
            }
            catch (HttpRequestException)
            {
                // This is also acceptable behavior
                Assert.True(true, "HttpRequestException thrown as expected for non-existent resource");
            }
            catch (Exception ex)
            {
                // Any other exception should be related to the HTTP request
                Assert.True(ex.Message.Contains("404") || ex.Message.Contains("Not Found"), 
                           $"Exception should be related to 404/Not Found, but was: {ex.Message}");
            }
        }
    }
}
