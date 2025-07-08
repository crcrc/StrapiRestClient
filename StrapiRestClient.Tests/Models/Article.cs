using StrapiRestClient.Blocks.DataModels;
using StrapiRestClient.Blocks.Extensions;
using System.Text.Json.Serialization;

namespace StrapiRestClient.Tests.Models
{
    public class Article
    {
        public int? id { get; set; }
        public string? DocumentId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public Category? category { get; set; }
        public Author? author { get; set; }
        [JsonConverter(typeof(ExtensibleBlocksConverter))]
        public List<IBlockComponent>? Blocks { get; set; }
    }

    public class Category
    {
        public int? id { get; set; }
        public string? documentId { get; set; }
        public string? name { get; set; }
        public string? slug { get; set; }
        public object? description { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public DateTime? publishedAt { get; set; }
    }

    public class Author
    {
        public int? id { get; set; }
        public string? documentId { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public DateTime? publishedAt { get; set; }
    }
}
