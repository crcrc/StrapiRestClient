using System.Diagnostics;
using Example_Website.Models;
using Example_Website.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using StrapiRestClient.Request;
using StrapiRestClient.RestClient;

namespace Example_Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStrapiRestClient _strapiRestClient;

        public HomeController(ILogger<HomeController> logger, IStrapiRestClient strapiRestClient)
        {
            _logger = logger;
            _strapiRestClient = strapiRestClient;
        }

        public async Task<IActionResult> Index(string slug)
        {
            var strapiArticleRequest = new StrapiQueryRequest("articles")
                .WithFields("title", "description", "slug")
                 .AddPopulate("author", new PopulateOptions { Fields = new[] { "name", "email" } })
                 .AddPopulate("category", new PopulateOptions { Fields = new[] { "name", "slug" } })
                 .AddPopulateAll("cover");

            if (!string.IsNullOrEmpty(slug))
            {
                strapiArticleRequest.AddRelationFilter("category", "slug", FilterBuilder.EqualCaseInsensitive(slug));
            }

            var responseArticle = await _strapiRestClient.ExecuteAsync<ICollection<ArticleDto>>(strapiArticleRequest);

            var strapiCategories = new StrapiQueryRequest("categories")
                                        .WithFields("name", "slug");

            var responseCategories = await _strapiRestClient.ExecuteAsync<ICollection<CategoryDto>>(strapiCategories);

            var model = new HomeViewModel
            {
                Articles = responseArticle?.Data?.ToList() ?? new List<ArticleDto>(),
                Categories = responseCategories?.Data?.ToDictionary(c => c.name ?? "", c => c.slug ?? "") ?? new Dictionary<string, string>()
            };

            return View(model);
        }

        public async Task<IActionResult> Author(string slug)
        {
            var strapiRequest = new StrapiQueryRequest("articles")
                .WithPopulateAll()
                .AddRelationFilter("author", "name", FilterBuilder.EqualCaseInsensitive(slug));

            var response = await _strapiRestClient.ExecuteAsync<ICollection<ArticleDto>>(strapiRequest);

            var model = new AuthorViewModel
            {
                AuthorName = slug,
                Articles = response?.Data?.ToList() ?? new List<ArticleDto>()
            };

            return View(model);
        }

        public async Task<IActionResult> Article(string slug)
        {
            var strapiRequest = new StrapiQueryRequest("articles")
                .AddPopulateAll("blocks")
                .AddPopulateAll("author")
                .AddPopulateAll("category")
                .AddFilter("slug", FilterBuilder.EqualCaseInsensitive(slug));


            var response = await _strapiRestClient.ExecuteAsync<ICollection<ArticleDto>>(strapiRequest);

            return View(response.Data.First());
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
