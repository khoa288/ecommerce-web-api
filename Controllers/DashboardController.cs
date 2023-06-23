using LoginForm.Services;
using LoginForm.Utils;
using LoginForm.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LoginForm.Controllers
{
    [ServiceFilter(typeof(AuthorizeFilter))]
    [Controller]
    public class DashboardController : Controller
    {
        private readonly ProductService _productService;

        public DashboardController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string search = "",
            [FromQuery] string sortBy = "",
            [FromQuery] bool isSortAscending = true
        )
        {
            var paginationFilter = new PaginationFilter(pageNumber, pageSize);
            var queryFilter = new QueryFilter()
            {
                Search = search,
                SortBy = sortBy,
                IsSortAscending = isSortAscending
            };

            var products = await _productService.GetProductsAsync(paginationFilter, queryFilter);
            var totalRecords = _productService.QueryProductCount();
            var pagedReponse = PaginationHelper.CreatePagedResponse(
                products,
                paginationFilter,
                totalRecords
            );

            return Ok(pagedReponse);
        }

        [HttpGet("GetProductById/{Id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return Ok(product);
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            var result = await _productService.AddProductAsync(product);
            return result ? NoContent() : BadRequest();
        }

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            var result = await _productService.UpdateProductAsync(product);
            return result ? NoContent() : BadRequest();
        }

        [HttpDelete("DeleteProduct/{Id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            return result ? NoContent() : BadRequest();
        }
    }
}
