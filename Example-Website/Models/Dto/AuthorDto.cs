namespace Example_Website.Models.Dto
{
    public class AuthorDto
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
