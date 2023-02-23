namespace Enterprise_Web.DTOs
{
  public class CommentDTO
  {
    public int Id { get; set; }
    public string? Content { get; set; }
    public bool? IsAnonymous { get; set; }
  }
}
