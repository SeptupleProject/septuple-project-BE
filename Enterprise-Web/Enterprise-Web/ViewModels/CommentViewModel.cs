using EnterpriseWeb.Models;

namespace Enterprise_Web.ViewModels
{
    public class CommentViewModel : Common
    {
        public int Id { get; set; }
        public int? IdeaId { get; set; }
        public string? Content { get; set; }      
    }
}
