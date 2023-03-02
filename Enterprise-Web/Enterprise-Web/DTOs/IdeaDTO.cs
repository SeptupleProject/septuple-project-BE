using Enterprise_Web.Models;
using EnterpriseWeb.Models;

namespace Enterprise_Web.DTOs
{
    public class IdeaDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? CategoryName { get; set; }
        public string? CreatedBy { get; set; }
        public string? Image { get; set; }
        public int Like { get; set; }
        public int DisLike { get; set; }
        public int Comments { get; set; }
    }
}
