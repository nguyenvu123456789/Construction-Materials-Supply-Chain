using System;
using System.Collections.Generic;
using System.Linq;
using Application.Constants.Enums;
using Application.DTOs;
using Domain.Interface;
using Domain.Models;
using Moq;
using NUnit.Framework;
using Services.Implementations;

// ✅ Fix ambiguity for Messages
using ImportMsgs = Application.Constants.Messages.ImportMessages;

namespace Services.Tests
{
    [TestFixture]
    public class ImportServiceTests
    {
        private Mock<IImportRepository> _imports = null!;
        private Mock<IInvoiceRepository> _invoices = null!;
        private Mock<IInventoryRepository> _inventories = null!;
        private Mock<IImportDetailRepository> _importDetails = null!;
        private Mock<IMaterialRepository> _materials = null!;
        private Mock<IOrderRepository> _orders = null!;
        private Mock<IOrderDetailRepository> _orderDetails = null!;
        private Mock<IMaterialPartnerRepository> _materialPartners = null!;

        private ImportService _sut = null!;

        [SetUp]
        public void SetUp()
        {
            _imports = new Mock<IImportRepository>(MockBehavior.Strict);
            _invoices = new Mock<IInvoiceRepository>(MockBehavior.Strict);
            _inventories = new Mock<IInventoryRepository>(MockBehavior.Strict);
            _importDetails = new Mock<IImportDetailRepository>(MockBehavior.Strict);
            _materials = new Mock<IMaterialRepository>(MockBehavior.Strict);
            _orders = new Mock<IOrderRepository>(MockBehavior.Strict);
            _orderDetails = new Mock<IOrderDetailRepository>(MockBehavior.Strict);
            _materialPartners = new Mock<IMaterialPartnerRepository>(MockBehavior.Strict);

            _sut = new ImportService(
                _imports.Object,
                _invoices.Object,
                _inventories.Object,
                _importDetails.Object,
                _materials.Object,
                _materialPartners.Object,
                _orderDetails.Object,
                _orders.Object
            );
        }

        // --------------------
        // Helpers
        // --------------------
        private static Material Mat(int id, string code = "M", string name = "Material", string? unit = "pcs")
            => new Material { MaterialId = id, MaterialCode = code, MaterialName = name, Unit = unit };

        private static Inventory Inv(int wh, int matId, decimal? qty)
            => new Inventory { WarehouseId = wh, MaterialId = matId, Quantity = qty };

        private static InvoiceDetail InvDetail(int matId, int qty, decimal unitPrice)
            => new InvoiceDetail { MaterialId = matId, Quantity = qty, UnitPrice = unitPrice };

        private static Invoice MakeInvoice(string code, string importStatus, int partnerId, int createdBy, int orderId, params InvoiceDetail[] details)
            => new Invoice
            {
                InvoiceId = 1,
                InvoiceCode = code,
                ImportStatus = importStatus,
                PartnerId = partnerId,
                CreatedBy = createdBy,
                OrderId = orderId,
                InvoiceDetails = details.ToList()
            };

        // =========================================================
        // CreateImportFromInvoice (main method)
        // =========================================================

        [Test]
        public void CreateImportFromInvoice_WhenBothInvoiceCodeAndImportCodeMissing_Throws()
        {
            var ex = Assert.Throws<Exception>(() =>
                _sut.CreateImportFromInvoice(null, null, warehouseId: 1, createdBy: 2, notes: "n"));

            Assert.That(ex!.Message, Is.EqualTo(ImportMsgs.MSG_MISSING_INVOICE_OR_IMPORT));
        }

        [Test]
        public void CreateImportFromInvoice_WhenInvoiceNotFound_Throws()
        {
            _invoices.Setup(r => r.GetByCodeNoTracking("INV-404")).Returns((Invoice?)null);

            var ex = Assert.Throws<Exception>(() =>
                _sut.CreateImportFromInvoice(null, "INV-404", 1, 2, "n"));

            Assert.That(ex!.Message, Is.EqualTo(ImportMsgs.MSG_INVOICE_NOT_FOUND));
            _invoices.VerifyAll();
        }

