namespace Enterprise_Web.Pagination.Wrapper
{
    public class PagedResponse<T> : Response<T>
    {

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Uri FirstUrl { get; set; }
        public int FirstPage { get; set; }
        public Uri LastUrl { get; set; }
        public int LastPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public int TotalRecordsOfCurrentPage { get; set; }
        public Uri NextUrl { get; set; }
        public int NextPage { get; set; }
        public Uri PreviousUrl { get; set; }
        public int PreviousPage { get; set; }

        public PagedResponse(T data, int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.Data = data;
            this.Message = null;
            this.Succeeded = true;
            this.Errors = null;
        }
    }
}
