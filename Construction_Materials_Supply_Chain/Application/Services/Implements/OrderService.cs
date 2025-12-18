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
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHandleRequestRepository _handleRequestRepository;
        private readonly IPartnerRepository _partnerRepository;
        private readonly IRegionService _regionService;

        public OrderService(
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IHandleRequestRepository handleRequestRepository,
            IPartnerRepository partnerRepository,
            IRegionService regionService,
            IOrderDetailRepository orderDetailRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _handleRequestRepository = handleRequestRepository;
            _partnerRepository = partnerRepository;
            _regionService = regionService;
            _orderDetailRepository = orderDetailRepository;
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

            var buyerRegions = buyerPartner.PartnerRegions
                .Select(r => r.Region.RegionName)
                .ToList();

            var supplierRegions = supplier.PartnerRegions
                .Select(r => r.Region.RegionName)
                .ToList();

            if (!buyerRegions.Any() || !supplierRegions.Any())
                throw new Exception(OrderMessages.REGION_MISMATCH);

            bool canTrade = false;

            foreach (var b in buyerRegions)
            {
                foreach (var s in supplierRegions)
                {
                    if (_regionService.CanTrade(b, s))
                    {
                        canTrade = true;
                        break;
                    }
                }
                if (canTrade) break;
            }

            if (!canTrade)
                throw new Exception(OrderMessages.REGION_MISMATCH);

            var orderCount = _orderRepository.GetAll().Count() + 1;
            var orderCode = $"PO-{orderCount:D3}";

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

            ApplyPricingForOrder(order, buyerPartner.PartnerId, supplier.PartnerId);

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

        //Caculate and save history price
        private void ApplyPricingForOrder(Order order, int buyerPartnerId, int supplierPartnerId)
        {
            var relation = _partnerRepository.GetRelation(buyerPartnerId, supplierPartnerId);

            decimal relationDiscountPercent = relation?.RelationType.DiscountPercent ?? 0;
            decimal relationDiscountAmount = relation?.RelationType.DiscountAmount ?? 0;

            foreach (var detail in order.OrderDetails)
            {
                var priceInfo = _partnerRepository.GetPriceMaterial(detail.MaterialId, supplierPartnerId);

                decimal basePrice = priceInfo?.SellPrice ?? 0;
                decimal materialDiscountPercent = priceInfo?.DiscountPercent ?? 0;
                decimal materialDiscountAmount = priceInfo?.DiscountAmount ?? 0;

                decimal totalDiscountPercent = relationDiscountPercent + materialDiscountPercent;
                decimal totalDiscountAmount = relationDiscountAmount + materialDiscountAmount;

                var discountedValuePercent = (basePrice * totalDiscountPercent / 100);
                var finalPrice = basePrice - discountedValuePercent - totalDiscountAmount;

                if (finalPrice < 0) finalPrice = 0;

                detail.UnitPrice = basePrice;
                detail.DiscountPercent = totalDiscountPercent;
                detail.DiscountAmount = totalDiscountAmount;
                detail.FinalPrice = finalPrice;
            }
        }

        public OrderWithDetailsDto? GetOrderWithDetails(string orderCode)
        {
            var order = _orderRepository.GetByCodeWithDetails(orderCode);
            if (order == null) return null;

            var createByName = order.CreatedByNavigation?.Partner?.PartnerName
                  ?? "Không xác định";


            return new OrderWithDetailsDto
            {
                OrderCode = order.OrderCode,
                PartnerId = order.SupplierId ?? 0,
                SupplierName = createByName,
                DeliveryAddress = order.DeliveryAddress,
                PhoneNumber = order.PhoneNumber,
                Note = order.Note,
                OrderStatus = order.Status,
                WarehouseId = order.WarehouseId,
                WarehouseName = order.Warehouse?.WarehouseName,

                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                {
                    OrderDetailId = od.OrderDetailId,
                    MaterialId = od.MaterialId,
                    MaterialName = od.Material?.MaterialName ?? "",
                    Quantity = od.Quantity,
                    DeliveredQuantity = od.DeliveredQuantity,
                    Status = od.Status,
                    UnitPrice = od.UnitPrice,
                    DiscountPercent = od.DiscountPercent,
                    DiscountAmount = od.DiscountAmount,
                    FinalPrice = od.FinalPrice
                }).ToList()
            };
        }

        public List<OrderResponseDto> GetPurchaseOrders(int partnerId)
        {
            var orders = _orderRepository.GetAllWithWarehouseAndSupplier()
                    .Where(o => o.CreatedByNavigation?.Partner?.PartnerId == partnerId)
                    .ToList();


            foreach (var order in orders)
            {
                order.OrderDetails = _orderDetailRepository.GetByOrderId(order.OrderId);
            }

            return orders.Select(o => new OrderResponseDto
            {
                OrderId = o.OrderId,
                OrderCode = o.OrderCode,
                SupplierName = o.Supplier?.PartnerName ?? "",
                CustomerName = o.Supplier?.PartnerName ?? "",
                Status = o.Status,
                DeliveryAddress = o.DeliveryAddress,
                PhoneNumber = o.PhoneNumber,
                WarehouseId = o.WarehouseId,
                WarehouseName = o.Warehouse?.WarehouseName ?? "",
                Note = o.Note,
                CreatedAt = o.CreatedAt ?? DateTime.Now,
                Materials = o.OrderDetails.Select(d => new OrderMaterialResponseDto
                {
                    MaterialId = d.MaterialId,
                    Quantity = d.Quantity,
                    Status = d.Status
                }).ToList()

            }).ToList();
        }


        public List<OrderResponseDto> GetSalesOrders(int partnerId)
        {
            var orders = _orderRepository.GetAllWithWarehouseAndSupplier()
                .Where(o => o.SupplierId == partnerId)
                .ToList();

            foreach (var order in orders)
            {
                order.OrderDetails = _orderDetailRepository.GetByOrderId(order.OrderId);
            }

            return orders.Select(o => new OrderResponseDto
            {
                OrderId = o.OrderId,
                OrderCode = o.OrderCode,
                SupplierName = o.CreatedByNavigation?.Partner?.PartnerName ?? "",
                CustomerName = o.CreatedByNavigation?.FullName ?? "",
                Status = o.Status,
                DeliveryAddress = o.DeliveryAddress,
                PhoneNumber = o.PhoneNumber,
                WarehouseId = o.WarehouseId,
                WarehouseName = o.Warehouse?.WarehouseName ?? "",
                Note = o.Note,
                CreatedAt = o.CreatedAt ?? DateTime.Now,
                Materials = o.OrderDetails.Select(d => new OrderMaterialResponseDto
                {
                    MaterialId = d.MaterialId,
                    Quantity = d.Quantity,
                    Status = d.Status
                }).ToList()

            }).ToList();
        }

    }
}
