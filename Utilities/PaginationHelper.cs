using EcommerceWebApi.DTOs;

namespace EcommerceWebApi.Utilities
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

    public static class PaginationHelper
    {
        public static PaginationDTO<List<T>> CreatePagedReponse<T>(
            List<T> pagedData,
            PaginationFilter validFilter,
            int totalRecords
        )
        {
            var response = new PaginationDTO<List<T>>(
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
