using Example_Website.Models.Dto;

namespace Example_Website.Models
{
    public class AuthorViewModel
    {
        public string AuthorName { get; set; } = "";
        public List<ArticleDto> Articles { get; set; } = new();
    }
}
