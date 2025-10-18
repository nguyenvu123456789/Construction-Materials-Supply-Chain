using Application.DTOs;
using Application.Interfaces;
using Domain.Interface;
using Domain.Models;

namespace Application.Services.Implements
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHandleRequestRepository _handleRequestRepository;
        private readonly ITransportRepository _transportRepository;
        private readonly IShippingLogRepository _shippingLogRepository;

        public OrderService(
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IHandleRequestRepository handleRequestRepository,
            ITransportRepository transportRepository,
            IShippingLogRepository shippingLogRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _handleRequestRepository = handleRequestRepository;
            _transportRepository = transportRepository;
            _shippingLogRepository = shippingLogRepository;
        }

        // Tạo đơn mua hàng
        public OrderResponseDto CreatePurchaseOrder(CreateOrderDto dto)
        {
            var user = _userRepository.GetById(dto.CreatedBy);
            if (user == null)
                throw new Exception("Người tạo không tồn tại");

            var orderCount = _orderRepository.GetAll().Count() + 1;
            var orderCode = $"PO-{orderCount:D3}";

            var order = new Order
            {
                OrderCode = orderCode,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.Now,
                Status = "Pending Approval",
                CustomerName = user.FullName ?? ""
            };

            order.OrderDetails = dto.Materials.Select(m => new OrderDetail
            {
                MaterialId = m.MaterialId,
                Quantity = m.Quantity
            }).ToList();

            _orderRepository.Add(order);

            return new OrderResponseDto
            {
                OrderId = order.OrderId,
                OrderCode = order.OrderCode,
                CustomerName = user.FullName ?? "",
                Status = order.Status ?? "",
                CreatedAt = order.CreatedAt ?? DateTime.Now,
                Materials = order.OrderDetails.Select(d => new OrderMaterialDto
                {
                    MaterialId = d.MaterialId,
                    Quantity = d.Quantity
                }).ToList()
            };
        }

        // Xử lý approve/reject
        public Order HandleOrder(HandleOrderRequestDto dto)
        {
            var order = _orderRepository.GetById(dto.OrderId);
            if (order == null)
                throw new Exception("Order not found");

            var user = _userRepository.GetById(dto.HandledBy);
            if (user == null)
                throw new Exception("Người xử lý không tồn tại");

            // Cập nhật trạng thái Order
            order.Status = dto.ActionType;
            order.UpdatedAt = DateTime.Now;
            _orderRepository.Update(order);

            // Nếu approve thì gán transport
            if (dto.ActionType == "Approved")
            {
                if (dto.TransportId == null)
                    throw new Exception("Phải chọn đơn vị vận chuyển khi approve");

                var transport = _transportRepository.GetById(dto.TransportId.Value);
                if (transport == null)
                    throw new Exception("Transport không tồn tại");

                var shippingLog = new ShippingLog
                {
                    OrderId = order.OrderId,
                    TransportId = transport.TransportId,
                    Status = "Assigned",
                    CreatedAt = DateTime.Now
                };
                _shippingLogRepository.Add(shippingLog);
            }

            // Ghi log xử lý
            var handle = new HandleRequest
            {
                RequestType = "Order",
                RequestId = order.OrderId,
                HandledBy = dto.HandledBy,
                ActionType = dto.ActionType,
                Note = dto.Note,
                HandledAt = DateTime.Now
            };
            _handleRequestRepository.Add(handle);

            return order;
        }

        public OrderWithDetailsDto? GetOrderWithDetails(string orderCode)
        {
            var order = _orderRepository.GetByCodeWithDetails(orderCode);
            if (order == null) return null;

            var dto = new OrderWithDetailsDto
            {
                OrderCode = order.OrderCode,
                PartnerId = order.CreatedBy ?? 0,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                {
                    MaterialId = od.MaterialId,
                    MaterialName = od.Material.MaterialName,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList()
            };

            return dto;
        }
        public List<Order> GetAllWithDetails()
        {
            return _orderRepository.GetAllWithDetails().ToList();
        }

    }
}
