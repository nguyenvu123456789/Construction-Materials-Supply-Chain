using Application.DTOs;
using Application.Interfaces;
using Application.Services.Interfaces;
using Domain.Interface;
using Domain.Models;

namespace Application.Services.Implements
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHandleRequestRepository _handleRequestRepository;
        private readonly IPartnerRepository _partnerRepository;
        private readonly IVietnamGeoService _vietnamGeoService;

        public OrderService(
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IHandleRequestRepository handleRequestRepository,
            IPartnerRepository partnerRepository,
            IVietnamGeoService vietnamGeoService)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _handleRequestRepository = handleRequestRepository;
            _partnerRepository = partnerRepository;
            _vietnamGeoService = vietnamGeoService;
        }

        //  Tạo đơn mua hàng
        public OrderResponseDto CreatePurchaseOrder(CreateOrderDto dto)
        {
            if (dto == null)
                throw new Exception("Dữ liệu đơn hàng không hợp lệ");

            var buyer = _userRepository.GetByIdWithPartner(dto.CreatedBy);
            if (buyer == null)
                throw new Exception("Người mua không tồn tại");

            var supplier = _partnerRepository.GetPartnerWithRegions(dto.SupplierId);
            if (supplier == null)
                throw new Exception("Nhà cung cấp không tồn tại");

            var buyerPartner = buyer.Partner;
            if (buyerPartner == null)
                throw new Exception("Người mua chưa thuộc đối tác nào");

            // Check PartnerType
            if (buyerPartner.PartnerTypeId <= supplier.PartnerTypeId)
                throw new Exception("Không thể mua từ đối tác có cùng hoặc cấp cao hơn");

            // ===== Kiểm tra region =====
            var buyerRegionIds = buyerPartner.PartnerRegions.Select(r => r.RegionId).ToList();
            var supplierRegionIds = supplier.PartnerRegions.Select(r => r.RegionId).ToList();

            bool hasCommonRegion = buyerRegionIds.Intersect(supplierRegionIds).Any();
            if (!hasCommonRegion)
                throw new Exception("Người mua và nhà cung cấp không cùng vùng, không thể tạo đơn hàng");
            
            // Sinh mã đơn hàng (PO-001, PO-002, ...)
            var orderCount = _orderRepository.GetAll().Count() + 1;
            var orderCode = $"PO-{orderCount:D3}";

            // Tạo đơn hàng
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
                Quantity = m.Quantity,
                Status = "Pending"
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
                Materials = order.OrderDetails.Select(d => new OrderMaterialResponseDto
                {
                    MaterialId = d.MaterialId,
                    Quantity = d.Quantity,
                    Status = d.Status
                }).ToList()
            };
        }


        public Order HandleOrder(HandleOrderRequestDto dto)
        {
            var order = _orderRepository.GetById(dto.OrderId);
            if (order == null)
                throw new Exception("Không tìm thấy đơn hàng");

            var user = _userRepository.GetById(dto.HandledBy);
            if (user == null)
                throw new Exception("Người xử lý không tồn tại");

            // Cập nhật trạng thái đơn hàng
            order.Status = dto.ActionType;
            order.UpdatedAt = DateTime.Now;
            _orderRepository.Update(order);

            // Lưu lịch sử xử lý
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

            // _unitOfWork.Commit();

            return order;
        }

        public OrderWithDetailsDto? GetOrderWithDetails(string orderCode)
        {
            var order = _orderRepository.GetByCodeWithDetails(orderCode);
            if (order == null) return null;

            var supplierName = order.Supplier?.PartnerName ?? "Không xác định";

            return new OrderWithDetailsDto
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
                    MaterialName = od.Material?.MaterialName ?? "",
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    Status = od.Status
                }).ToList()
            };
        }

        public List<Order> GetPurchaseOrders(int partnerId)
        {
            return _orderRepository
                .GetAllWithDetails()
                .Where(o => o.CreatedByNavigation != null
                            && o.CreatedByNavigation.PartnerId == partnerId)
                .ToList();
        }

        public List<Order> GetSalesOrders(int partnerId)
        {
            return _orderRepository
                .GetAllWithDetails()
                .Where(o => o.SupplierId == partnerId)
                .ToList();
        }
    }
}
