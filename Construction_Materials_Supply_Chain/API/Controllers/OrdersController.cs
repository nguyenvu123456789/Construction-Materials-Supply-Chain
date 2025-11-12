using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrdersController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
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
            if (orderDto == null) return NotFound(new { message = "Order not found" });

            return Ok(orderDto);
        }


        [HttpPost("create-purchase-order")]
        public IActionResult CreatePurchaseOrder([FromBody] CreateOrderDto dto)
        {
            try
            {
                var order = _orderService.CreatePurchaseOrder(dto);
                return Ok(Application.Responses.ApiResponse<OrderResponseDto>.SuccessResponse(order, "Tạo đơn hàng thành công"));
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
                return Ok(Application.Responses.ApiResponse<object>.SuccessResponse(order, "Xử lý đơn hàng thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(Application.Responses.ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

    }
}
