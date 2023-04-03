namespace Enterprise_Web.DTOs
{
    public class DepartmentDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Users { get; set; }
        public string? ManagedBy { get; set; }
    }
}
