using Application.Constants.Enums;
using Application.Constants.Messages;
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
                throw new Exception(OrderMessages.INVALID_ORDER_DATA);

            var buyer = _userRepository.GetByIdWithPartner(dto.CreatedBy);
            if (buyer == null)
                throw new Exception(OrderMessages.BUYER_NOT_FOUND);

            var supplier = _partnerRepository.GetPartnerWithRegions(dto.SupplierId);
            if (supplier == null)
                throw new Exception(OrderMessages.SUPPLIER_NOT_FOUND);

            var buyerPartner = buyer.Partner;
            if (buyerPartner == null)
                throw new Exception(OrderMessages.BUYER_NO_PARTNER);

            if (buyerPartner.PartnerTypeId <= supplier.PartnerTypeId)
                throw new Exception(OrderMessages.INVALID_PARTNER_LEVEL);

            var buyerRegionIds = buyerPartner.PartnerRegions.Select(r => r.RegionId).ToList();
            var supplierRegionIds = supplier.PartnerRegions.Select(r => r.RegionId).ToList();

            bool hasCommonRegion = buyerRegionIds.Intersect(supplierRegionIds).Any();
            if (!hasCommonRegion)
                throw new Exception(OrderMessages.REGION_MISMATCH);

            var orderCount = _orderRepository.GetAll().Count() + 1;
            var orderCode = $"PO-{orderCount:D3}";

            // Tạo đơn hàng
            var order = new Order
            {
                OrderCode = orderCode,
                CreatedBy = dto.CreatedBy,
                SupplierId = dto.SupplierId,
                WarehouseId = dto.WarehouseId,
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
                Status = StatusEnum.Pending.ToStatusString()
            }).ToList();

            _orderRepository.Add(order);
            var savedOrder = _orderRepository.GetByCode(order.OrderCode);
            return new OrderResponseDto
            {
                OrderId = order.OrderId,
                OrderCode = order.OrderCode,
                CustomerName = buyer.FullName ?? "",
                Status = order.Status ?? "",
                CreatedAt = order.CreatedAt ?? DateTime.Now,
                PhoneNumber = order.PhoneNumber,
                WarehouseId = order.WarehouseId,
                DeliveryAddress = order.DeliveryAddress,
                WarehouseName = savedOrder.Warehouse?.WarehouseName,
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
                throw new Exception(OrderMessages.ORDER_NOT_FOUND);

            var user = _userRepository.GetById(dto.HandledBy);
            if (user == null)
                throw new Exception(OrderMessages.HANDLER_NOT_FOUND);

            // Cập nhật trạng thái đơn hàng
            order.Status = dto.ActionType;
            order.UpdatedAt = DateTime.Now;
            _orderRepository.Update(order);

            // Lưu lịch sử xử lý
            var handle = new HandleRequest
            {
                RequestType = StatusEnum.Order.ToStatusString(),
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

            return new OrderWithDetailsDto
            {
                OrderCode = order.OrderCode,
                PartnerId = order.CreatedBy ?? 0,
                SupplierName = supplierName,
                DeliveryAddress = order.DeliveryAddress,
                PhoneNumber = order.PhoneNumber,
                Note = order.Note,
                WarehouseId = order.WarehouseId,
                WarehouseName = order.Warehouse?.WarehouseName,
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
