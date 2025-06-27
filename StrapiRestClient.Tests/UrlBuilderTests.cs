using StrapiRestClient.Extensions;
using StrapiRestClient.Request;
using Xunit;
using System.Collections.Generic;

namespace StrapiRestClient.Tests
{
    public class UrlBuilderTests
    {
        private const string BaseUrl = "http://localhost:1337/api";

        [Fact]
        public void Create_Should_Build_Correct_Url_For_Get_Request()
        {
            var request = StrapiRequest.Get("articles");
            var url = UrlBuilder.Create(BaseUrl, request);
            Assert.Equal("http://localhost:1337/api/articles", url);
        }

        [Fact]
        public void Create_Should_Build_Correct_Url_With_Id()
        {
            var request = StrapiRequest.Get("articles", "/1");
            var url = UrlBuilder.Create(BaseUrl, request);
            Assert.Equal("http://localhost:1337/api/articles/1", url);
        }

        [Fact]
        public void Create_Should_Build_Correct_Url_With_Slug()
        {
            var request = StrapiRequest.Get("articles", "/my-article-slug");
            var url = UrlBuilder.Create(BaseUrl, request);
            Assert.Equal("http://localhost:1337/api/articles/my-article-slug", url);
        }

        [Fact]
        public void Create_Should_Build_Correct_Url_With_Single_Filter()
        {
            var request = StrapiRequest.Get("articles")
                                        .WithFilter(Enums.FilterType.EqualTo, "title", "Test Article");
            var url = UrlBuilder.Create(BaseUrl, request);
            Assert.Equal("http://localhost:1337/api/articles?filters[title][$eq]=Test Article", url);
        }

        [Fact]
        public void Create_Should_Build_Correct_Url_With_Multiple_Filters()
        {
            var request = StrapiRequest.Get("articles")
                                        .WithFilter(Enums.FilterType.EqualTo, "title", "Test Article")
                                        .WithFilter(Enums.FilterType.Contains, "description", "lorem ipsum");
            var url = UrlBuilder.Create(BaseUrl, request);
            Assert.Equal("http://localhost:1337/api/articles?filters[title][$eq]=Test Article&filters[description][$contains]=lorem ipsum", url);
        }

        [Fact]
        public void Create_Should_Build_Correct_Url_With_Populate_All()
        {
            var request = StrapiRequest.Get("articles")
                                        .WithPopulateAll();
            var url = UrlBuilder.Create(BaseUrl, request);
            Assert.Equal("http://localhost:1337/api/articles?populate=*", url);
        }

        [Fact]
        public void Create_Should_Build_Correct_Url_With_Populate_Specific_Relation()
        {
            var request = StrapiRequest.Get("articles")
                                        .WithPopulate("category");
            var url = UrlBuilder.Create(BaseUrl, request);
            Assert.Equal("http://localhost:1337/api/articles?populate[category]=*", url);
        }

        [Fact]
        public void Create_Should_Build_Correct_Url_With_Populate_Nested_Relation()
        {
            var request = StrapiRequest.Get("articles")
                                        .WithPopulate("category.author");
            var url = UrlBuilder.Create(BaseUrl, request);
            Assert.Equal("http://localhost:1337/api/articles?populate[category][populate][author]=*", url);
        }

        [Fact]
        public void Create_Should_Build_Correct_Url_With_Populate_Fields()
        {
            var request = StrapiRequest.Get("articles")
                                        .WithPopulateFields("category", "name", "slug");
            var url = UrlBuilder.Create(BaseUrl, request);
            Assert.Equal("http://localhost:1337/api/articles?populate[category][fields][0]=name&populate[category][fields][1]=slug", url);
        }

        [Fact]
        public void Create_Should_Build_Correct_Url_With_Sort()
        {
            var request = StrapiRequest.Get("articles")
                                        .WithSort("title", Enums.SortDirection.Ascending);
            var url = UrlBuilder.Create(BaseUrl, request);
            Assert.Equal("http://localhost:1337/api/articles?sort[0]=title:asc", url);
        }

        [Fact]
        public void Create_Should_Build_Correct_Url_With_Multiple_Sorts()
        {
            var request = StrapiRequest.Get("articles")
                                        .WithSort("title", Enums.SortDirection.Ascending)
                                        .WithSort("createdAt", Enums.SortDirection.Descending);
            var url = UrlBuilder.Create(BaseUrl, request);
            Assert.Equal("http://localhost:1337/api/articles?sort[0]=title:asc&sort[1]=createdAt:desc", url);
        }

        [Fact]
        public void Create_Should_Build_Correct_Url_With_Pagination_Page()
        {
            var request = StrapiRequest.Get("articles")
                                        .WithPage(2);
            var url = UrlBuilder.Create(BaseUrl, request);
            Assert.Equal("http://localhost:1337/api/articles?pagination[page]=2", url);
        }

        [Fact]
        public void Create_Should_Build_Correct_Url_With_Pagination_PageSize()
        {
            var request = StrapiRequest.Get("articles")
                                        .WithPageSize(10);
            var url = UrlBuilder.Create(BaseUrl, request);
            Assert.Equal("http://localhost:1337/api/articles?pagination[pageSize]=10", url);
        }

        [Fact]
        public void Create_Should_Build_Correct_Url_With_Randomize()
        {
            var request = StrapiRequest.Get("articles")
                                        .WithRandomSort();
            var url = UrlBuilder.Create(BaseUrl, request);
            Assert.Equal("http://localhost:1337/api/articles?randomSort=true", url);
        }

        [Fact]
        public void Create_Should_Build_Correct_Url_With_In_Filter()
        {
            var request = StrapiRequest.Get("articles")
                                        .WithFilter(Enums.FilterType.In, "id", "1")
                                        .WithFilter(Enums.FilterType.In, "id", "2");
            var url = UrlBuilder.Create(BaseUrl, request);
            Assert.Equal("http://localhost:1337/api/articles?filters[id][$in]=1&filters[id][$in]=2", url);
        }

        [Fact]
        public void Create_Should_Build_Correct_Url_With_NotIn_Filter()
        {
            var request = StrapiRequest.Get("articles")
                                        .WithFilter(Enums.FilterType.NotIn, "id", "1")
                                        .WithFilter(Enums.FilterType.NotIn, "id", "2");
            var url = UrlBuilder.Create(BaseUrl, request);
            Assert.Equal("http://localhost:1337/api/articles?filters[id][$notIn]=1&filters[id][$notIn]=2", url);
        }

        [Fact]
        public void Create_Should_Build_Correct_Url_With_IsNull_Filter()
        {
            var request = StrapiRequest.Get("articles")
                                        .WithFilter(Enums.FilterType.IsNull, "publishedAt", "");
            var url = UrlBuilder.Create(BaseUrl, request);
            Assert.Equal("http://localhost:1337/api/articles?filters[publishedAt][$null]", url);
        }

        [Fact]
        public void Create_Should_Build_Correct_Url_With_IsNotNull_Filter()
        {
            var request = StrapiRequest.Get("articles");
            request.WithFilter(Enums.FilterType.IsNotNull, "publishedAt", "");
            var url = UrlBuilder.Create(BaseUrl, request);
            Assert.Equal("http://localhost:1337/api/articles?filters[publishedAt][$notNull]", url);
        }
    }
}
