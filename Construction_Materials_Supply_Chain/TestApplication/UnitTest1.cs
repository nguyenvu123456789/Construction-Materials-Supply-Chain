using System;
using System.Collections.Generic;
using System.Linq;
using Application.Constants.Enums;
using Application.Constants.Messages;
using Application.DTOs;
using AutoMapper;
using Domain.Interface;
using Domain.Models;
using Moq;
using NUnit.Framework;
using Services.Implementations;

namespace Services.Tests
{
    [TestFixture]
    public class InvoiceServiceTests
    {
        private Mock<IInvoiceRepository> _invoiceRepo = null!;
        private Mock<IMaterialRepository> _materialRepo = null!;
        private Mock<IOrderRepository> _orderRepo = null!;
        private Mock<IPartnerRelationRepository> _partnerRelationRepo = null!;
        private Mock<IMapper> _mapper = null!;

        private InvoiceService _sut = null!;

        [SetUp]
        public void SetUp()
        {
            _invoiceRepo = new Mock<IInvoiceRepository>(MockBehavior.Strict);
            _materialRepo = new Mock<IMaterialRepository>(MockBehavior.Strict);
            _orderRepo = new Mock<IOrderRepository>(MockBehavior.Strict);
            _partnerRelationRepo = new Mock<IPartnerRelationRepository>(MockBehavior.Strict);
            _mapper = new Mock<IMapper>(MockBehavior.Strict);

            _sut = new InvoiceService(
                _invoiceRepo.Object,
                _materialRepo.Object,
                _partnerRelationRepo.Object,
                _orderRepo.Object,
                _mapper.Object
            );
        }

        // -----------------------------
        // CreateInvoice(CreateInvoiceDto)
        // -----------------------------
        [Test]
        public void CreateInvoice_WhenInvoiceCodeExists_Throws()
        {
            var dto = new CreateInvoiceDto
            {
                InvoiceCode = "INV-X",
                Details = new()
                {
                    new() { MaterialId = 1, Quantity = 1, UnitPrice = 10m }
                }
            };

            _invoiceRepo.Setup(r => r.GetByCode("INV-X"))
                        .Returns(new Invoice());

            var ex = Assert.Throws<Exception>(() => _sut.CreateInvoice(dto));
            Assert.That(ex!.Message, Is.EqualTo(InvoiceMessages.INVOICE_CODE_EXISTS));

            _invoiceRepo.VerifyAll();
            _materialRepo.VerifyNoOtherCalls();
            _orderRepo.VerifyNoOtherCalls();
            _partnerRelationRepo.VerifyNoOtherCalls();
            _mapper.VerifyNoOtherCalls();
        }

        [Test]
        public void CreateInvoice_WhenMaterialNotFound_ThrowsFormattedMessage()
        {
            var dto = new CreateInvoiceDto
            {
                InvoiceCode = "INV-NEW",
                Details = new()
                {
                    new() { MaterialId = 999, Quantity = 2, UnitPrice = 100m }
                }
            };

            _invoiceRepo.Setup(r => r.GetByCode("INV-NEW"))
                        .Returns((Invoice?)null);

            _materialRepo.Setup(r => r.GetById(999))
                         .Returns((Material?)null);

            var ex = Assert.Throws<Exception>(() => _sut.CreateInvoice(dto));
            Assert.That(ex!.Message, Is.EqualTo(string.Format(InvoiceMessages.MATERIAL_NOT_FOUND, 999)));

            _invoiceRepo.VerifyAll();
            _materialRepo.VerifyAll();
            _orderRepo.VerifyNoOtherCalls();
            _partnerRelationRepo.VerifyNoOtherCalls();
            _mapper.VerifyNoOtherCalls();
        }

