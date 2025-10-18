using Application.DTOs;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IOrderService
    {
        OrderResponseDto CreatePurchaseOrder(CreateOrderDto dto);
        Order HandleOrder(HandleOrderRequestDto dto);

        OrderWithDetailsDto? GetOrderWithDetails(string orderCode);
        List<Order> GetAllWithDetails();

    }
}
