using Example_Website.Models.Dto;

namespace Example_Website.Models
{
    public class CategoryViewModel
    {
        public string Category { get; set; } = "";
        public List<ArticleDto> Articles { get; set; } = new();
    }
}
