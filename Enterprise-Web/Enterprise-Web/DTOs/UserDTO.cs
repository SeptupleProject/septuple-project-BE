using Microsoft.Identity.Client;

namespace Enterprise_Web.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Role  { get; set; }
        public string? DepartmentName { get; set; }
    }
}
