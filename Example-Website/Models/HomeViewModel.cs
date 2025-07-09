using Example_Website.Models.Dto;

namespace Example_Website.Models
{
    public class HomeViewModel
    {
        public List<ArticleDto> Articles { get; set; } = new();
        public Dictionary<string, string> Categories { get; set; } = new();
    }
}
