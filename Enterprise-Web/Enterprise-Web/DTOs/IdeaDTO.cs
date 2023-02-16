namespace Enterprise_Web.DTOs;

public class IdeaDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int Views { get; set; }
    public string Image { get; set; }
    public DateTime CreatedBy { get; set; }
    public bool IsAnonymos { get; set; }
    public int AcademicYear_Id { get; set; }
    public DateTime CreateAt { get; set; }
    public int Category_Id { get; set; }
}