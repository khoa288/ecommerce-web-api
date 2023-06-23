namespace LoginForm.Models.Response
{
    public class PaginationResponse<T>
    {
        public T Data { get; set; } = default!;

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }

        public int TotalRecords { get; set; }

        public PaginationResponse(T data, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Data = data;
        }
    }
}