        [Test]
        public void CreateInvoice_Success_AddsInvoiceDetails_ComputesTotal_AndCallsAdd()
        {
            var dto = new CreateInvoiceDto
            {
                InvoiceCode = "INV-OK",
                InvoiceType = "AnyType",
                PartnerId = 10,
                CreatedBy = 77,
                IssueDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(7),
                Details = new()
                {
                    new() { MaterialId = 1, Quantity = 2, UnitPrice = 100m }, // 200
                    new() { MaterialId = 2, Quantity = 1, UnitPrice = 50m }   // 50
                }
            };

            _invoiceRepo.Setup(r => r.GetByCode("INV-OK"))
                        .Returns((Invoice?)null);

            _materialRepo.Setup(r => r.GetById(1)).Returns(new Material { MaterialId = 1 });
            _materialRepo.Setup(r => r.GetById(2)).Returns(new Material { MaterialId = 2 });

            Invoice? added = null;
            _invoiceRepo.Setup(r => r.Add(It.IsAny<Invoice>()))
                        .Callback<Invoice>(i => added = i);

            var result = _sut.CreateInvoice(dto);

            Assert.That(result, Is.Not.Null);
            Assert.That(added, Is.SameAs(result));
            Assert.That(result.InvoiceCode, Is.EqualTo("INV-OK"));
            Assert.That(result.ExportStatus, Is.EqualTo(StatusEnum.Pending.ToStatusString()));
            Assert.That(result.ImportStatus, Is.EqualTo(StatusEnum.Pending.ToStatusString()));
            Assert.That(result.InvoiceDetails.Count, Is.EqualTo(2));
            Assert.That(result.TotalAmount, Is.EqualTo(250m));

            _invoiceRepo.VerifyAll();
            _materialRepo.VerifyAll();
            _orderRepo.VerifyNoOtherCalls();
            _partnerRelationRepo.VerifyNoOtherCalls();
            _mapper.VerifyNoOtherCalls();
        }

        // -----------------------------------------
        // CreateInvoiceFromOrder(CreateInvoiceFromOrderDto)
        // -----------------------------------------
        [Test]
        public void CreateInvoiceFromOrder_WhenOrderNotFound_Throws()
        {
            var dto = new CreateInvoiceFromOrderDto { OrderCode = "ORD-404" };

            _orderRepo.Setup(r => r.GetByCode("ORD-404"))
                      .Returns((Order?)null);

            var ex = Assert.Throws<Exception>(() => _sut.CreateInvoiceFromOrder(dto));
            Assert.That(ex!.Message, Is.EqualTo(InvoiceMessages.ORDER_NOT_FOUND));

            _orderRepo.VerifyAll();
            _invoiceRepo.VerifyNoOtherCalls();
            _materialRepo.VerifyNoOtherCalls();
            _partnerRelationRepo.VerifyNoOtherCalls();
            _mapper.VerifyNoOtherCalls();
        }

        [Test]
        public void CreateInvoiceFromOrder_WhenOrderStatusNotApprovedOrProcessing_Throws()
        {
            var dto = new CreateInvoiceFromOrderDto { OrderCode = "ORD-1" };

            var order = new Order
            {
                OrderCode = "ORD-1",
                Status = "Draft",
                // FIX: ICollection<OrderDetail> -> concrete List<OrderDetail>
                OrderDetails = new List<OrderDetail>()
            };

            _orderRepo.Setup(r => r.GetByCode("ORD-1"))
                      .Returns(order);

            var ex = Assert.Throws<Exception>(() => _sut.CreateInvoiceFromOrder(dto));
            Assert.That(ex!.Message, Is.EqualTo(InvoiceMessages.ORDER_NOT_APPROVED));

            _orderRepo.VerifyAll();
            _invoiceRepo.VerifyNoOtherCalls();
            _materialRepo.VerifyNoOtherCalls();
            _partnerRelationRepo.VerifyNoOtherCalls();
            _mapper.VerifyNoOtherCalls();
        }