        [Test]
        public void CreateImportFromInvoice_WhenInvoiceAlreadyImported_Throws()
        {
            var inv = MakeInvoice("INV-1", importStatus: ImportStatus.Success.ToString(),
                partnerId: 10, createdBy: 99, orderId: 5,
                InvDetail(1, 1, 10));

            _invoices.Setup(r => r.GetByCodeNoTracking("INV-1")).Returns(inv);

            var ex = Assert.Throws<Exception>(() =>
                _sut.CreateImportFromInvoice(null, "INV-1", 1, 2, "n"));

            Assert.That(ex!.Message, Is.EqualTo(ImportMsgs.MSG_INVOICE_ALREADY_IMPORTED));
            _invoices.VerifyAll();
        }

        [Test]
        public void CreateImportFromInvoice_WhenInvoiceNotDelivered_Throws()
        {
            // invoice.ImportStatus != "Delivered"
            var inv = MakeInvoice("INV-1", importStatus: "Pending",
                partnerId: 10, createdBy: 99, orderId: 5,
                InvDetail(1, 1, 10));

            _invoices.Setup(r => r.GetByCodeNoTracking("INV-1")).Returns(inv);

            var ex = Assert.Throws<Exception>(() =>
                _sut.CreateImportFromInvoice(null, "INV-1", 1, 2, "n"));

            Assert.That(ex!.Message, Is.EqualTo(ImportMsgs.MSG_INVOICE_NOT_DELIVERED));
            _invoices.VerifyAll();
        }

        [Test]
        public void CreateImportFromInvoice_WhenMaterialNotFoundInGetByIds_ThrowsFormatted()
        {
            var inv = MakeInvoice("INV-OK", importStatus: StatusEnum.Delivered.ToStatusString(),
                partnerId: 10, createdBy: 99, orderId: 5,
                InvDetail(1, 2, 10));

            _invoices.Setup(r => r.GetByCodeNoTracking("INV-OK")).Returns(inv);

            // GenerateImportCode uses _invoices.GetAll()
            _invoices.Setup(r => r.GetAll()).Returns(new List<Invoice>());

            Import? addedImport = null;
            _imports.Setup(r => r.Add(It.IsAny<Import>()))
                    .Callback<Import>(i => { i.ImportId = 123; addedImport = i; });

            // GetByIds returns empty => TryGetValue false
            _materials.Setup(r => r.GetByIds(It.Is<List<int>>(ids => ids.SequenceEqual(new List<int> { 1 }))))
                      .Returns(new List<Material>());

            var ex = Assert.Throws<Exception>(() =>
                _sut.CreateImportFromInvoice(null, "INV-OK", warehouseId: 1, createdBy: 2, notes: "n"));

            Assert.That(ex!.Message, Is.EqualTo(string.Format(ImportMsgs.MSG_MATERIAL_NOT_FOUND_BY_ID, 1)));

            _invoices.VerifyAll();
            _imports.VerifyAll();
            _materials.VerifyAll();
        }

