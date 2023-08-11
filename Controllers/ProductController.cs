using EcommerceWebApi.DTOs;
using EcommerceWebApi.Entities;
using EcommerceWebApi.Filters;
using EcommerceWebApi.Notification;
using EcommerceWebApi.Services;
using EcommerceWebApi.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebApi.Controllers
{
    [ServiceFilter(typeof(AuthorizeFilter))]
    [Controller]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly NotificationSubject _notificationSubject;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            ProductService productService,
            NotificationSubject notificationSubject,
            ILogger<ProductController> logger
        )
        {
            _productService = productService;
            _notificationSubject = notificationSubject;
            _logger = logger;
        }

        [HttpGet("GetProducts")]
        public IActionResult GetProducts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchBy = "title",
            [FromQuery] string search = "",
            [FromQuery] string sortBy = "",
            [FromQuery] bool isSortAscending = true
        )
        {
            try
            {
                var paginationFilter = new PaginationFilter(pageNumber, pageSize);
                var queryFilter = new QueryFilter()
                {
                    SearchBy = searchBy,
                    Search = search,
                    SortBy = sortBy,
                    IsSortAscending = isSortAscending
                };

                var products = _productService.GetPaginationProducts(
                    paginationFilter,
                    queryFilter,
                    out int totalRecords
                );
                var pagedReponse = PaginationHelper.CreatePagedReponse(
                    products,
                    paginationFilter,
                    totalRecords
                );

                return Ok(pagedReponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("GetProductById/{Id}")]
        public IActionResult GetProductById(int id)
        {
            try
            {
                var product = _productService.GetProductById(id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AuthorizeRole("Admin")]
        [HttpPost("InsertProduct")]
        public async Task<IActionResult> InsertProduct([FromBody] ProductDTO productDTO)
        {
            try
            {
                var product = new Product()
                {
                    Id = _productService.GetNextProductId(),
                    Title = productDTO.Title,
                    Price = productDTO.Price,
                    Brand = productDTO.Brand,
                    Category = productDTO.Category,
                    Thumbnail = productDTO.Thumbnail,
                    Quantity = productDTO.Quantity,
                    Rating = 0
                };
                var result = await _productService.InsertProductAsync(product);
                if (result)
                {
                    _logger.LogInformation("Product <{Id}> inserted", product.Id);
                    await _notificationSubject.NotifyAsync(
                        "all",
                        $"A new product has been added: {product.Title}"
                    );
                    return NoContent();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AuthorizeRole("Admin")]
        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            try
            {
                var result = await _productService.UpdateProductAsync(product);
                if (result)
                {
                    _logger.LogInformation("Product <{Id}> updated", product.Id);
                    return NoContent();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AuthorizeRole("Admin")]
        [HttpDelete("DeleteProduct/{Id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                if (result)
                {
                    _logger.LogInformation("Product <{Id}> deleted", id);
                    return NoContent();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        protected override void Dispose(bool disposing)
        {
            _productService.Dispose();
            base.Dispose(disposing);
        }
    }
}
