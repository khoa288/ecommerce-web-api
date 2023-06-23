using LoginForm.Models.Response;

namespace LoginForm.Utils
{
    public class PaginationFilter
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public PaginationFilter()
        {
            PageNumber = 1;
            PageSize = 10;
        }

        public PaginationFilter(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize > 10 ? 10 : pageSize;
        }
    }

    public class QueryFilter
    {
        public string Search { get; set; } = null!;

        public string SortBy { get; set; } = null!;

        public bool IsSortAscending { get; set; }
    }

    public static class PaginationHelper
    {
        public static PaginationResponse<List<T>> CreatePagedResponse<T>(
            List<T> pagedData,
            PaginationFilter validFilter,
            int totalRecords
        )
        {
            var response = new PaginationResponse<List<T>>(
                pagedData,
                validFilter.PageNumber,
                validFilter.PageSize
            );
            var totalPages = (double)totalRecords / validFilter.PageSize;
            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

            response.TotalPages = roundedTotalPages;

            response.TotalRecords = totalRecords;
            return response;
        }
    }
}