        [Test]
        public void CreateImportFromInvoice_Success_CreatesImport_AddsDetails_UpdatesInventory_AddsMaterialPartner_UpdatesInvoice_AndOrderStatus()
        {
            // Delivered invoice
            var inv = MakeInvoice("INV-OK", importStatus: StatusEnum.Delivered.ToStatusString(),
                partnerId: 10, createdBy: 99, orderId: 5,
                InvDetail(1, 2, 10),
                InvDetail(2, 1, 20));

            _invoices.Setup(r => r.GetByCodeNoTracking("INV-OK")).Returns(inv);

            // GenerateImportCode uses _invoices.GetAll() and looks for InvoiceCode startswith "IMP-"
            _invoices.Setup(r => r.GetAll()).Returns(new List<Invoice>
            {
                new Invoice { InvoiceCode = "IMP-001" }
            });

            Import? addedImport = null;
            _imports.Setup(r => r.Add(It.IsAny<Import>()))
                    .Callback<Import>(i =>
                    {
                        i.ImportId = 777; // giả lập DB
                        addedImport = i;
                    });

            _materials.Setup(r => r.GetByIds(It.IsAny<List<int>>()))
                      .Returns(new List<Material>
                      {
                          Mat(1, "M1", "Mat1", "kg"),
                          Mat(2, "M2", "Mat2", "kg"),
                      });

            var addedDetails = new List<ImportDetail>();
            _importDetails.Setup(r => r.Add(It.IsAny<ImportDetail>()))
                          .Callback<ImportDetail>(d => addedDetails.Add(d));

            // UpdateInventory: giả lập cả 2 item đều chưa có inventory => Add mới
            _inventories.Setup(r => r.GetByWarehouseAndMaterial(1,1))
                        .Returns((Inventory?)null);
            _inventories.Setup(r => r.GetByWarehouseAndMaterial(1,2))
                        .Returns((Inventory?)null);

            Inventory? addedInv1 = null;
            Inventory? addedInv2 = null;
            _inventories.Setup(r => r.Add(It.IsAny<Inventory>()))
                        .Callback<Inventory>(x =>
                        {
                            if (x.MaterialId == 1) addedInv1 = x;
                            if (x.MaterialId == 2) addedInv2 = x;
                        });

            // MaterialPartner: existingRelation null => Add new
            _materialPartners.Setup(r => r.GetAll()).Returns(new List<MaterialPartner>());
            _materialPartners.Setup(r => r.Add(It.IsAny<MaterialPartner>()));

            // UpdateOrderStatusIfFullyDelivered calls:
            // 1) GetByOrderAndMaterial
            // 2) GetByOrderId
            // 3) (optional) GetById + Update order
            var od1 = new OrderDetail { OrderId = 5, MaterialId = 1, Quantity = 2, DeliveredQuantity = 2, Status = "X" };
            var od2 = new OrderDetail { OrderId = 5, MaterialId = 2, Quantity = 1, DeliveredQuantity = 1, Status = OrderDetailStatus.Success.ToString() };

            _orderDetails.Setup(r => r.GetByOrderAndMaterial(5, 1)).Returns(od1);
            _orderDetails.Setup(r => r.Update(It.IsAny<OrderDetail>()));

            // For materialId=2, assume already success but still call GetByOrderAndMaterial:
            _orderDetails.Setup(r => r.GetByOrderAndMaterial(5, 2)).Returns(od2);

            // After each material, service checks all order details:
            _orderDetails.Setup(r => r.GetByOrderId(5)).Returns(new List<OrderDetail> { od1, od2 });

            var order = new Order { OrderId = 5, Status = "Old" };
            _orders.Setup(r => r.GetById(5)).Returns(order);
            _orders.Setup(r => r.Update(order));

            // Finally update invoice
            _invoices.Setup(r => r.Update(inv));

            var result = _sut.CreateImportFromInvoice(importCode: null, invoiceCode: "INV-OK",
                warehouseId: 1, createdBy: 2, notes: "note");

            Assert.That(result, Is.Not.Null);
            Assert.That(addedImport, Is.SameAs(result));
            Assert.That(result.ImportCode, Is.EqualTo("IMP-002")); // after IMP-001
            Assert.That(result.Status, Is.EqualTo(ImportStatus.Success.ToString()));
            Assert.That(result.CreatedAt, Is.Not.EqualTo(default(DateTime)));
            Assert.That(result.ImportDate, Is.Not.EqualTo(default(DateTime)));

            Assert.That(addedDetails.Count, Is.EqualTo(2));
            Assert.That(addedDetails.All(d => d.ImportId == 777), Is.True);
            Assert.That(addedDetails[0].LineTotal, Is.EqualTo(2 * 10));
            Assert.That(addedDetails[1].LineTotal, Is.EqualTo(1 * 20));

            Assert.That(addedInv1, Is.Not.Null);
            Assert.That(addedInv1!.Quantity, Is.EqualTo(2));
            Assert.That(addedInv2, Is.Not.Null);
            Assert.That(addedInv2!.Quantity, Is.EqualTo(1));

            Assert.That(inv.ImportStatus, Is.EqualTo(ImportStatus.Success.ToString()));
            Assert.That(inv.UpdatedAt, Is.Not.EqualTo(default(DateTime)));

            Assert.That(od1.Status, Is.EqualTo(OrderDetailStatus.Success.ToString()));
            Assert.That(order.Status, Is.EqualTo(StatusEnum.Success.ToStatusString()));
            Assert.That(order.UpdatedAt, Is.Not.EqualTo(default(DateTime)));

            _invoices.VerifyAll();
            _imports.VerifyAll();
            _materials.VerifyAll();
            _importDetails.VerifyAll();
            _inventories.VerifyAll();
            _materialPartners.VerifyAll();
            _orderDetails.VerifyAll();
            _orders.VerifyAll();
        }

