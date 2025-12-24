using Application.Constants.Enums;
using Application.DTOs;
using Application.Interfaces;
using Application.Services.Implements;
using Application.Services.Interfaces;
using Domain.Interface;
using Domain.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
// ✅ Fix ambiguity Messages bằng alias (đổi namespace cho đúng project bạn nếu khác)
using OrderMsgs = Application.Constants.Messages.OrderMessages;

namespace Services.Tests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private Mock<IOrderRepository> _orderRepository = null!;
        private Mock<IOrderDetailRepository> _orderDetailRepository = null!;
        private Mock<IUserRepository> _userRepository = null!;
        private Mock<IHandleRequestRepository> _handleRequestRepository = null!;
        private Mock<IPartnerRepository> _partnerRepository = null!;
        private Mock<IRegionService> _regionService = null!;

        private IOrderService _sut = null!;

        [SetUp]
        public void SetUp()
        {
            _orderRepository = new Mock<IOrderRepository>(MockBehavior.Strict);
            _orderDetailRepository = new Mock<IOrderDetailRepository>(MockBehavior.Strict);
            _userRepository = new Mock<IUserRepository>(MockBehavior.Strict);
            _handleRequestRepository = new Mock<IHandleRequestRepository>(MockBehavior.Strict);

            // PartnerRepository và RegionService thường có nhiều method, để Loose cho đỡ “missing setup”
            _partnerRepository = new Mock<IPartnerRepository>(MockBehavior.Loose);
            _regionService = new Mock<IRegionService>(MockBehavior.Loose);

            _sut = new OrderService(
                _orderRepository.Object,
                _userRepository.Object,
                _handleRequestRepository.Object,
                _partnerRepository.Object,
                _regionService.Object,
                _orderDetailRepository.Object
            );
        }

        // -----------------------------
        // Helpers
        // -----------------------------
        private static Partner MakePartner(int partnerId, int partnerTypeId, params string[] regionNames)
        {
            var p = new Partner
            {
                PartnerId = partnerId,
                PartnerTypeId = partnerTypeId,
                PartnerRegions = new List<PartnerRegion>()
            };

            foreach (var rn in regionNames)
            {
                p.PartnerRegions.Add(new PartnerRegion
                {
                    Region = new Region { RegionName = rn }
                });
            }

            return p;
        }

        private static User MakeUserWithPartner(int userId, string fullName, Partner partner)
        {
            return new User
            {
                UserId = userId,
                FullName = fullName,
                Partner = partner
            };
        }

        private static CreateOrderDto MakeValidCreateOrderDto(int createdBy = 1, int supplierId = 2)
        {
            return new CreateOrderDto
            {
                CreatedBy = createdBy,
                SupplierId = supplierId,
                WarehouseId = 10,
                PhoneNumber = "0123",
                DeliveryAddress = "Addr",
                Note = "Note",
                Materials = new List<CreateOrderMaterialDto>
                {
                    new CreateOrderMaterialDto { MaterialId = 101, Quantity = 2 },
                    new CreateOrderMaterialDto { MaterialId = 102, Quantity = 1 }
                }
            };
        }

        // =========================================================
        // CreatePurchaseOrder
        // =========================================================

        [Test]
        public void CreatePurchaseOrder_WhenDtoNull_ThrowsInvalidOrderData()
        {
            var ex = Assert.Throws<Exception>(() => _sut.CreatePurchaseOrder(null!));
            Assert.That(ex!.Message, Is.EqualTo(OrderMsgs.INVALID_ORDER_DATA));

            _orderRepository.VerifyNoOtherCalls();
            _userRepository.VerifyNoOtherCalls();
            _partnerRepository.VerifyNoOtherCalls();
            _regionService.VerifyNoOtherCalls();
            _handleRequestRepository.VerifyNoOtherCalls();
            _orderDetailRepository.VerifyNoOtherCalls();
        }

        [Test]
        public void CreatePurchaseOrder_WhenBuyerNotFound_Throws()
        {
            var dto = MakeValidCreateOrderDto(createdBy: 1);

            _userRepository.Setup(r => r.GetByIdWithPartner(1)).Returns((User?)null);

            var ex = Assert.Throws<Exception>(() => _sut.CreatePurchaseOrder(dto));
            Assert.That(ex!.Message, Is.EqualTo(OrderMsgs.BUYER_NOT_FOUND));

            _userRepository.VerifyAll();
            _orderRepository.VerifyNoOtherCalls();
        }

        [Test]
        public void CreatePurchaseOrder_WhenSupplierNotFound_Throws()
        {
            var dto = MakeValidCreateOrderDto(createdBy: 1, supplierId: 2);

            var buyerPartner = MakePartner(partnerId: 10, partnerTypeId: 5, "HN");
            var buyer = MakeUserWithPartner(1, "Buyer", buyerPartner);

            _userRepository.Setup(r => r.GetByIdWithPartner(1)).Returns(buyer);
            _partnerRepository.Setup(r => r.GetPartnerWithRegions(2)).Returns((Partner?)null);

            var ex = Assert.Throws<Exception>(() => _sut.CreatePurchaseOrder(dto));
            Assert.That(ex!.Message, Is.EqualTo(OrderMsgs.SUPPLIER_NOT_FOUND));

            _userRepository.VerifyAll();
            _partnerRepository.VerifyAll();
            _orderRepository.VerifyNoOtherCalls();
        }

        [Test]
        public void CreatePurchaseOrder_WhenBuyerHasNoPartner_Throws()
        {
            var dto = MakeValidCreateOrderDto(createdBy: 1, supplierId: 2);

            var buyer = new User { UserId = 1, FullName = "Buyer", Partner = null };

            _userRepository.Setup(r => r.GetByIdWithPartner(1)).Returns(buyer);
            _partnerRepository.Setup(r => r.GetPartnerWithRegions(2))
                              .Returns(MakePartner(20, 1, "HN"));

            var ex = Assert.Throws<Exception>(() => _sut.CreatePurchaseOrder(dto));
            Assert.That(ex!.Message, Is.EqualTo(OrderMsgs.BUYER_NO_PARTNER));

            _userRepository.VerifyAll();
            _partnerRepository.VerifyAll();
            _orderRepository.VerifyNoOtherCalls();
        }

        [Test]
        public void CreatePurchaseOrder_WhenBuyerPartnerTypeNotHigherThanSupplier_Throws()
        {
            var dto = MakeValidCreateOrderDto(createdBy: 1, supplierId: 2);

            // buyerPartnerTypeId <= supplierPartnerTypeId => invalid
            var buyerPartner = MakePartner(10, partnerTypeId: 2, "HN");
            var buyer = MakeUserWithPartner(1, "Buyer", buyerPartner);

            var supplier = MakePartner(20, partnerTypeId: 2, "HN");

            _userRepository.Setup(r => r.GetByIdWithPartner(1)).Returns(buyer);
            _partnerRepository.Setup(r => r.GetPartnerWithRegions(2)).Returns(supplier);

            var ex = Assert.Throws<Exception>(() => _sut.CreatePurchaseOrder(dto));
            Assert.That(ex!.Message, Is.EqualTo(OrderMsgs.INVALID_PARTNER_LEVEL));

            _userRepository.VerifyAll();
            _partnerRepository.VerifyAll();
            _orderRepository.VerifyNoOtherCalls();
        }

        [Test]
        public void CreatePurchaseOrder_WhenRegionsEmpty_ThrowsRegionMismatch()
        {
            var dto = MakeValidCreateOrderDto(createdBy: 1, supplierId: 2);

            var buyerPartner = MakePartner(10, partnerTypeId: 5 /* no regions */);
            var buyer = MakeUserWithPartner(1, "Buyer", buyerPartner);

            var supplier = MakePartner(20, partnerTypeId: 1 /* no regions */);

            _userRepository.Setup(r => r.GetByIdWithPartner(1)).Returns(buyer);
            _partnerRepository.Setup(r => r.GetPartnerWithRegions(2)).Returns(supplier);

            var ex = Assert.Throws<Exception>(() => _sut.CreatePurchaseOrder(dto));
            Assert.That(ex!.Message, Is.EqualTo(OrderMsgs.REGION_MISMATCH));

            _userRepository.VerifyAll();
            _partnerRepository.VerifyAll();
            _orderRepository.VerifyNoOtherCalls();
        }

        [Test]
        public void CreatePurchaseOrder_WhenNoTradableRegions_ThrowsRegionMismatch()
        {
            var dto = MakeValidCreateOrderDto(createdBy: 1, supplierId: 2);

            var buyerPartner = MakePartner(10, partnerTypeId: 5, "HN");
            var buyer = MakeUserWithPartner(1, "Buyer", buyerPartner);

            var supplier = MakePartner(20, partnerTypeId: 1, "HCM");

            _userRepository.Setup(r => r.GetByIdWithPartner(1)).Returns(buyer);
            _partnerRepository.Setup(r => r.GetPartnerWithRegions(2)).Returns(supplier);

            // CanTrade luôn false
            _regionService.Setup(s => s.CanTrade(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(false);

            var ex = Assert.Throws<Exception>(() => _sut.CreatePurchaseOrder(dto));
            Assert.That(ex!.Message, Is.EqualTo(OrderMsgs.REGION_MISMATCH));

            _userRepository.VerifyAll();
            _partnerRepository.VerifyAll();
            _orderRepository.VerifyNoOtherCalls();
        }

        [Test]
        public void CreatePurchaseOrder_Success_CreatesOrder_AddsAndReturnsResponse()
        {
            var dto = MakeValidCreateOrderDto(createdBy: 1, supplierId: 2);

            var buyerPartner = MakePartner(10, partnerTypeId: 5, "HN");
            var buyer = MakeUserWithPartner(1, "Buyer Name", buyerPartner);

            var supplier = MakePartner(20, partnerTypeId: 1, "HN");

            _userRepository.Setup(r => r.GetByIdWithPartner(1)).Returns(buyer);
            _partnerRepository.Setup(r => r.GetPartnerWithRegions(2)).Returns(supplier);

            _regionService.Setup(s => s.CanTrade(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(true);

            // GetAll dùng để tạo PO-xxx
            _orderRepository.Setup(r => r.GetAll()).Returns(new List<Order>
            {
                new Order { OrderCode = "PO-001" },
                new Order { OrderCode = "PO-002" }
            });

            Order? addedOrder = null;
            _orderRepository.Setup(r => r.Add(It.IsAny<Order>()))
                            .Callback<Order>(o =>
                            {
                                // giả lập DB set OrderId
                                o.OrderId = 123;
                                addedOrder = o;
                            });

            // GetByCode để lấy WarehouseName trong response
            _orderRepository.Setup(r => r.GetByCode(It.IsAny<string>()))
                            .Returns((string code) => new Order
                            {
                                OrderCode = code,
                                Warehouse = new Warehouse { WarehouseName = "WH-A" }
                            });

            var res = _sut.CreatePurchaseOrder(dto);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.OrderId, Is.EqualTo(123));
            Assert.That(res.OrderCode, Is.EqualTo("PO-003")); // 2 existing + 1
            Assert.That(res.CustomerName, Is.EqualTo("Buyer Name"));
            Assert.That(res.Status, Is.EqualTo("Pending Approval"));
            Assert.That(res.WarehouseName, Is.EqualTo("WH-A"));

            Assert.That(addedOrder, Is.Not.Null);
            Assert.That(addedOrder!.OrderDetails, Is.Not.Null);
            Assert.That(addedOrder.OrderDetails.Count, Is.EqualTo(dto.Materials.Count));

            // Status từng detail = Pending
            Assert.That(addedOrder.OrderDetails.All(d => d.Status == StatusEnum.Pending.ToStatusString()), Is.True);

            _userRepository.VerifyAll();
            _orderRepository.VerifyAll();
        }

        // =========================================================
        // HandleOrder
        // =========================================================

        [Test]
        public void HandleOrder_WhenOrderNotFound_Throws()
        {
            var dto = new HandleOrderRequestDto
            {
                OrderId = 999,
                HandledBy = 1,
                ActionType = "Approved",
                Note = "ok"
            };

            _orderRepository.Setup(r => r.GetById(999)).Returns((Order?)null);

            var ex = Assert.Throws<Exception>(() => _sut.HandleOrder(dto));
            Assert.That(ex!.Message, Is.EqualTo(OrderMsgs.ORDER_NOT_FOUND));

            _orderRepository.VerifyAll();
            _userRepository.VerifyNoOtherCalls();
            _handleRequestRepository.VerifyNoOtherCalls();
        }

        [Test]
        public void HandleOrder_WhenHandlerNotFound_Throws()
        {
            var dto = new HandleOrderRequestDto
            {
                OrderId = 1,
                HandledBy = 999,
                ActionType = "Approved",
                Note = "ok"
            };

            var order = new Order { OrderId = 1, Status = "Pending Approval" };

            _orderRepository.Setup(r => r.GetById(1)).Returns(order);
            _userRepository.Setup(r => r.GetById(999)).Returns((User?)null);

            var ex = Assert.Throws<Exception>(() => _sut.HandleOrder(dto));
            Assert.That(ex!.Message, Is.EqualTo(OrderMsgs.HANDLER_NOT_FOUND));

            _orderRepository.VerifyAll();
            _userRepository.VerifyAll();
            _handleRequestRepository.VerifyNoOtherCalls();
        }

        [Test]
        public void HandleOrder_Success_UpdatesOrder_AndCreatesHandleHistory()
        {
            var dto = new HandleOrderRequestDto
            {
                OrderId = 1,
                HandledBy = 7,
                ActionType = "Approved",
                Note = "fine"
            };

            var order = new Order { OrderId = 1, Status = "Pending Approval" };
            var user = new User { UserId = 7 };

            _orderRepository.Setup(r => r.GetById(1)).Returns(order);
            _userRepository.Setup(r => r.GetById(7)).Returns(user);
            _orderRepository.Setup(r => r.Update(order));

            HandleRequest? addedHandle = null;
            _handleRequestRepository.Setup(r => r.Add(It.IsAny<HandleRequest>()))
                                    .Callback<HandleRequest>(h => addedHandle = h);

            var result = _sut.HandleOrder(dto);

            Assert.That(result, Is.SameAs(order));
            Assert.That(order.Status, Is.EqualTo("Approved"));
            Assert.That(order.UpdatedAt, Is.Not.Null);

            Assert.That(addedHandle, Is.Not.Null);
            Assert.That(addedHandle!.RequestType, Is.EqualTo(StatusEnum.Order.ToStatusString()));
            Assert.That(addedHandle.RequestId, Is.EqualTo(1));
            Assert.That(addedHandle.HandledBy, Is.EqualTo(7));
            Assert.That(addedHandle.ActionType, Is.EqualTo("Approved"));
            Assert.That(addedHandle.Note, Is.EqualTo("fine"));
            Assert.That(addedHandle!.HandledAt, Is.Not.EqualTo(DateTime.MinValue));

            _orderRepository.VerifyAll();
            _userRepository.VerifyAll();
            _handleRequestRepository.VerifyAll();
        }

        // =========================================================
        // GetOrderWithDetails
        // =========================================================

        [Test]
        public void GetOrderWithDetails_WhenNotFound_ReturnsNull()
        {
            _orderRepository.Setup(r => r.GetByCodeWithDetails("PO-001"))
                            .Returns((Order?)null);

            var result = _sut.GetOrderWithDetails("PO-001");

            Assert.That(result, Is.Null);
            _orderRepository.VerifyAll();
        }

        [Test]
        public void GetOrderWithDetails_Success_MapsDto()
        {
            var order = new Order
            {
                OrderCode = "PO-001",
                SupplierId = 20,
                DeliveryAddress = "Addr",
                PhoneNumber = "0123",
                Note = "N",
                Status = "Pending Approval",
                WarehouseId = 10,
                Warehouse = new Warehouse { WarehouseName = "WH-A" },
                CreatedByNavigation = new User
                {
                    Partner = new Partner { PartnerName = "SupplierNameFromCreatedByPartner" }
                },
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        OrderDetailId = 1,
                        MaterialId = 101,
                        Material = new Material { MaterialName = "Cement" },
                        Quantity = 2,
                        DeliveredQuantity = 0,
                        Status = "Pending",
                        UnitPrice = 100,
                        DiscountPercent = 5,
                        DiscountAmount = 2,
                        FinalPrice = 93
                    }
                }
            };

            _orderRepository.Setup(r => r.GetByCodeWithDetails("PO-001"))
                            .Returns(order);

            var dto = _sut.GetOrderWithDetails("PO-001");

            Assert.That(dto, Is.Not.Null);
            Assert.That(dto!.OrderCode, Is.EqualTo("PO-001"));
            Assert.That(dto.PartnerId, Is.EqualTo(20));
            Assert.That(dto.SupplierName, Is.EqualTo("SupplierNameFromCreatedByPartner"));
            Assert.That(dto.WarehouseName, Is.EqualTo("WH-A"));
            Assert.That(dto.OrderDetails, Has.Count.EqualTo(1));
            Assert.That(dto.OrderDetails[0].MaterialName, Is.EqualTo("Cement"));

            _orderRepository.VerifyAll();
        }

        // =========================================================
        // GetPurchaseOrders
        // =========================================================

        [Test]
        public void GetPurchaseOrders_FiltersByCreatedByPartner_AndLoadsDetails()
        {
            int partnerId = 10;

            var orders = new List<Order>
            {
                new Order
                {
                    OrderId = 1,
                    OrderCode = "PO-001",
                    Supplier = new Partner { PartnerName = "Supp1" },
                    CreatedByNavigation = new User { Partner = new Partner { PartnerId = partnerId } },
                    WarehouseId = 5,
                    Warehouse = new Warehouse { WarehouseName = "WH1" },
                    Status = "Pending",
                    DeliveryAddress = "A",
                    PhoneNumber = "1",
                    Note = "N",
                    CreatedAt = DateTime.Now
                },
                new Order
                {
                    OrderId = 2,
                    OrderCode = "PO-002",
                    Supplier = new Partner { PartnerName = "Supp2" },
                    CreatedByNavigation = new User { Partner = new Partner { PartnerId = 999 } }, // filtered out
                    WarehouseId = 6,
                    Warehouse = new Warehouse { WarehouseName = "WH2" },
                    Status = "Pending"
                }
            };

            _orderRepository.Setup(r => r.GetAllWithWarehouseAndSupplier()).Returns(orders);

            _orderDetailRepository.Setup(r => r.GetByOrderId(1))
                                  .Returns(new List<OrderDetail>
                                  {
                                      new OrderDetail { MaterialId = 101, Quantity = 2, Status = "Pending" }
                                  });

            // orderId=2 sẽ không được gọi (filtered out), nếu gọi sẽ fail vì Strict
            var result = _sut.GetPurchaseOrders(partnerId);

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].OrderId, Is.EqualTo(1));
            Assert.That(result[0].WarehouseName, Is.EqualTo("WH1"));
            Assert.That(result[0].Materials, Has.Count.EqualTo(1));

            _orderRepository.VerifyAll();
            _orderDetailRepository.VerifyAll();
        }

        // =========================================================
        // GetSalesOrders
        // =========================================================

        [Test]
        public void GetSalesOrders_FiltersBySupplierId_AndLoadsDetails()
        {
            int partnerId = 20;

            var orders = new List<Order>
            {
                new Order
                {
                    OrderId = 1,
                    OrderCode = "SO-001",
                    SupplierId = partnerId, // keep
                    CreatedByNavigation = new User { FullName = "Buyer1", Partner = new Partner { PartnerName = "BuyerPartner" } },
                    WarehouseId = 5,
                    Warehouse = new Warehouse { WarehouseName = "WH1" },
                    Status = "Pending",
                    DeliveryAddress = "A",
                    PhoneNumber = "1",
                    Note = "N",
                    CreatedAt = DateTime.Now
                },
                new Order
                {
                    OrderId = 2,
                    OrderCode = "SO-002",
                    SupplierId = 999, // filtered out
                    Status = "Pending"
                }
            };

            _orderRepository.Setup(r => r.GetAllWithWarehouseAndSupplier()).Returns(orders);

            _orderDetailRepository.Setup(r => r.GetByOrderId(1))
                                  .Returns(new List<OrderDetail>
                                  {
                                      new OrderDetail { MaterialId = 201, Quantity = 3, Status = "Pending" }
                                  });

            var result = _sut.GetSalesOrders(partnerId);

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].OrderId, Is.EqualTo(1));
            Assert.That(result[0].WarehouseName, Is.EqualTo("WH1"));
            Assert.That(result[0].CustomerName, Is.EqualTo("Buyer1"));
            Assert.That(result[0].Materials, Has.Count.EqualTo(1));

            _orderRepository.VerifyAll();
            _orderDetailRepository.VerifyAll();
        }
    }
}
