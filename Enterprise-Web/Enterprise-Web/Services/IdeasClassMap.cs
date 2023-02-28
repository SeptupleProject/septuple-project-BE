using CsvHelper.Configuration;
using Enterprise_Web.ViewModels;

namespace Enterprise_Web.Services
{
    public class IdeasClassMap : ClassMap<IdeaViewModel>
    {
        public IdeasClassMap()
        {
            Map(i => i.Id).Name("id");
            Map(i => i.Title).Name("title");
            Map(i => i.Content).Name("content");
            Map(i => i.Views).Name("num_views");
            Map(i => i.Category).Name("category");
            Map(i => i.AcademicYear).Name("academic_year");
            Map(i => i.CreatedBy).Name("posted_by");
            Map(i => i.CreatedAt).Name("posted_at").TypeConverterOption.Format("s");            
        }
    }
}