        // =========================================================
        // CreateImportFromInvoice (Free import branch - invoiceCode null)
        // =========================================================

        [Test]
        public void CreateImportFromInvoice_FreeImport_WhenExistingImportFound_UpdatesNotesAndReturnsExisting()
        {
            var existing = new Import { ImportId = 1, ImportCode = "IMP-777", Notes = "old", Status = ImportStatus.Success.ToString() };

            _imports.Setup(r => r.GetAll()).Returns(new List<Import> { existing });
            _imports.Setup(r => r.Update(existing));

            var result = _sut.CreateImportFromInvoice(importCode: "IMP-777", invoiceCode: null,
                warehouseId: 1, createdBy: 2, notes: "new");

            Assert.That(result, Is.SameAs(existing));
            Assert.That(existing.Notes, Is.EqualTo("new"));
            Assert.That(existing.UpdatedAt, Is.Not.EqualTo(default(DateTime)));

            _imports.VerifyAll();
        }

        [Test]
        public void CreateImportFromInvoice_FreeImport_WhenNotFound_CreatesNewImport()
        {
            _imports.Setup(r => r.GetAll()).Returns(new List<Import>());

            Import? added = null;
            _imports.Setup(r => r.Add(It.IsAny<Import>()))
                    .Callback<Import>(i => { i.ImportId = 10; added = i; });

            var result = _sut.CreateImportFromInvoice(importCode: "IMP-888", invoiceCode: null,
                warehouseId: 1, createdBy: 2, notes: "note");

            Assert.That(result, Is.Not.Null);
            Assert.That(added, Is.SameAs(result));
            Assert.That(result.ImportCode, Is.EqualTo("IMP-888"));
            Assert.That(result.Status, Is.EqualTo(ImportStatus.Success.ToString()));
            Assert.That(result.CreatedAt, Is.Not.EqualTo(default(DateTime)));

            _imports.VerifyAll();
        }

        // =========================================================
        // CreateImportFromImport
        // =========================================================

        [Test]
        public void CreateImportFromImport_WhenPendingImportNotFound_Throws()
        {
            _imports.Setup(r => r.GetAll()).Returns(new List<Import>());

            var ex = Assert.Throws<Exception>(() => _sut.CreateImportFromImport("IMP-001", "n"));
            Assert.That(ex!.Message, Is.EqualTo(ImportMsgs.MSG_IMPORT_PENDING_NOT_FOUND));

            _imports.VerifyAll();
        }

        [Test]
        public void CreateImportFromImport_WhenDetailsMissing_Throws()
        {
            var imp = new Import { ImportId = 1, ImportCode = "IMP-001", Status = ImportStatus.Pending.ToString(), WarehouseId = 10, Notes = "old" };

            _imports.Setup(r => r.GetAll()).Returns(new List<Import> { imp });
            _importDetails.Setup(r => r.GetByImportId(1)).Returns(new List<ImportDetail>());

            var ex = Assert.Throws<Exception>(() => _sut.CreateImportFromImport("IMP-001", "n"));
            Assert.That(ex!.Message, Is.EqualTo(ImportMsgs.MSG_IMPORT_DETAIL_NOT_FOUND));

            _imports.VerifyAll();
            _importDetails.VerifyAll();
        }

