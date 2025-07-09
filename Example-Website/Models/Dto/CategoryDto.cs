namespace Example_Website.Models.Dto
{
    public class CategoryDto
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
}
