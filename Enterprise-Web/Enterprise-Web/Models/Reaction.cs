using EnterpriseWeb.Models;

namespace Enterprise_Web.Models
{
    public class Reaction
    {
        public int UserId { set; get; }
        public int IdeaId { set; get; }
        public bool? Like { set; get; }
        public Idea? Idea { set; get; }
        public User? User { set; get; }
    }
}
