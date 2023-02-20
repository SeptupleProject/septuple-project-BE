using Enterprise_Web.Models;
using System.ComponentModel.DataAnnotations;

namespace EnterpriseWeb.Models
{
  public class Category : Common
  {
    public int Id { get; set; }
    public string? Name { get; set; }
    public ICollection<Idea>? Ideas { get; set; }

  }
}
