using EcommerceWebApi.Authentication;
using EcommerceWebApi.DTOs;
using EcommerceWebApi.Entities;
using EcommerceWebApi.Filters;
using EcommerceWebApi.Notification;
using EcommerceWebApi.Services;
using EcommerceWebApi.Utilities;
using Microsoft.AspNetCore.Mvc;
using static EcommerceWebApi.Services.OrderService;

namespace EcommerceWebApi.Controllers
{
    [ServiceFilter(typeof(AuthorizeFilter))]
    [Controller]
    public class OrderController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IOrderService _orderService;
        private readonly NotificationSubject _notificationSubject;
        private readonly ILogger<OrderController> _logger;

        public OrderController(
            AuthService authService,
            OrderService orderService,
            NotificationSubject notificationSubject,
            ILogger<OrderController> logger
        )
        {
            _authService = authService;
            _orderService = orderService;
            _notificationSubject = notificationSubject;
            _logger = logger;
        }

        [HttpPost("InsertOrder")]
        public async Task<IActionResult> InsertOrder([FromBody] OrderDTO orderDTO)
        {
            try
            {
                User user = _authService.CurrentUser;

                var result = await _orderService.InsertOrderAsync(user.Id, orderDTO.ProductList);
                if (result == OrderResult.Success)
                {
                    _logger.LogInformation("User <{id}> added an order", user.Id);
                    return NoContent();
                }
                return result switch
                {
                    OrderResult.QuantityNotEnough => BadRequest("Quantity not enough"),
                    OrderResult.ProductNotFound => NotFound("Product not found"),
                    OrderResult.Fail => BadRequest("Operation failed"),
                    OrderResult.InvalidInput => BadRequest("Invalid input"),
                    _ => BadRequest("Unable to add order"),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("GetOrderById/{Id}")]
        public IActionResult GetOrderById(string id)
        {
            try
            {
                User user = _authService.CurrentUser;

                var order = _orderService.GetOrderById(id);
                if (order == null)
                {
                    return NotFound();
                }
                else if (user.Id != order.UserId)
                {
                    return Unauthorized();
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("GetUserOrders")]
        public IActionResult GetUserOrders(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
        )
        {
            try
            {
                User user = _authService.CurrentUser;

                var paginationFilter = new PaginationFilter(pageNumber, pageSize);
                var queryFilter = new QueryFilter() { SearchBy = "UserId", Search = user.Id };

                var orders = _orderService.GetPaginationOrders(
                    paginationFilter,
                    queryFilter,
                    out int totalRecords
                );
                var pagedReponse = PaginationHelper.CreatePagedReponse(
                    orders,
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

        [AuthorizeRole("Admin")]
        [HttpPut("UpdateProductStatus/{Id}")]
        public async Task<IActionResult> UpdateProductStatus(
            string id,
            [FromQuery] OrderStatus status = OrderStatus.Canceled
        )
        {
            try
            {
                User user = _authService.CurrentUser;

                var order = _orderService.GetOrderById(id);
                if (order == null)
                {
                    return NotFound();
                }
                else if (user.Id != order.UserId)
                {
                    return Unauthorized();
                }

                var result = await _orderService.UpdateOrderStatusAsync(order, status);
                if (result == OrderResult.Success)
                {
                    _logger.LogInformation("Order <{id}> updated", order.Id);
                    await _notificationSubject.NotifyAsync(
                        user.Id,
                        $"Your order status has been updated to {status}."
                    );
                    return NoContent();
                }
                return result switch
                {
                    OrderResult.QuantityNotEnough => BadRequest("Quantity not enough"),
                    OrderResult.ProductNotFound => NotFound("Product not found"),
                    OrderResult.Fail => BadRequest("Operation failed"),
                    OrderResult.InvalidInput => BadRequest("Invalid input"),
                    _ => BadRequest("Unable to update order"),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        protected override void Dispose(bool disposing)
        {
            _orderService.Dispose();
            base.Dispose(disposing);
        }
    }
}
