using Enterprise_Web.Models;

namespace EnterpriseWeb.Models
{
  public class Comment : Common
  {
    public int Id { get; set; }
    public string? Content { get; set; }
    public bool? IsAnonymous{ get; set; }
    public User? User { get; set; }
    public int? UserId { get; set; }
    public Idea? Idea { get; set; }
    public int? IdeaId { get; set; }
  }
}