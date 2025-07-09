using StrapiRestClient.Blocks.DataModels;
using StrapiRestClient.Blocks.Extensions;
using System.Text.Json.Serialization;

namespace Example_Website.Models.Dto
{
    public class ArticleDto
    {
        public int? id { get; set; }
        public string? DocumentId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public Media? Cover { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public CategoryDto? category { get; set; }
        public AuthorDto? author { get; set; }
        [JsonConverter(typeof(ExtensibleBlocksConverter))]
        public List<IBlockComponent>? Blocks { get; set; }
    }
}
