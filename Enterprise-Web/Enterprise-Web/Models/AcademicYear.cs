namespace Enterprise_Web.Models
{
    public class AcademicYear
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? IdeaDeadline { get; set; }
    }
}
