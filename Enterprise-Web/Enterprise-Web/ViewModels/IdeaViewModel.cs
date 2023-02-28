using EnterpriseWeb.Models;

namespace Enterprise_Web.ViewModels
{
    public class IdeaViewModel : Common
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int Views { get; set; }
        public string? Category { get; set; }
        public string? AcademicYear { get; set; }       
    }
}