        [Test]
        public void CreateInvoiceFromOrder_WhenPartnerIdMissing_Throws()
        {
            var dto = new CreateInvoiceFromOrderDto
            {
                OrderCode = "ORD-2",
                UnitPrices = new()
                {
                    new() { MaterialId = 1, DeliveredQuantity = 1 }
                }
            };

            var order = new Order
            {
                OrderCode = "ORD-2",
                Status = StatusEnum.Approved.ToStatusString(),
                CreatedByNavigation = null,
                // FIX
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail { MaterialId = 1, Quantity = 5 }
                }
            };

            _orderRepo.Setup(r => r.GetByCode("ORD-2"))
                      .Returns(order);

            var ex = Assert.Throws<Exception>(() => _sut.CreateInvoiceFromOrder(dto));
            Assert.That(ex!.Message, Is.EqualTo(InvoiceMessages.PARTNER_NOT_FOUND));

            _orderRepo.VerifyAll();
            _invoiceRepo.VerifyNoOtherCalls();
            _partnerRelationRepo.VerifyNoOtherCalls();
            _mapper.VerifyNoOtherCalls();
            _materialRepo.VerifyNoOtherCalls();
        }

        [Test]
        public void CreateInvoiceFromOrder_WhenNoUnitPricesProvided_Throws()
        {
            var dto = new CreateInvoiceFromOrderDto
            {
                OrderCode = "ORD-3",
                UnitPrices = null
            };

            var order = new Order
            {
                OrderCode = "ORD-3",
                Status = StatusEnum.Approved.ToStatusString(),
                CreatedByNavigation = new() { PartnerId = 10 },
                // FIX
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail { MaterialId = 1, Quantity = 5 }
                }
            };

            _orderRepo.Setup(r => r.GetByCode("ORD-3"))
                      .Returns(order);

            var ex = Assert.Throws<Exception>(() => _sut.CreateInvoiceFromOrder(dto));
            Assert.That(ex!.Message, Is.EqualTo(InvoiceMessages.NO_MATERIAL_PROVIDED));

