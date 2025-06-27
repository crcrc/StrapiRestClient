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
            var listRequest = StrapiRequest.Get("articles")
                 .WithPopulate("category").WithPopulate("author");
            var listResponse = await _strapiRestClient.ExecuteAsync<StrapiCollectionResponse<ICollection<Article>>>(listRequest);

            Assert.True(listResponse.IsSuccess);
            Assert.NotNull(listResponse.Data);
            Assert.NotEmpty(listResponse.Data.Data);

            // Second: Get first article by ID
            var firstArticle = listResponse.Data.Data.First();
            var singleRequest = StrapiRequest.Get("articles", $"/{firstArticle.DocumentId}");
            var singleResponse = await _strapiRestClient.ExecuteAsync<StrapiCollectionResponse<Article>>(singleRequest);

            Assert.True(singleResponse.IsSuccess);
            Assert.NotNull(singleResponse.Data);
            Assert.Equal(firstArticle.DocumentId, singleResponse.Data.Data.DocumentId);
        }

        [Fact]
        public async Task Get_NonExistent_Article_Should_Return_Error()
        {
            // Arrange
            var request = StrapiRequest.Get("articles", "/99999"); // Assuming 99999 does not exist

            // Act
            var response = await _strapiRestClient.ExecuteAsync<StrapiCollectionResponse<ICollection<Article>>>(request);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.NotNull(response.Error);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal(404, response.Error.Status);
            Assert.Equal("NotFoundError", response.Error.Name);
        }
    }
}
