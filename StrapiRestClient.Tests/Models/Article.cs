namespace StrapiRestClient.Tests.Models
{
    public class Article
    {
        public string? DocumentId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime PublishedAt { get; set; }
    }
}