        [Test]
        public void CreateImportFromImport_Success_AddsOrUpdatesInventory_AndMarksImportSuccess()
        {
            var imp = new Import
            {
                ImportId = 1,
                ImportCode = "IMP-001",
                Status = ImportStatus.Pending.ToString(),
                WarehouseId = 10,
                Notes = "old"
            };

            var details = new List<ImportDetail>
            {
                new ImportDetail { ImportId = 1, MaterialId = 101, Quantity = 2 },
                new ImportDetail { ImportId = 1, MaterialId = 102, Quantity = 3 }
            };

            _imports.Setup(r => r.GetAll()).Returns(new List<Import> { imp });
            _importDetails.Setup(r => r.GetByImportId(1)).Returns(details);

            // 101 chưa có => Add
            _inventories.Setup(r => r.GetByWarehouseAndMaterial(10, 101)).Returns((Inventory?)null);
            _inventories.Setup(r => r.Add(It.Is<Inventory>(x => x.MaterialId == 101 && x.Quantity == 2)));

            // 102 có => Update
            var inv102 = Inv(10, 102, qty: 5);
            _inventories.Setup(r => r.GetByWarehouseAndMaterial(10, 102)).Returns(inv102);
            _inventories.Setup(r => r.Update(inv102));

            _imports.Setup(r => r.Update(imp));

            var result = _sut.CreateImportFromImport("IMP-001", "newnote");

            Assert.That(result, Is.SameAs(imp));
            Assert.That(imp.Status, Is.EqualTo(ImportStatus.Success.ToString()));
            Assert.That(imp.Notes, Is.EqualTo("newnote"));
            Assert.That(imp.UpdatedAt, Is.Not.EqualTo(default(DateTime)));

            Assert.That(inv102.Quantity, Is.EqualTo(8)); // 5+3
            Assert.That(inv102.UpdatedAt, Is.Not.EqualTo(default(DateTime)));

            _imports.VerifyAll();
            _importDetails.VerifyAll();
            _inventories.VerifyAll();
        }

        // =========================================================
        // CreateDirectionImport
        // =========================================================

        [Test]
        public void CreateDirectionImport_WhenNoMaterials_Throws()
        {
            var ex = Assert.Throws<Exception>(() =>
                _sut.CreateDirectionImport(warehouseId: 1, createdBy: 2, notes: "n", materials: new List<PendingImportMaterialDto>()));

            Assert.That(ex!.Message, Is.EqualTo(ImportMsgs.MSG_REQUIRE_AT_LEAST_ONE_MATERIAL));
        }

        [Test]
        public void CreateDirectionImport_WhenMaterialNotFound_ThrowsFormatted()
        {
            _invoices.Setup(r => r.GetAll()).Returns(new List<Invoice>()); // GenerateImportCode
            _imports.Setup(r => r.Add(It.IsAny<Import>()))
                    .Callback<Import>(i => i.ImportId = 99);

            _materials.Setup(r => r.GetById(101)).Returns((Material?)null);

            var ex = Assert.Throws<Exception>(() =>
                _sut.CreateDirectionImport(1, 2, "n", new List<PendingImportMaterialDto>
                {
                    new PendingImportMaterialDto { MaterialId = 101, Quantity = 1 }
                }));

            Assert.That(ex!.Message, Is.EqualTo(string.Format(ImportMsgs.MSG_MATERIAL_NOT_FOUND, 101)));

            _invoices.VerifyAll();
            _imports.VerifyAll();
            _materials.VerifyAll();
        }

