using Enterprise_Web.Models;

namespace EnterpriseWeb.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public Department? Department { get; set; }
        public ICollection<Reaction>? Reactions { set; get; }
        public ICollection<Comment>? Comments { get; set; }
  }
}
