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
        private readonly IPartnerRepository _partnerRepository; 

        public OrderService(
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IHandleRequestRepository handleRequestRepository,
            ITransportRepository transportRepository,
            IShippingLogRepository shippingLogRepository,
            IPartnerRepository partnerRepository) 
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _handleRequestRepository = handleRequestRepository;
            _transportRepository = transportRepository;
            _shippingLogRepository = shippingLogRepository;
            _partnerRepository = partnerRepository;
        }

        public OrderResponseDto CreatePurchaseOrder(CreateOrderDto dto)
        {
            var buyer = _userRepository.GetById(dto.CreatedBy);
            if (buyer == null)
                throw new Exception("Người mua không tồn tại");

            var supplier = _partnerRepository.GetById(dto.SupplierId);
            if (supplier == null)
                throw new Exception("Nhà cung cấp không tồn tại");

            var buyerPartner = buyer.Partner;
            if (buyerPartner == null)
                throw new Exception("Người mua chưa thuộc đối tác nào");

            if (buyerPartner.PartnerTypeId <= supplier.PartnerTypeId)
                throw new Exception("Không thể mua từ đối tác có cùng hoặc cấp cao hơn");

            // Sinh mã đơn hàng
            var orderCount = _orderRepository.GetAll().Count() + 1;
            var orderCode = $"PO-{orderCount:D3}";

            var order = new Order
            {
                OrderCode = orderCode,
                CreatedBy = dto.CreatedBy,
                SupplierId = dto.SupplierId,
                CreatedAt = DateTime.Now,
                Status = "Pending Approval",
                CustomerName = buyer.FullName ?? "",
                PhoneNumber = dto.PhoneNumber,
                DeliveryAddress = dto.DeliveryAddress,
                Note = dto.Note
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
                CustomerName = buyer.FullName ?? "",
                Status = order.Status ?? "",
                CreatedAt = order.CreatedAt ?? DateTime.Now,
                PhoneNumber = order.PhoneNumber,
                DeliveryAddress = order.DeliveryAddress,
                Note = order.Note,
                Materials = order.OrderDetails.Select(d => new OrderMaterialDto
                {
                    MaterialId = d.MaterialId,
                    Quantity = d.Quantity
                }).ToList()
            };
        }

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

            var supplierName = order.Supplier?.PartnerName ?? "Không xác định";

            var dto = new OrderWithDetailsDto
            {
                OrderCode = order.OrderCode,
                PartnerId = order.CreatedBy ?? 0,
                SupplierName = supplierName, 
                DeliveryAddress = order.DeliveryAddress,
                PhoneNumber = order.PhoneNumber,
                Note = order.Note,
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