        [Test]
        public void CreateDirectionImport_Success_AddsImport_AddsDetails_AndUpdatesInventory()
        {
            _invoices.Setup(r => r.GetAll()).Returns(new List<Invoice>()); // GenerateImportCode -> IMP-001

            Import? addedImport = null;
            _imports.Setup(r => r.Add(It.IsAny<Import>()))
                    .Callback<Import>(i => { i.ImportId = 200; addedImport = i; });

            _materials.Setup(r => r.GetById(101)).Returns(Mat(101, "M101", "Mat101", "kg"));
            _materials.Setup(r => r.GetById(102)).Returns(Mat(102, "M102", "Mat102", "kg"));

            var addedDetails = new List<ImportDetail>();
            _importDetails.Setup(r => r.Add(It.IsAny<ImportDetail>()))
                          .Callback<ImportDetail>(d => addedDetails.Add(d));

            // UpdateInventory: 101 exists => Update, 102 null => Add
            var inv101 = Inv(1, 101, qty: 5);
            _inventories.Setup(r => r.GetByWarehouseAndMaterial(1, 101)).Returns(inv101);
            _inventories.Setup(r => r.Update(inv101));

            _inventories.Setup(r => r.GetByWarehouseAndMaterial(1, 102)).Returns((Inventory?)null);
            _inventories.Setup(r => r.Add(It.Is<Inventory>(x => x.WarehouseId == 1 && x.MaterialId == 102 && x.Quantity == 1)));

            var result = _sut.CreateDirectionImport(1, 2, "note", new List<PendingImportMaterialDto>
            {
                new PendingImportMaterialDto{ MaterialId = 101, Quantity = 2 },
                new PendingImportMaterialDto{ MaterialId = 102, Quantity = 1 }
            });

            Assert.That(result, Is.SameAs(addedImport));
            Assert.That(result.ImportCode, Is.EqualTo("IMP-001"));
            Assert.That(result.Status, Is.EqualTo(ImportStatus.Success.ToString()));
            Assert.That(result.CreatedAt, Is.Not.EqualTo(default(DateTime)));

            Assert.That(addedDetails.Count, Is.EqualTo(2));
            Assert.That(addedDetails.All(d => d.ImportId == 200), Is.True);

            Assert.That(inv101.Quantity, Is.EqualTo(7)); // 5+2
            Assert.That(inv101.UpdatedAt, Is.Not.EqualTo(default(DateTime)));

            _invoices.VerifyAll();
            _imports.VerifyAll();
            _materials.VerifyAll();
            _importDetails.VerifyAll();
            _inventories.VerifyAll();
        }

        // =========================================================
        // RejectImport
        // =========================================================

        [Test]
        public void RejectImport_WhenNotFound_ReturnsNull()
        {
            _imports.Setup(r => r.GetByIdWithDetails(1)).Returns((Import?)null);

            var result = _sut.RejectImport(1);

            Assert.That(result, Is.Null);
            _imports.VerifyAll();
        }

        [Test]
        public void RejectImport_WhenNotPending_Throws()
        {
            var imp = new Import { ImportId = 1, Status = ImportStatus.Success.ToString() };
            _imports.Setup(r => r.GetByIdWithDetails(1)).Returns(imp);

            var ex = Assert.Throws<Exception>(() => _sut.RejectImport(1));
            Assert.That(ex!.Message, Is.EqualTo(ImportMsgs.MSG_ONLY_PENDING_CAN_BE_REJECTED));

            _imports.VerifyAll();
        }

        [Test]
        public void RejectImport_WhenPending_SetsRejected_AndUpdates()
        {
            var imp = new Import { ImportId = 1, Status = ImportStatus.Pending.ToString() };
            _imports.Setup(r => r.GetByIdWithDetails(1)).Returns(imp);
            _imports.Setup(r => r.Update(imp));

            var result = _sut.RejectImport(1);

            Assert.That(result, Is.SameAs(imp));
            Assert.That(imp.Status, Is.EqualTo(ImportStatus.Rejected.ToString()));
            Assert.That(imp.UpdatedAt, Is.Not.EqualTo(default(DateTime)));

            _imports.VerifyAll();
        }

        // =========================================================
        // GetById / GetByIdWithDetails / GetAll / GetImports
        // =========================================================

