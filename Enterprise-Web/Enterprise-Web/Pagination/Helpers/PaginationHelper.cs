using Enterprise_Web.Pagination.Filter;
using Enterprise_Web.Pagination.Service;
using Enterprise_Web.Pagination.Wrapper;

namespace Enterprise_Web.Pagination.Helpers
{
    public class PaginationHelper
    {
        public static PagedResponse<List<T>> CreatePagedReponse<T>(List<T> pagedData, PaginationFilter validFilter, int totalRecords, IUriService uriService, string route)
        {
            var respose = new PagedResponse<List<T>>(pagedData, validFilter.PageNumber, validFilter.PageSize);
            var totalPages = ((double)totalRecords / (double)validFilter.PageSize);
            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
            int totalRecordsOfCurrentPage = totalRecords % validFilter.PageSize;
            respose.NextUrl = validFilter.PageNumber >= 1 && validFilter.PageNumber < roundedTotalPages
                               ? uriService.GetPageUri(new PaginationFilter(validFilter.PageNumber + 1, validFilter.PageSize, validFilter.Search,validFilter.Role), route)
                               : null;
            respose.NextPage = validFilter.PageNumber >= 1 && validFilter.PageNumber < roundedTotalPages ? validFilter.PageNumber + 1 : 0;
            respose.PreviousUrl = validFilter.PageNumber - 1 >= 1 && validFilter.PageNumber <= roundedTotalPages
                                   ? uriService.GetPageUri(new PaginationFilter(validFilter.PageNumber - 1, validFilter.PageSize, validFilter.Search, validFilter.Role), route)
                                   : uriService.GetPageUri(new PaginationFilter(1, validFilter.PageSize, validFilter.Search, validFilter.Role), route);
            respose.PreviousPage = validFilter.PageNumber - 1;
            respose.FirstUrl = uriService.GetPageUri(new PaginationFilter(1, validFilter.PageSize, validFilter.Search, validFilter.Role), route);
            respose.LastUrl = uriService.GetPageUri(new PaginationFilter(roundedTotalPages, validFilter.PageSize, validFilter.Search, validFilter.Role), route);
            respose.FirstPage = 1;
            respose.LastPage = roundedTotalPages;
            respose.TotalPages = roundedTotalPages;
            respose.TotalRecords = totalRecords;
            respose.TotalRecordsOfCurrentPage = totalRecordsOfCurrentPage == 0 ? roundedTotalPages - 1 : totalRecordsOfCurrentPage;
            return respose;
        }
    }
}