            _orderRepo.VerifyAll();
            _invoiceRepo.VerifyNoOtherCalls();
            _partnerRelationRepo.VerifyNoOtherCalls();
            _mapper.VerifyNoOtherCalls();
            _materialRepo.VerifyNoOtherCalls();
        }

        [Test]
        public void CreateInvoiceFromOrder_WhenNoMatchingMaterials_Throws()
        {
            var dto = new CreateInvoiceFromOrderDto
            {
                OrderCode = "ORD-4",
                UnitPrices = new()
                {
                    new() { MaterialId = 999, DeliveredQuantity = 1 }
                }
            };

            var order = new Order
            {
                OrderCode = "ORD-4",
                Status = StatusEnum.Approved.ToStatusString(),
                CreatedByNavigation = new() { PartnerId = 10 },
                // FIX
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail { MaterialId = 1, Quantity = 5 }
                }
            };

            _orderRepo.Setup(r => r.GetByCode("ORD-4"))
                      .Returns(order);

            var ex = Assert.Throws<Exception>(() => _sut.CreateInvoiceFromOrder(dto));
            Assert.That(ex!.Message, Is.EqualTo(InvoiceMessages.NO_MATCHING_MATERIALS));

            _orderRepo.VerifyAll();
            _invoiceRepo.VerifyNoOtherCalls();
            _partnerRelationRepo.VerifyNoOtherCalls();
            _mapper.VerifyNoOtherCalls();
            _materialRepo.VerifyNoOtherCalls();
        }

        [Test]
        public void CreateInvoiceFromOrder_WhenDeliveredQtyExceedsOrder_ThrowsFormatted()
        {
            var dto = new CreateInvoiceFromOrderDto
            {
                OrderCode = "ORD-5",
                CreatedBy = 88,
                IssueDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(10),
                UnitPrices = new()
                {
                    new() { MaterialId = 1, DeliveredQuantity = 10 } // exceed
                }
            };

            var order = new Order
            {
                OrderCode = "ORD-5",
                Status = StatusEnum.Approved.ToStatusString(),
                CreatedByNavigation = new() { PartnerId = 10 },
                WarehouseId = 1,
                OrderId = 123,
                DeliveryAddress = "Addr",
                // FIX
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        MaterialId = 1,
                        Quantity = 5,
                        DeliveredQuantity = 0,
                        UnitPrice = 100m,
                        FinalPrice = 90m,
                        Status = "Any"
                    }
                }
            };

            _orderRepo.Setup(r => r.GetByCode("ORD-5")).Returns(order);

            _invoiceRepo.Setup(r => r.GetAll())
                        .Returns(new List<Invoice> { new() { InvoiceCode = "INV-001" } });

            _invoiceRepo.Setup(r => r.GetAllWithDetails())
                        .Returns(new List<Invoice>());

            _partnerRelationRepo.Setup(r => r.GetRelation(It.IsAny<int>(), It.IsAny<int>()))
                                .Returns((PartnerRelation?)null);

            var ex = Assert.Throws<Exception>(() => _sut.CreateInvoiceFromOrder(dto));
            Assert.That(ex!.Message, Is.EqualTo(string.Format(InvoiceMessages.DELIVERED_QTY_EXCEEDS_ORDER, 1)));

            _orderRepo.VerifyAll();
            _invoiceRepo.VerifyAll();
            _partnerRelationRepo.VerifyAll();
            _mapper.VerifyNoOtherCalls();
            _materialRepo.VerifyNoOtherCalls();
        }

        [Test]
        public void CreateInvoiceFromOrder_Success_CreatesInvoice_ComputesTotals_UpdatesOrderAndDetails()
        {
            var dto = new CreateInvoiceFromOrderDto
            {
                OrderCode = "ORD-OK",
                CreatedBy = 500,
                IssueDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(5),
                UnitPrices = new()
                {
                    new() { MaterialId = 1, DeliveredQuantity = 2 },
                    new() { MaterialId = 2, DeliveredQuantity = 1 }
                }
            };

            var order = new Order
            {
                OrderCode = "ORD-OK",
                Status = StatusEnum.Approved.ToStatusString(),
                CreatedByNavigation = new() { PartnerId = 10 },
                WarehouseId = 7,
                OrderId = 777,
                DeliveryAddress = "Delivery",
                // FIX
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        MaterialId = 1,
                        Quantity = 2,
                        DeliveredQuantity = 0,
                        UnitPrice = 100m,
                        FinalPrice = 90m,
                        Status = "Old"
                    },
                    new OrderDetail
                    {
                        MaterialId = 2,
                        Quantity = 1,
                        DeliveredQuantity = 0,
                        UnitPrice = 50m,
                        FinalPrice = 50m,
                        Status = "Old"
                    }
                }
            };

            _orderRepo.Setup(r => r.GetByCode("ORD-OK")).Returns(order);

            _invoiceRepo.Setup(r => r.GetAll())
                        .Returns(new List<Invoice> { new() { InvoiceCode = "INV-005" } });

            _invoiceRepo.Setup(r => r.GetAllWithDetails())
                        .Returns(new List<Invoice> { new() { InvoiceId = 999, InvoiceCode = "INV-005" } });

            _partnerRelationRepo.Setup(r => r.GetRelation(It.IsAny<int>(), It.IsAny<int>()))
                                .Returns((PartnerRelation?)null);

            Invoice? added = null;
            _invoiceRepo.Setup(r => r.Add(It.IsAny<Invoice>()))
                        .Callback<Invoice>(i => added = i);

            _orderRepo.Setup(r => r.Update(order));

            var created = _sut.CreateInvoiceFromOrder(dto);

            Assert.That(created, Has.Count.EqualTo(1));
            var invoice = created[0];

            Assert.That(added, Is.SameAs(invoice));
            Assert.That(invoice.InvoiceCode, Is.EqualTo("INV-006"));
            Assert.That(invoice.InvoiceType, Is.EqualTo(StatusEnum.Export.ToStatusString()));
            Assert.That(invoice.ExportStatus, Is.EqualTo(StatusEnum.Pending.ToStatusString()));
            Assert.That(invoice.ImportStatus, Is.EqualTo(StatusEnum.Pending.ToStatusString()));
            Assert.That(invoice.Address, Is.EqualTo("Delivery"));

            // total= (2*90) + (1*50) = 230
            // discount = (100-90)*2 + (50-50)*1 = 20
            Assert.That(invoice.InvoiceDetails.Count, Is.EqualTo(2));
            Assert.That(invoice.TotalAmount, Is.EqualTo(230m));
            Assert.That(invoice.DiscountAmount, Is.EqualTo(20m));
            Assert.That(invoice.PayableAmount, Is.EqualTo(210m));

            Assert.That(order.OrderDetails.First(od => od.MaterialId == 1).DeliveredQuantity, Is.EqualTo(2));
            Assert.That(order.OrderDetails.First(od => od.MaterialId == 2).DeliveredQuantity, Is.EqualTo(1));

            Assert.That(order.OrderDetails.All(od => od.Status == StatusEnum.Invoiced.ToStatusString()), Is.True);
            Assert.That(order.Status, Is.EqualTo(StatusEnum.Processing.ToStatusString()));

            _orderRepo.VerifyAll();
            _invoiceRepo.VerifyAll();
            _partnerRelationRepo.VerifyAll();
            _mapper.VerifyNoOtherCalls();
            _materialRepo.VerifyNoOtherCalls();
        }

        // -----------------------------
        // UpdateExportStatus / UpdateImportStatus
        // -----------------------------
        [Test]
        public void UpdateExportStatus_WhenInvoiceNotFound_Throws()
        {
            _invoiceRepo.Setup(r => r.GetByIdWithDetails(1)).Returns((Invoice?)null);

            var ex = Assert.Throws<Exception>(() => _sut.UpdateExportStatus(1, "New"));
            Assert.That(ex!.Message, Is.EqualTo(InvoiceMessages.INVOICE_NOT_FOUND));

            _invoiceRepo.VerifyAll();
        }

        [Test]
        public void UpdateExportStatus_Success_UpdatesAndCallsRepoUpdate()
        {
            var inv = new Invoice { InvoiceId = 1, ExportStatus = "Old" };

            _invoiceRepo.Setup(r => r.GetByIdWithDetails(1)).Returns(inv);
            _invoiceRepo.Setup(r => r.Update(inv));

            var result = _sut.UpdateExportStatus(1, "Approved");

            Assert.That(result, Is.SameAs(inv));
            Assert.That(inv.ExportStatus, Is.EqualTo("Approved"));
            Assert.That(inv.UpdatedAt, Is.Not.Null);

            _invoiceRepo.VerifyAll();
        }

        [Test]
        public void UpdateImportStatus_WhenInvoiceNotFound_Throws()
        {
            _invoiceRepo.Setup(r => r.GetByIdWithDetails(2)).Returns((Invoice?)null);

            var ex = Assert.Throws<Exception>(() => _sut.UpdateImportStatus(2, "New"));
            Assert.That(ex!.Message, Is.EqualTo(InvoiceMessages.INVOICE_NOT_FOUND));

            _invoiceRepo.VerifyAll();
        }

        [Test]
        public void UpdateImportStatus_Success_UpdatesAndCallsRepoUpdate()
        {
            var inv = new Invoice { InvoiceId = 2, ImportStatus = "Old" };

            _invoiceRepo.Setup(r => r.GetByIdWithDetails(2)).Returns(inv);
            _invoiceRepo.Setup(r => r.Update(inv));

            var result = _sut.UpdateImportStatus(2, "Delivered");

            Assert.That(result, Is.SameAs(inv));
            Assert.That(inv.ImportStatus, Is.EqualTo("Delivered"));
            Assert.That(inv.UpdatedAt, Is.Not.Null);

            _invoiceRepo.VerifyAll();
        }

        // -----------------------------
        // MarkInvoicesAsDelivered
        // -----------------------------
        [Test]
        public void MarkInvoicesAsDelivered_WhenNullOrEmpty_ThrowsInvalidRequest()
        {
            var ex1 = Assert.Throws<Exception>(() => _sut.MarkInvoicesAsDelivered(null!));
            Assert.That(ex1!.Message, Is.EqualTo(InvoiceMessages.INVALID_REQUEST));

            var ex2 = Assert.Throws<Exception>(() => _sut.MarkInvoicesAsDelivered(new List<int>()));
            Assert.That(ex2!.Message, Is.EqualTo(InvoiceMessages.INVALID_REQUEST));

            _invoiceRepo.VerifyNoOtherCalls();
        }

        [Test]
        public void MarkInvoicesAsDelivered_WhenInvoiceNotFound_Throws()
        {
            _invoiceRepo.Setup(r => r.GetById(1)).Returns((Invoice?)null);

            var ex = Assert.Throws<Exception>(() => _sut.MarkInvoicesAsDelivered(new List<int> { 1 }));
            Assert.That(ex!.Message, Is.EqualTo(InvoiceMessages.INVOICE_NOT_FOUND));

            _invoiceRepo.VerifyAll();
        }

        [Test]
        public void MarkInvoicesAsDelivered_SetsImportStatusDelivered_AndUpdatesEach()
        {
            var inv1 = new Invoice { InvoiceId = 1, ImportStatus = "Old" };
            var inv2 = new Invoice { InvoiceId = 2, ImportStatus = "Old" };

            _invoiceRepo.Setup(r => r.GetById(1)).Returns(inv1);
            _invoiceRepo.Setup(r => r.GetById(2)).Returns(inv2);
            _invoiceRepo.Setup(r => r.Update(inv1));
            _invoiceRepo.Setup(r => r.Update(inv2));

            _sut.MarkInvoicesAsDelivered(new List<int> { 1, 2 });

            Assert.That(inv1.ImportStatus, Is.EqualTo(StatusEnum.Delivered.ToStatusString()));
            Assert.That(inv2.ImportStatus, Is.EqualTo(StatusEnum.Delivered.ToStatusString()));
            Assert.That(inv1.UpdatedAt, Is.Not.Null);
            Assert.That(inv2.UpdatedAt, Is.Not.Null);

            _invoiceRepo.VerifyAll();
        }

        // -----------------------------
        // GetInvoiceForPartner
        // -----------------------------
        [Test]
        public void GetInvoiceForPartner_WhenInvoiceNotFound_Throws()
        {
            _invoiceRepo.Setup(r => r.GetByIdWithDetails(10)).Returns((Invoice?)null);

            var ex = Assert.Throws<Exception>(() => _sut.GetInvoiceForPartner(10, 1));
            Assert.That(ex!.Message, Is.EqualTo(InvoiceMessages.INVOICE_NOT_FOUND));

            _invoiceRepo.VerifyAll();
        }

        [Test]
        public void GetInvoiceForPartner_WhenCurrentPartnerIsExporter_ReturnsExportTypeAndExportStatus()
        {
            var inv = new Invoice
            {
                InvoiceId = 10,
                InvoiceCode = "INV-010",
                PartnerId = 99,
                IssueDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(1),
                TotalAmount = 123,
                WarehouseId = 1,
                ExportStatus = "EXP",
                ImportStatus = "IMP",
                CreatedAt = DateTime.Today.AddDays(-1),
                CreatedByNavigation = new() { PartnerId = 5 },
                Partner = new() { PartnerName = "P" },
                Warehouse = new() { WarehouseName = "W" }
            };

            _invoiceRepo.Setup(r => r.GetByIdWithDetails(10)).Returns(inv);

            var dto = _sut.GetInvoiceForPartner(10, currentPartnerId: 5);

            Assert.That(dto.InvoiceType, Is.EqualTo("Export"));
            Assert.That(dto.Status, Is.EqualTo("EXP"));

            _invoiceRepo.VerifyAll();
        }

        [Test]
        public void GetInvoiceForPartner_WhenCurrentPartnerIsNotExporter_ReturnsImportTypeAndImportStatus()
        {
            var inv = new Invoice
            {
                InvoiceId = 10,
                InvoiceCode = "INV-010",
                PartnerId = 99,
                ExportStatus = "EXP",
                ImportStatus = "IMP",
                CreatedByNavigation = new() { PartnerId = 5 }
            };

            _invoiceRepo.Setup(r => r.GetByIdWithDetails(10)).Returns(inv);

            var dto = _sut.GetInvoiceForPartner(10, currentPartnerId: 999);

            Assert.That(dto.InvoiceType, Is.EqualTo("Import"));
            Assert.That(dto.Status, Is.EqualTo("IMP"));

            _invoiceRepo.VerifyAll();
        }

        // -----------------------------
        // GetAllInvoicesForPartnerOrManager
        // -----------------------------
        [Test]
        public void GetAllInvoicesForPartnerOrManager_FiltersByPartnerId_AndMapsInvoiceTypeAndStatus()
        {
            int partnerId = 10;

            var list = new List<Invoice>
            {
                new()
                {
                    InvoiceId = 1,
                    InvoiceCode = "A",
                    PartnerId = 999,
                    CreatedByNavigation = new() { PartnerId = partnerId },
                    ExportStatus = "EXP1",
                    ImportStatus = "IMP1",
                    Partner = new() { PartnerName = "P1" },
                    Warehouse = new() { WarehouseName = "W1" }
                },
                new()
                {
                    InvoiceId = 2,
                    InvoiceCode = "B",
                    PartnerId = partnerId,
                    CreatedByNavigation = new() { PartnerId = 777 },
                    ExportStatus = "EXP2",
                    ImportStatus = "IMP2",
                    Partner = new() { PartnerName = "P2" },
                    Warehouse = new() { WarehouseName = "W2" }
                },
                new()
                {
                    InvoiceId = 3,
                    InvoiceCode = "C",
                    PartnerId = 123,
                    CreatedByNavigation = new() { PartnerId = 456 }
                }
            };

            _invoiceRepo.Setup(r => r.GetAllWithDetails()).Returns(list);

            var result = _sut.GetAllInvoicesForPartnerOrManager(partnerId: partnerId, managerId: null);

            Assert.That(result, Has.Count.EqualTo(2));

            var first = result.Single(r => r.InvoiceId == 1);
            Assert.That(first.InvoiceType, Is.EqualTo("Export"));
            Assert.That(first.Status, Is.EqualTo("EXP1"));

            var second = result.Single(r => r.InvoiceId == 2);
            Assert.That(second.InvoiceType, Is.EqualTo("Import"));
            Assert.That(second.Status, Is.EqualTo("IMP2"));

            _invoiceRepo.VerifyAll();
        }

        [Test]
        public void GetAllInvoicesForPartnerOrManager_FiltersByManagerId()
        {
            int managerId = 55;

            var list = new List<Invoice>
            {
                new() { InvoiceId = 1, Warehouse = new() { ManagerId = managerId } },
                new() { InvoiceId = 2, Warehouse = new() { ManagerId = 999 } },
                new() { InvoiceId = 3, Warehouse = null }
            };

            _invoiceRepo.Setup(r => r.GetAllWithDetails()).Returns(list);

            var result = _sut.GetAllInvoicesForPartnerOrManager(partnerId: null, managerId: managerId);

            Assert.That(result.Select(x => x.InvoiceId).ToList(), Is.EqualTo(new List<int> { 1 }));

            _invoiceRepo.VerifyAll();
        }

        // -----------------------------
        // RejectInvoice
        // -----------------------------
        [Test]
        public void RejectInvoice_WhenInvoiceNotFound_ReturnsNull()
        {
            _invoiceRepo.Setup(r => r.GetByIdWithDetails(1)).Returns((Invoice?)null);

            var result = _sut.RejectInvoice(1);

            Assert.That(result, Is.Null);
            _invoiceRepo.VerifyAll();
        }

        [Test]
        public void RejectInvoice_WhenFound_SetsBothStatusesRejected_AndUpdates()
        {
            var inv = new Invoice { InvoiceId = 1, ExportStatus = "E", ImportStatus = "I" };

            _invoiceRepo.Setup(r => r.GetByIdWithDetails(1)).Returns(inv);
            _invoiceRepo.Setup(r => r.Update(inv));

            var result = _sut.RejectInvoice(1);

            Assert.That(result, Is.SameAs(inv));
            Assert.That(inv.ExportStatus, Is.EqualTo(StatusEnum.Rejected.ToStatusString()));
            Assert.That(inv.ImportStatus, Is.EqualTo(StatusEnum.Rejected.ToStatusString()));
            Assert.That(inv.UpdatedAt, Is.Not.Null);

            _invoiceRepo.VerifyAll();
        }

        // -----------------------------
        // Pass-through methods
        // -----------------------------
        [Test]
        public void GetByIdWithDetails_ReturnsRepoValue()
        {
            var inv = new Invoice { InvoiceId = 1 };

            _invoiceRepo.Setup(r => r.GetByIdWithDetails(1)).Returns(inv);

            var result = _sut.GetByIdWithDetails(1);

            Assert.That(result, Is.SameAs(inv));
            _invoiceRepo.VerifyAll();
        }

        [Test]
        public void GetAllWithDetails_ReturnsRepoValue()
        {
            var list = new List<Invoice> { new() { InvoiceId = 1 } };

            _invoiceRepo.Setup(r => r.GetAllWithDetails()).Returns(list);

            var result = _sut.GetAllWithDetails();

            Assert.That(result, Is.SameAs(list));
            _invoiceRepo.VerifyAll();
        }

        [Test]
        public void GetInvoiceSeller_ReturnsRepoValue()
        {
            var list = new List<Invoice> { new() { InvoiceId = 1 } };

            _invoiceRepo.Setup(r => r.GetInvoiceSeller(10)).Returns(list);

            var result = _sut.GetInvoiceSeller(10);

            Assert.That(result, Is.SameAs(list));
            _invoiceRepo.VerifyAll();
        }

        [Test]
        public void GetInvoiceBuyer_ReturnsRepoValue()
        {
            var list = new List<Invoice> { new() { InvoiceId = 2 } };

            _invoiceRepo.Setup(r => r.GetInvoiceBuyer(20)).Returns(list);

            var result = _sut.GetInvoiceBuyer(20);

            Assert.That(result, Is.SameAs(list));
            _invoiceRepo.VerifyAll();
        }

        // -----------------------------
        // GetPendingInvoicesBySellerPartner
        // -----------------------------
        [Test]
        public void GetPendingInvoicesBySellerPartner_WhenInvalidSellerPartnerId_Throws()
        {
            var ex = Assert.Throws<Exception>(() => _sut.GetPendingInvoicesBySellerPartner(0));
            Assert.That(ex!.Message, Is.EqualTo(InvoiceMessages.INVALID_REQUEST));

            _invoiceRepo.VerifyNoOtherCalls();
            _mapper.VerifyNoOtherCalls();
        }

        [Test]
        public void GetPendingInvoicesBySellerPartner_Success_CallsRepoAndMaps()
        {
            int sellerPartnerId = 10;

            var invoices = new List<Invoice>
            {
                new() { InvoiceId = 1 },
                new() { InvoiceId = 2 }
            };

            var mapped = new List<InvoiceDto>
            {
                new() { InvoiceId = 1 },
                new() { InvoiceId = 2 }
            };

            _invoiceRepo.Setup(r => r.GetPendingInvoicesBySellerPartner(sellerPartnerId))
                        .Returns(invoices);

            _mapper.Setup(m => m.Map<List<InvoiceDto>>(invoices))
                   .Returns(mapped);

            var result = _sut.GetPendingInvoicesBySellerPartner(sellerPartnerId);

            Assert.That(result, Is.SameAs(mapped));

            _invoiceRepo.VerifyAll();
            _mapper.VerifyAll();
        }
    }
}
