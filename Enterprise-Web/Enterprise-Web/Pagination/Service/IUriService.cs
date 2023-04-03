using Enterprise_Web.Pagination.Filter;

namespace Enterprise_Web.Pagination.Service
{
    public interface IUriService
    {
        public Uri GetPageUri(PaginationFilter filter, string route);
    }
}
