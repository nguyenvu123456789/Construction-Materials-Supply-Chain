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

        [HttpPost("create-purchase-order")]
        public IActionResult CreatePurchaseOrder([FromBody] CreateOrderDto dto)
        {
            var order = _orderService.CreatePurchaseOrder(dto);
            return Ok(order);
        }

        [HttpPost("handle")]
        public IActionResult HandleOrder([FromBody] HandleOrderRequestDto dto)
        {
            var order = _orderService.HandleOrder(dto);
            return Ok(order);
        }
    }
}
