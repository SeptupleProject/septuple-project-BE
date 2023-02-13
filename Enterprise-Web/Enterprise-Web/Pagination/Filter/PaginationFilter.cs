namespace Enterprise_Web.Pagination.Filter
{
    public class PaginationFilter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public PaginationFilter()
        {
            this.PageNumber = 1;
            this.PageSize = PageSize == 0 ? 10 : PageSize;
        }
        public PaginationFilter(int pageNumber, int pageSize, string search)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize == 0 ? 10 : pageSize;
            this.Search = search;
        }


    }
}
