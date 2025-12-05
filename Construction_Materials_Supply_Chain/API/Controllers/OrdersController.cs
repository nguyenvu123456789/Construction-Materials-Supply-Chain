using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Application.Constants.Messages;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("purchase/{partnerId}")]
        public IActionResult GetPurchaseOrders(int partnerId)
        {
            var orders = _orderService.GetPurchaseOrders(partnerId);
            return Ok(orders);
        }

        [HttpGet("sales/{partnerId}")]
        public IActionResult GetSalesOrders(int partnerId)
        {
            var orders = _orderService.GetSalesOrders(partnerId);
            return Ok(orders);
        }

        [HttpGet("{orderCode}/details")]
        public IActionResult GetOrderDetails(string orderCode)
        {
            var orderDto = _orderService.GetOrderWithDetails(orderCode);
            if (orderDto == null)
                return NotFound(new { message = OrderMessages.ORDER_NOT_FOUND });

            return Ok(orderDto);
        }

        [HttpPost("create-purchase-order")]
        public IActionResult CreatePurchaseOrder([FromBody] CreateOrderDto dto)
        {
            try
            {
                var order = _orderService.CreatePurchaseOrder(dto);
                return Ok(Application.Responses.ApiResponse<OrderResponseDto>
                    .SuccessResponse(order, OrderMessages.ORDER_CREATE_SUCCESS));
            }
            catch (Exception ex)
            {
                return BadRequest(Application.Responses.ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("handle")]
        public IActionResult HandleOrder([FromBody] HandleOrderRequestDto dto)
        {
            try
            {
                var order = _orderService.HandleOrder(dto);
                return Ok(Application.Responses.ApiResponse<object>
                    .SuccessResponse(order, OrderMessages.ORDER_HANDLE_SUCCESS));
            }
            catch (Exception ex)
            {
                return BadRequest(Application.Responses.ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }
    }
}