        [Test]
        public void GetById_ReturnsRepoValue()
        {
            var imp = new Import { ImportId = 1 };
            _imports.Setup(r => r.GetById(1)).Returns(imp);

            var result = _sut.GetById(1);

            Assert.That(result, Is.SameAs(imp));
            _imports.VerifyAll();
        }

        [Test]
        public void GetByIdWithDetails_WhenFound_LoadsDetails()
        {
            var imp = new Import { ImportId = 1 };
            _imports.Setup(r => r.GetById(1)).Returns(imp);
            _importDetails.Setup(r => r.GetByImportId(1)).Returns(new List<ImportDetail>
            {
                new ImportDetail{ ImportId = 1, MaterialId = 101, Quantity = 1 }
            });

            var result = _sut.GetByIdWithDetails(1);

            Assert.That(result, Is.SameAs(imp));
            Assert.That(imp.ImportDetails, Is.Not.Null);
            Assert.That(imp.ImportDetails.Count, Is.EqualTo(1));

            _imports.VerifyAll();
            _importDetails.VerifyAll();
        }

        [Test]
        public void GetByIdWithDetails_WhenNotFound_ReturnsNull()
        {
            _imports.Setup(r => r.GetById(99)).Returns((Import?)null);

            var result = _sut.GetByIdWithDetails(99);

            Assert.That(result, Is.Null);
            _imports.VerifyAll();
            _importDetails.VerifyNoOtherCalls();
        }

        [Test]
        public void GetAll_LoadsDetailsForEachImport()
        {
            var i1 = new Import { ImportId = 1 };
            var i2 = new Import { ImportId = 2 };

            _imports.Setup(r => r.GetAll()).Returns(new List<Import> { i1, i2 });
            _importDetails.Setup(r => r.GetByImportId(1)).Returns(new List<ImportDetail> { new ImportDetail { ImportId = 1 } });
            _importDetails.Setup(r => r.GetByImportId(2)).Returns(new List<ImportDetail> { new ImportDetail { ImportId = 2 } });

            var result = _sut.GetAll();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(i1.ImportDetails.Count, Is.EqualTo(1));
            Assert.That(i2.ImportDetails.Count, Is.EqualTo(1));

            _imports.VerifyAll();
            _importDetails.VerifyAll();
        }

        [Test]
        public void GetImports_FiltersByPartnerId_LoadsDetails()
        {
            int partnerId = 10;

            var i1 = new Import
            {
                ImportId = 1,
                Warehouse = new Warehouse
                {
                    Manager = new User { PartnerId = partnerId }
                }
            };

            var i2 = new Import
            {
                ImportId = 2,
                Warehouse = new Warehouse
                {
                    Manager = new User { PartnerId = 999 }
                }
            };

            _imports.Setup(r => r.GetAllWithWarehouse()).Returns(new List<Import> { i1, i2 });

            _importDetails.Setup(r => r.GetByImportId(1)).Returns(new List<ImportDetail> { new ImportDetail { ImportId = 1 } });

            var result = _sut.GetImports(partnerId: partnerId, managerId: null);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].ImportId, Is.EqualTo(1));
            Assert.That(i1.ImportDetails, Is.Not.Null);

            _imports.VerifyAll();
            _importDetails.VerifyAll();
        }

        [Test]
        public void GetImports_FiltersByManagerId_LoadsDetails()
        {
            int managerId = 77;

            var i1 = new Import { ImportId = 1, Warehouse = new Warehouse { ManagerId = managerId } };
            var i2 = new Import { ImportId = 2, Warehouse = new Warehouse { ManagerId = 999 } };

            _imports.Setup(r => r.GetAllWithWarehouse()).Returns(new List<Import> { i1, i2 });

            _importDetails.Setup(r => r.GetByImportId(1)).Returns(new List<ImportDetail> { new ImportDetail { ImportId = 1 } });

            var result = _sut.GetImports(partnerId: null, managerId: managerId);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].ImportId, Is.EqualTo(1));

            _imports.VerifyAll();
            _importDetails.VerifyAll();
        }
    }
}
