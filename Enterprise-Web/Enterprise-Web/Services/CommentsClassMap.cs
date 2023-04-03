using CsvHelper.Configuration;
using Enterprise_Web.ViewModels;

namespace Enterprise_Web.Services
{
    public class CommentsClassMap : ClassMap<CommentViewModel>
    {
        public CommentsClassMap()
        {
            Map(i => i.Id).Name("id");
            Map(i => i.IdeaId).Name("idea_id");
            Map(i => i.Content).Name("content");
            Map(i => i.CreatedBy).Name("posted_by");
            Map(i => i.CreatedAt).Name("posted_at");
        }
    }
}
