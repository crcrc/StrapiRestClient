using Microsoft.Extensions.Logging;
using StrapiRestClient.Request;
using StrapiRestClient.RestClient;
using System.Net;
using System.Text;
using Xunit;
using Moq;
using Moq.Protected;

namespace StrapiRestClient.Tests
{
    /// <summary>
    /// Unit tests for the GetRawJsonAsync method with mocked HTTP client.
    /// </summary>
    public class GetRawJsonAsyncTests
    {
        private readonly Mock<ILogger<StrapiRestClient.RestClient.StrapiRestClient>> _mockLogger;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly StrapiRestClient.RestClient.StrapiRestClient _strapiRestClient;

        public GetRawJsonAsyncTests()
        {
            _mockLogger = new Mock<ILogger<StrapiRestClient.RestClient.StrapiRestClient>>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost:1337/api/")
            };
            
            _strapiRestClient = new StrapiRestClient.RestClient.StrapiRestClient(_httpClient, _mockLogger.Object);
        }

        [Fact]
        public async Task GetRawJsonAsync_Should_Return_Raw_Json_String()
        {
            // Arrange
            var expectedJson = """{"data":[{"id":1,"title":"Test Article","content":"Test content"}],"meta":{"pagination":{"page":1,"pageSize":25,"pageCount":1,"total":1}}}""";
            var request = StrapiRequest.Get("articles");

            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedJson, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _strapiRestClient.GetRawJsonAsync(request);

            // Assert
            Assert.Equal(expectedJson, result);
        }

        [Fact]
        public async Task GetRawJsonAsync_Should_Include_Populate_Parameters_In_Request()
        {
            // Arrange
            var expectedJson = """{"data":[{"id":1,"title":"Test Article","category":{"id":1,"name":"Tech"},"author":{"id":1,"name":"John"}}]}""";
            var request = StrapiRequest.Get("articles")
                .WithPopulate("category")
                .WithPopulate("author");

            HttpRequestMessage? capturedRequest = null;
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedJson, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _strapiRestClient.GetRawJsonAsync(request);

            // Assert
            Assert.Equal(expectedJson, result);
            Assert.NotNull(capturedRequest);
            Assert.Contains("populate[0]=category", capturedRequest.RequestUri?.Query);
            Assert.Contains("populate[1]=author", capturedRequest.RequestUri?.Query);
        }

        [Fact]
        public async Task GetRawJsonAsync_Should_Include_Filter_Parameters_In_Request()
        {
            // Arrange
            var expectedJson = """{"data":[{"id":1,"title":"Filtered Article"}],"meta":{"pagination":{"page":1,"pageSize":5}}}""";
            var request = StrapiRequest.Get("articles")
                .WithPage(1)
                .WithPageSize(5)
                .WithEqual("status", "published");

            HttpRequestMessage? capturedRequest = null;
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedJson, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _strapiRestClient.GetRawJsonAsync(request);

            // Assert
            Assert.Equal(expectedJson, result);
            Assert.NotNull(capturedRequest);
            Assert.Contains("pagination[page]=1", capturedRequest.RequestUri?.Query);
            Assert.Contains("pagination[pageSize]=5", capturedRequest.RequestUri?.Query);
            Assert.Contains("filters[status][$eq]=published", capturedRequest.RequestUri?.Query);
        }

        [Fact]
        public async Task GetRawJsonAsync_Should_Throw_Exception_On_Http_Error()
        {
            // Arrange
            var request = StrapiRequest.Get("articles", "/99999");

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Not Found"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                _strapiRestClient.GetRawJsonAsync(request));
        }

        [Fact]
        public async Task GetRawJsonAsync_Should_Return_Error_Json_For_404_Response()
        {
            // Arrange
            var errorJson = """{"error":{"status":404,"name":"NotFoundError","message":"Not Found"}}""";
            var request = StrapiRequest.Get("articles", "/99999");

            var mockResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(errorJson, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _strapiRestClient.GetRawJsonAsync(request);

            // Assert
            Assert.Equal(errorJson, result);
            Assert.Contains("404", result);
            Assert.Contains("NotFoundError", result);
        }

        [Fact]
        public async Task GetRawJsonAsync_Should_Handle_Empty_Response()
        {
            // Arrange
            var request = StrapiRequest.Get("articles");

            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("", Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _strapiRestClient.GetRawJsonAsync(request);

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public async Task GetRawJsonAsync_Should_Log_Debug_Message()
        {
            // Arrange
            var expectedJson = """{"data":[]}""";
            var request = StrapiRequest.Get("articles");

            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedJson, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse);

            // Act
            await _strapiRestClient.GetRawJsonAsync(request);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Raw JSON response from Strapi")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}