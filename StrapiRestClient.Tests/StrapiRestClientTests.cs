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


        #region Basic Tests (Your existing tests)

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

        #endregion

        #region Filtering Tests

        [Fact]
        public async Task Get_Articles_Filter_Title_Equal()
        {
            var request = new StrapiQueryRequest("articles")
                                .AddFilter("title", FilterBuilder.Equal("Getting Started with Strapi"));

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.All(response.Data, article =>
                Assert.Equal("Getting Started with Strapi", article.Title));
        }

        [Fact]
        public async Task Get_Articles_Filter_Title_Contains()
        {
            var request = new StrapiQueryRequest("articles")
                                .AddFilter("title", FilterBuilder.ContainsCaseInsensitive("strapi"));

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.All(response.Data, article =>
                Assert.Contains("strapi", article.Title.ToLower()));
        }

        [Fact]
        public async Task Get_Articles_Filter_Title_StartsWith()
        {
            var request = new StrapiQueryRequest("articles")
                                .AddFilter("title", FilterBuilder.StartsWith("How to"));

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.All(response.Data, article =>
                Assert.StartsWith("How to", article.Title));
        }

        [Fact]
        public async Task Get_Articles_Filter_PublishedAt_GreaterThan()
        {
            var cutoffDate = new DateTime(2024, 1, 1);
            var request = new StrapiQueryRequest("articles")
                                .AddFilter("publishedAt", FilterBuilder.GreaterThan(cutoffDate.ToString("yyyy-MM-dd")));

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.All(response.Data, article =>
                Assert.True(article.PublishedAt > cutoffDate));
        }


        [Fact]
        public async Task Get_Articles_Filter_Category_In()
        {
            var categories = new[] { "news" };
            var request = new StrapiQueryRequest("articles")
                                .AddPopulate("category", new PopulateOptions { Fields = new[] { "slug" } })
                                .AddRelationFilter("category", "slug", FilterBuilder.In(categories));

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.All(response.Data, article =>
            Assert.Contains(article.category.slug, categories));
        }

        #endregion

        #region Sorting Tests

        [Fact]
        public async Task Get_Articles_Sort_Title_Ascending()
        {
            var request = new StrapiQueryRequest("articles")
                                .WithSort("title:asc");

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);

            var sortedTitles = response.Data.Select(a => a.Title).ToList();
            var expectedSorted = sortedTitles.OrderBy(t => t).ToList();
            Assert.Equal(expectedSorted, sortedTitles);
        }

        [Fact]
        public async Task Get_Articles_Sort_PublishedAt_Descending()
        {
            var request = new StrapiQueryRequest("articles")
                                .WithSort("publishedAt:desc");

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);

            var publishedDates = response.Data.Select(a => a.PublishedAt).ToList();
            var expectedSorted = publishedDates.OrderByDescending(d => d).ToList();
            Assert.Equal(expectedSorted, publishedDates);
        }

        [Fact]
        public async Task Get_Articles_Sort_Multiple_Fields()
        {
            var request = new StrapiQueryRequest("articles")
                                .WithSort("publishedAt:desc", "title:asc");

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);

            // Verify sorting order (publishedAt desc, then title asc)
            for (int i = 1; i < response.Data.Count; i++)
            {
                var current = response.Data.ElementAt(i);
                var previous = response.Data.ElementAt(i - 1);

                Assert.True(previous.PublishedAt >= current.PublishedAt);
                if (previous.PublishedAt == current.PublishedAt)
                {
                    Assert.True(string.Compare(previous.Title, current.Title, StringComparison.Ordinal) <= 0);
                }
            }
        }

        #endregion

        #region Field Selection Tests

        [Fact]
        public async Task Get_Articles_Fields_Title_Only()
        {
            var request = new StrapiQueryRequest("articles")
                                .WithFields("title");

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);
            Assert.All(response.Data, article =>
            {
                Assert.False(string.IsNullOrWhiteSpace(article.Title));
                // Other fields should be null/default when not requested
                Assert.Null(article.Description);
                Assert.Null(article.category);
            });
        }

        [Fact]
        public async Task Get_Articles_Fields_Multiple()
        {
            var request = new StrapiQueryRequest("articles")
                                .WithFields("title", "slug", "publishedAt");

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);
            Assert.All(response.Data, article =>
            {
                Assert.False(string.IsNullOrWhiteSpace(article.Title));
                Assert.False(string.IsNullOrWhiteSpace(article.Slug));
                Assert.True(article.PublishedAt != default);
                // Other fields should be null/default
                Assert.Null(article.Description);
            });
        }

        #endregion

        #region Pagination Tests

        [Fact]
        public async Task Get_Articles_Pagination_Page_Size()
        {
            var request = new StrapiQueryRequest("articles")
                                .WithPagination(page: 1, pageSize: 5);

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.True(response.Data.Count <= 5);
        }

        [Fact]
        public async Task Get_Articles_Pagination_Second_Page()
        {
            var firstPageRequest = new StrapiQueryRequest("articles")
                                        .WithPagination(page: 1, pageSize: 1)
                                        .WithSort("id:asc");

            var secondPageRequest = new StrapiQueryRequest("articles")
                                         .WithPagination(page: 2, pageSize: 1)
                                         .WithSort("id:asc");

            var firstPageResponse = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(firstPageRequest);
            var secondPageResponse = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(secondPageRequest);

            Assert.True(firstPageResponse.IsSuccess);
            Assert.True(secondPageResponse.IsSuccess);
            Assert.NotNull(firstPageResponse.Data);
            Assert.NotNull(secondPageResponse.Data);

            // Ensure different articles are returned
            var firstPageIds = firstPageResponse.Data.Select(a => a.DocumentId).ToList();
            var secondPageIds = secondPageResponse.Data.Select(a => a.DocumentId).ToList();
            Assert.False(firstPageIds.Intersect(secondPageIds).Any());
        }

        [Fact]
        public async Task Get_Articles_Pagination_Start_Limit()
        {
            var request = new StrapiQueryRequest("articles")
            {
                Pagination = new PaginationOptions
                {
                    Start = 5,
                    Limit = 10
                }
            };

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.True(response.Data.Count <= 10);
        }

        #endregion

        #region Locale Tests

        [Fact]
        public async Task Get_Articles_Locale_English()
        {
            var request = new StrapiQueryRequest("articles")
                                .WithLocale("en");

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);
        }

        [Fact]
        public async Task Get_Articles_Locale_French()
        {
            var request = new StrapiQueryRequest("articles")
                                .WithLocale("fr");

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            // May be empty if no French content exists
        }

        #endregion

        #region Complex Populate Tests

        [Fact]
        public async Task Get_Articles_Populate_Category_Name_Slug()
        {
            var request = new StrapiQueryRequest("articles")
                                .AddPopulate("category", new PopulateOptions
                                {
                                    Fields = new[] { "name", "slug" }
                                });

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);
            Assert.All(response.Data.Where(a => a.category != null), article =>
            {
                Assert.False(string.IsNullOrWhiteSpace(article.category.name));
                Assert.False(string.IsNullOrWhiteSpace(article.category.slug));
            });
        }

        [Fact]
        public async Task Get_Articles_Populate_Blocks_Dynamic_Zone()
        {
            var request = new StrapiQueryRequest("articles")
                                .AddPopulateAll("blocks");

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);
            Assert.True(response.Data.Any(a => a.Blocks != null && a.Blocks.Length > 0));
        }

        [Fact]
        public async Task Get_Articles_Populate_Multiple_Relations()
        {
            var request = new StrapiQueryRequest("articles")
                                .AddPopulate("author", new PopulateOptions { Fields = new[] { "name", "email" } })
                                .AddPopulate("category", new PopulateOptions { Fields = new[] { "name", "slug" } })
                                .AddPopulate("tags", new PopulateOptions { Fields = new[] { "name" } });

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);

            var articleWithAllRelations = response.Data.FirstOrDefault(a =>
                a.author != null && a.category != null);

            if (articleWithAllRelations != null)
            {
                Assert.NotNull(articleWithAllRelations.author);
                Assert.NotNull(articleWithAllRelations.category);
            }
        }

        #endregion

        #region Advanced Filtering Tests

        [Fact]
        public async Task Get_Articles_Filter_OR_Conditions()
        {
            var request = new StrapiQueryRequest("articles")
            {
                Filters = new Dictionary<string, object>
                {
                    [FilterOperators.Or] = new object[]
                    {
                        new Dictionary<string, object>
                        {
                            ["title"] = FilterBuilder.ContainsCaseInsensitive("bug")
                        },
                        new Dictionary<string, object>
                        {
                            ["title"] = FilterBuilder.ContainsCaseInsensitive("internet")
                        }
                    }
                }
            };

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.All(response.Data, article =>
            Assert.True(article.Title.ToLower().Contains("bug") || article.Title.ToLower().Contains("internet")));
            Assert.True(response.Data.Count == 2);
        }

        [Fact]
        public async Task Get_Articles_Filter_AND_Conditions_Published()
        {
            var request = new StrapiQueryRequest("articles")
            {
                Status = "published",
                Filters = new Dictionary<string, object>
                {
                    [FilterOperators.And] = new object[]
                    {
                        new Dictionary<string, object>
                        {
                            ["title"] = FilterBuilder.ContainsCaseInsensitive("bug")
                        },
                        new Dictionary<string, object>
                        {
                            ["title"] = FilterBuilder.ContainsCaseInsensitive("internet")
                        }
                    }
                }
            };

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.All(response.Data, article =>
            Assert.True(article.Title.ToLower().Contains("bug") && article.Title.ToLower().Contains("internet")));
            Assert.True(response.Data.Count == 1);
        }

        #endregion

        #region Fluent API Tests

        [Fact]
        public async Task Get_Articles_Fluent_API_Complete_Query()
        {
            var request = new StrapiQueryRequest("articles")
                                .AddFilter("title", FilterBuilder.ContainsCaseInsensitive("internet"))
                                .AddRelationFilter("author", "name", FilterBuilder.Contains("David"))
                                .AddPopulate("author", new PopulateOptions { Fields = new[] { "name", "email" } })
                                .AddPopulate("category", new PopulateOptions { Fields = new[] { "name", "slug" } })
                                .WithFields("title", "slug", "publishedAt", "description")
                                .WithSort("publishedAt:desc")
                                .WithPagination(page: 1, pageSize: 10)
                                .WithStatus("published")
                                .WithLocale("en");

            var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.True(response.Data.Count <= 10);
            Assert.All(response.Data, article =>
            {
                Assert.Contains("internet", article.Title.ToLower());
                Assert.Contains("David", article.author.name);
                Assert.False(string.IsNullOrWhiteSpace(article.Slug));
            });
        }

        #endregion

        #region URL Generation Tests

        [Fact]
        public void Test_URL_Generation_Basic()
        {
            var request = new StrapiQueryRequest("articles")
                                .WithStatus("published");

            var url = request.ToUrl();

            Assert.Contains("/articles", url);
            Assert.Contains("status=published", url);
        }

        [Fact]
        public void Test_URL_Generation_Complex()
        {
            var request = new StrapiQueryRequest("articles")
                                .AddFilter("title", FilterBuilder.ContainsCaseInsensitive("test"))
                                .AddPopulate("author", new PopulateOptions { Fields = new[] { "name" } })
                                .WithStatus("published")
                                .WithLocale("en")
                                .WithPagination(page: 1, pageSize: 10);

            var url = request.ToUrl("https://api.example.com/api");

            Assert.StartsWith("https://api.example.com/api/articles", url);
            Assert.Contains("filters[title][$containsi]=test", url);
            Assert.Contains("populate[author][fields][0]=name", url);
            Assert.Contains("status=published", url);
            Assert.Contains("locale=en", url);
            Assert.Contains("pagination[page]=1", url);
            Assert.Contains("pagination[pageSize]=10", url);
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public void Test_Constructor_Throws_On_Empty_DocumentType()
        {
            Assert.Throws<ArgumentException>(() => new StrapiQueryRequest(""));
            Assert.Throws<ArgumentException>(() => new StrapiQueryRequest(null));
            Assert.Throws<ArgumentException>(() => new StrapiQueryRequest("   "));
        }

        [Fact]
        public void Test_ToUrl_Throws_On_Missing_DocumentType()
        {
            var request = new StrapiQueryRequest();

            Assert.Throws<ArgumentException>(() => request.ToUrl());
        }

        #endregion
    }
}
