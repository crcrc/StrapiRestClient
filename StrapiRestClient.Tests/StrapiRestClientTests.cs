using Microsoft.Extensions.DependencyInjection;
using StrapiRestClient.Models;
using StrapiRestClient.Request;
using StrapiRestClient.RestClient;
using StrapiRestClient.Tests.Models;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
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

        //    [Fact]
        //    public async Task Get_Articles_With_()
        //    {
        //        // First: Get all articles
        //        var zzz = new StrapiQueryRequest("articles")
        //.WithLocale("en")
        //.WithStatus("published")
        //.AddPopulate("author", new PopulateOptions { Fields = new[] { "*" } })
        //.AddPopulate("category", new PopulateOptions { Fields = new[] { "name", "slug" } })
        //.AddPopulateAll("blocks")  // Helper for dynamic zones
        //.WithPagination(page: 1, pageSize: 10)
        //        .ToQueryString();

        //        var request = new StrapiQueryRequest("articles")
        //.WithPopulateAll()
        //.WithStatus("published")
        //.WithLocale("en")
        //.AddFilter("title", FilterBuilder.ContainsCaseInsensitive("internet"));

        //          var listResponse = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);
        //    }


        [Fact]
        public async Task Get_Articles()
        {
            var request = new StrapiQueryRequest("articles");

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);
            Assert.False(string.IsNullOrWhiteSpace(response.Data.First().Title));
        }


        [Fact]
        public async Task Get_Articles_PopulateAll()
        {
            var request = new StrapiQueryRequest("articles")
                                .WithPopulateAll();

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);
            Assert.True(response.Data.First().Blocks.Length > 0);
        }

        [Fact]
        public async Task Get_Articles_Draft()
        {
            var request = new StrapiQueryRequest("articles")
                                .WithStatus("draft");

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);
        }

        [Fact]
        public async Task Get_Articles_Draft_PopulateAll()
        {
            var request = new StrapiQueryRequest("articles")
                                .WithStatus("draft")
                                .WithPopulateAll();

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);
            Assert.True(response.Data.First().Blocks.Length > 0);
        }

        [Fact]
        public async Task Get_Articles_Author_PopulateAll()
        {
            var request = new StrapiQueryRequest("articles")
                                .AddPopulate("author", new PopulateOptions { Fields = new[] { "*" } });

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);
            Assert.False(string.IsNullOrWhiteSpace(response.Data.First().author.name));
            Assert.False(string.IsNullOrWhiteSpace(response.Data.First().author.email));
        }


        [Fact]
        public async Task Get_Articles_Author_Populate_Name()
        {
            var request = new StrapiQueryRequest("articles")
                                .AddPopulate("author", new PopulateOptions { Fields = new[] { "name" } });

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);
            Assert.False(string.IsNullOrWhiteSpace(response.Data.First().author.name));
            Assert.True(string.IsNullOrWhiteSpace(response.Data.First().author.email));
        }

        [Fact]
        public async Task Get_Articles_Author_Populate_Name_Email()
        {
            var request = new StrapiQueryRequest("articles")
                                .AddPopulate("author", new PopulateOptions { Fields = new[] { "name", "email" } });

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);
            Assert.False(string.IsNullOrWhiteSpace(response.Data.First().author.name));
            Assert.False(string.IsNullOrWhiteSpace(response.Data.First().author.email));
        }

        [Fact]
        public async Task Get_Articles_Author_Populate_Name_Email_Filter_Sarah()
        {
            var request = new StrapiQueryRequest("articles")
                                .AddPopulate("author", new PopulateOptions { Fields = new[] { "name", "email" } })
                                 .AddRelationFilters("author", new Dictionary<string, object>
                                 {
                                     ["name"] = FilterBuilder.Contains("Sarah")
                                 });

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);
            Assert.True(response.Data.All(a => a.author.name.Contains("Sarah")));
        }

        [Fact]
        public async Task Get_Articles_Author_Populate_Name_Email_Filter_David()
        {
            var request = new StrapiQueryRequest("articles")
                                .AddPopulate("author", new PopulateOptions { Fields = new[] { "name" } })
                                 .AddRelationFilters("author", new Dictionary<string, object>
                                 {
                                     ["email"] = FilterBuilder.ContainsCaseInsensitive("David")
                                 });

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);
            Assert.True(response.Data.All(a => a.author.name.Contains("David")));
        }




    }
}
