using System;
using System.Collections.Generic;
using System.Linq;
using Application.Constants.Enums;
using Application.DTOs;
using AutoMapper;
using Domain.Interface;
using Domain.Models;
using Moq;
using NUnit.Framework;
using Services.Implementations;

// ✅ Fix ambiguity for Messages (đổi namespace cho đúng nếu bạn để khác)
using ExportMsgs = Application.Constants.Messages.ExportMessages;

namespace Services.Tests
{
    [TestFixture]
    public class ExportServiceTests
    {
        private Mock<IExportRepository> _exports = null!;
        private Mock<IExportDetailRepository> _exportDetails = null!;
        private Mock<IInventoryRepository> _inventories = null!;
        private Mock<IMaterialRepository> _materials = null!;
        private Mock<IInvoiceRepository> _invoices = null!;
        private Mock<IMapper> _mapper = null!;

        private ExportService _sut = null!;

        [SetUp]
        public void SetUp()
        {
            _exports = new Mock<IExportRepository>(MockBehavior.Strict);
            _exportDetails = new Mock<IExportDetailRepository>(MockBehavior.Strict);
            _inventories = new Mock<IInventoryRepository>(MockBehavior.Strict);
            _materials = new Mock<IMaterialRepository>(MockBehavior.Strict);
            _invoices = new Mock<IInvoiceRepository>(MockBehavior.Strict);
            _mapper = new Mock<IMapper>(MockBehavior.Strict);

            _sut = new ExportService(
                _exports.Object,
                _exportDetails.Object,
                _inventories.Object,
                _materials.Object,
                _invoices.Object,
                _mapper.Object
            );
        }

        // -------------------------
        // Helpers
        // -------------------------
        private static Inventory Inv(int whId, int matId, decimal? qty)
            => new Inventory { WarehouseId = whId, MaterialId = matId, Quantity = qty };

        private static Material Mat(int id, string code = "M", string name = "Material", string? unit = "pcs")
            => new Material { MaterialId = id, MaterialCode = code, MaterialName = name, Unit = unit };

        private static Invoice MakeInvoice(string code, params InvoiceDetail[] details)
            => new Invoice
            {
                InvoiceId = 999,
                InvoiceCode = code,
                InvoiceDetails = details.ToList()
            };

        private static InvoiceDetail InvDetail(int matId, int qty, decimal unitPrice, Material? mat = null)
            => new InvoiceDetail
            {
                MaterialId = matId,
                Quantity = qty,
                UnitPrice = unitPrice,
                Material = mat
            };

        // =========================================================
        // CreatePendingExport
        // =========================================================

        [Test]
        public void CreatePendingExport_WhenNoMaterials_ThrowsRequireAtLeastOne()
        {
            var dto = new ExportRequestDto
            {
                WarehouseId = 1,
                CreatedBy = 10,
                Notes = "N",
                Materials = new List<ExportMaterialDto>() // empty
            };

            var ex = Assert.Throws<Exception>(() => _sut.CreatePendingExport(dto));
            Assert.That(ex!.Message, Is.EqualTo(ExportMsgs.MSG_REQUIRE_AT_LEAST_ONE_MATERIAL));
        }

        [Test]
        public void CreatePendingExport_WhenInventoryNotFound_ThrowsFormatted()
        {
            var dto = new ExportRequestDto
            {
                WarehouseId = 1,
                CreatedBy = 10,
                Notes = "N",
                Materials = new List<ExportMaterialDto>
                {
                    new ExportMaterialDto { MaterialId = 101, Quantity = 5, UnitPrice = 10 }
                }
            };

            _inventories.Setup(r => r.GetByWarehouseAndMaterial(1, 101))
                        .Returns((Inventory?)null);

            var ex = Assert.Throws<Exception>(() => _sut.CreatePendingExport(dto));
            Assert.That(ex!.Message,
                Is.EqualTo(string.Format(ExportMsgs.MSG_MATERIAL_NOT_FOUND_IN_WAREHOUSE, 101, 1)));

            _inventories.VerifyAll();
        }

        [Test]
        public void CreatePendingExport_WhenNotEnoughStock_ThrowsFormatted()
        {
            var dto = new ExportRequestDto
            {
                WarehouseId = 1,
                CreatedBy = 10,
                Notes = "N",
                Materials = new List<ExportMaterialDto>
                {
                    new ExportMaterialDto { MaterialId = 101, Quantity = 10, UnitPrice = 10 }
                }
            };

            _inventories.Setup(r => r.GetByWarehouseAndMaterial(1, 101))
                        .Returns(Inv(1, 101, qty: 3));

            var ex = Assert.Throws<Exception>(() => _sut.CreatePendingExport(dto));
            Assert.That(ex!.Message,
                Is.EqualTo(string.Format(ExportMsgs.MSG_NOT_ENOUGH_STOCK, 101, 3, 10)));

            _inventories.VerifyAll();
        }

        [Test]
        public void CreatePendingExport_WhenMaterialNotFound_ThrowsFormatted()
        {
            var dto = new ExportRequestDto
            {
                WarehouseId = 1,
                CreatedBy = 10,
                Notes = "N",
                Materials = new List<ExportMaterialDto>
                {
                    new ExportMaterialDto { MaterialId = 101, Quantity = 2, UnitPrice = 10 }
                }
            };

            _inventories.Setup(r => r.GetByWarehouseAndMaterial(1, 101))
                        .Returns(Inv(1, 101, qty: 100));

            Export? addedExport = null;
            _exports.Setup(r => r.Add(It.IsAny<Export>()))
                    .Callback<Export>(e =>
                    {
                        e.ExportId = 555; // giả lập DB set id
                        addedExport = e;
                    });

            _materials.Setup(r => r.GetById(101)).Returns((Material?)null);

            var ex = Assert.Throws<Exception>(() => _sut.CreatePendingExport(dto));
            Assert.That(ex!.Message, Is.EqualTo(string.Format(ExportMsgs.MSG_MATERIAL_NOT_FOUND, 101)));

            _inventories.VerifyAll();
            _exports.VerifyAll();
            _materials.VerifyAll();
        }

        [Test]
        public void CreatePendingExport_Success_AddsExport_AndAddsExportDetails()
        {
            var dto = new ExportRequestDto
            {
                WarehouseId = 1,
                CreatedBy = 10,
                Notes = "N",
                Materials = new List<ExportMaterialDto>
                {
                    new ExportMaterialDto { MaterialId = 101, Quantity = 2, UnitPrice = 10 },
                    new ExportMaterialDto { MaterialId = 102, Quantity = 1, UnitPrice = 20 }
                }
            };

            _inventories.Setup(r => r.GetByWarehouseAndMaterial(1, 101)).Returns(Inv(1, 101, 100));
            _inventories.Setup(r => r.GetByWarehouseAndMaterial(1, 102)).Returns(Inv(1, 102, 100));

            Export? addedExport = null;
            _exports.Setup(r => r.Add(It.IsAny<Export>()))
                    .Callback<Export>(e =>
                    {
                        e.ExportId = 777; // giả lập DB id
                        addedExport = e;
                    });

            _materials.Setup(r => r.GetById(101)).Returns(Mat(101, "M101", "Mat101", "kg"));
            _materials.Setup(r => r.GetById(102)).Returns(Mat(102, "M102", "Mat102", "kg"));

            var addedDetails = new List<ExportDetail>();
            _exportDetails.Setup(r => r.Add(It.IsAny<ExportDetail>()))
                          .Callback<ExportDetail>(d => addedDetails.Add(d));

            var result = _sut.CreatePendingExport(dto);

            Assert.That(result, Is.Not.Null);
            Assert.That(addedExport, Is.SameAs(result));
            Assert.That(result.Status, Is.EqualTo(StatusEnum.Pending.ToStatusString()));
            Assert.That(result.CreatedAt, Is.Not.EqualTo(default(DateTime)));
            Assert.That(result.ExportCode, Does.StartWith("EXP-"));

            Assert.That(addedDetails.Count, Is.EqualTo(2));
            Assert.That(addedDetails.All(d => d.ExportId == 777), Is.True);
            Assert.That(addedDetails[0].LineTotal, Is.EqualTo(2 * 10));
            Assert.That(addedDetails[1].LineTotal, Is.EqualTo(1 * 20));

            _exports.VerifyAll();
            _inventories.VerifyAll();
            _materials.VerifyAll();
            _exportDetails.VerifyAll();
        }

        // =========================================================
        // ConfirmExport
        // =========================================================

        [Test]
        public void ConfirmExport_WhenPendingExportNotFound_Throws()
        {
            _exports.Setup(r => r.GetAll()).Returns(new List<Export>());

            var ex = Assert.Throws<Exception>(() => _sut.ConfirmExport("EXP-001", "note"));
            Assert.That(ex!.Message, Is.EqualTo(ExportMsgs.MSG_PENDING_EXPORT_NOT_FOUND));

            _exports.VerifyAll();
        }

        [Test]
        public void ConfirmExport_WhenExportDetailsMissing_Throws()
        {
            var exp = new Export
            {
                ExportId = 1,
                ExportCode = "EXP-001",
                WarehouseId = 10,
                Status = StatusEnum.Pending.ToStatusString(),
                Notes = "old"
            };

            _exports.Setup(r => r.GetAll()).Returns(new List<Export> { exp });
            _exportDetails.Setup(r => r.GetByExportId(1)).Returns(new List<ExportDetail>());

            var ex = Assert.Throws<Exception>(() => _sut.ConfirmExport("EXP-001", null));
            Assert.That(ex!.Message, Is.EqualTo(ExportMsgs.MSG_EXPORT_DETAIL_NOT_FOUND));

            _exports.VerifyAll();
            _exportDetails.VerifyAll();
        }

        [Test]
        public void ConfirmExport_WhenNotEnoughStockAtConfirm_ThrowsFormatted()
        {
            var exp = new Export
            {
                ExportId = 1,
                ExportCode = "EXP-001",
                WarehouseId = 10,
                Status = StatusEnum.Pending.ToStatusString(),
                Notes = "old"
            };

            var details = new List<ExportDetail>
            {
                new ExportDetail { ExportId = 1, MaterialId = 101, Quantity = 5 }
            };

            _exports.Setup(r => r.GetAll()).Returns(new List<Export> { exp });
            _exportDetails.Setup(r => r.GetByExportId(1)).Returns(details);

            _inventories.Setup(r => r.GetByWarehouseAndMaterial(10, 101))
                        .Returns(Inv(10, 101, qty: 2));

            var ex = Assert.Throws<Exception>(() => _sut.ConfirmExport("EXP-001", "n"));
            Assert.That(ex!.Message,
                Is.EqualTo(string.Format(ExportMsgs.MSG_NOT_ENOUGH_STOCK_WHEN_CONFIRM, 101)));

            _exports.VerifyAll();
            _exportDetails.VerifyAll();
            _inventories.VerifyAll();
        }

        [Test]
        public void ConfirmExport_Success_SubtractsInventory_UpdatesExport()
        {
            var exp = new Export
            {
                ExportId = 1,
                ExportCode = "EXP-001",
                WarehouseId = 10,
                Status = StatusEnum.Pending.ToStatusString(),
                Notes = "old"
            };

            var details = new List<ExportDetail>
            {
                new ExportDetail { ExportId = 1, MaterialId = 101, Quantity = 5 },
                new ExportDetail { ExportId = 1, MaterialId = 102, Quantity = 2 }
            };

            var inv101 = Inv(10, 101, 10);
            var inv102 = Inv(10, 102, 2);

            _exports.Setup(r => r.GetAll()).Returns(new List<Export> { exp });
            _exportDetails.Setup(r => r.GetByExportId(1)).Returns(details);

            _inventories.Setup(r => r.GetByWarehouseAndMaterial(10, 101)).Returns(inv101);
            _inventories.Setup(r => r.GetByWarehouseAndMaterial(10, 102)).Returns(inv102);

            _inventories.Setup(r => r.Update(inv101));
            _inventories.Setup(r => r.Update(inv102));
            _exports.Setup(r => r.Update(exp));

            var result = _sut.ConfirmExport("EXP-001", "new note");

            Assert.That(result, Is.SameAs(exp));
            Assert.That(exp.Status, Is.EqualTo(StatusEnum.Success.ToStatusString()));
            Assert.That(exp.Notes, Is.EqualTo("new note"));
            Assert.That(exp.ExportDate, Is.Not.EqualTo(default(DateTime)));
            Assert.That(exp.UpdatedAt, Is.Not.EqualTo(default(DateTime)));

            Assert.That(inv101.Quantity, Is.EqualTo(5)); // 10-5
            Assert.That(inv102.Quantity, Is.EqualTo(0)); // 2-2
            Assert.That(inv101.UpdatedAt, Is.Not.EqualTo(default(DateTime)));
            Assert.That(inv102.UpdatedAt, Is.Not.EqualTo(default(DateTime)));

            _exports.VerifyAll();
            _exportDetails.VerifyAll();
            _inventories.VerifyAll();
        }

        // =========================================================
        // RejectExport
        // =========================================================

        [Test]
        public void RejectExport_WhenNotFound_ReturnsNull()
        {
            _exports.Setup(r => r.GetExportById(1)).Returns((Export?)null);

            var result = _sut.RejectExport(1);

            Assert.That(result, Is.Null);
            _exports.VerifyAll();
        }

        [Test]
        public void RejectExport_WhenNotPending_Throws()
        {
            var exp = new Export { ExportId = 1, Status = StatusEnum.Success.ToStatusString() };
            _exports.Setup(r => r.GetExportById(1)).Returns(exp);

            var ex = Assert.Throws<Exception>(() => _sut.RejectExport(1));
            Assert.That(ex!.Message, Is.EqualTo(ExportMsgs.MSG_ONLY_PENDING_CAN_BE_REJECTED));

            _exports.VerifyAll();
        }

        [Test]
        public void RejectExport_WhenPending_SetsRejected_AndUpdates()
        {
            var exp = new Export { ExportId = 1, Status = StatusEnum.Pending.ToStatusString() };
            _exports.Setup(r => r.GetExportById(1)).Returns(exp);
            _exports.Setup(r => r.Update(exp));

            var result = _sut.RejectExport(1);

            Assert.That(result, Is.SameAs(exp));
            Assert.That(exp.Status, Is.EqualTo(StatusEnum.Rejected.ToStatusString()));
            Assert.That(exp.UpdatedAt, Is.Not.EqualTo(default(DateTime)));

            _exports.VerifyAll();
        }

        // =========================================================
        // CreateExportFromInvoice
        // =========================================================

        [Test]
        public void CreateExportFromInvoice_WhenInvoiceNotFound_Throws()
        {
            var dto = new ExportFromInvoiceDto { InvoiceCode = "INV-404", WarehouseId = 1, CreatedBy = 10 };

            _invoices.Setup(r => r.GetByCode("INV-404")).Returns((Invoice?)null);

            var ex = Assert.Throws<Exception>(() => _sut.CreateExportFromInvoice(dto));
            Assert.That(ex!.Message, Is.EqualTo(ExportMsgs.MSG_INVOICE_NOT_FOUND));

            _invoices.VerifyAll();
        }

        [Test]
        public void CreateExportFromInvoice_WhenInvoiceHasNoDetails_Throws()
        {
            var dto = new ExportFromInvoiceDto { InvoiceCode = "INV-1", WarehouseId = 1, CreatedBy = 10 };

            _invoices.Setup(r => r.GetByCode("INV-1"))
                     .Returns(new Invoice { InvoiceId = 1, InvoiceCode = "INV-1", InvoiceDetails = new List<InvoiceDetail>() });

            var ex = Assert.Throws<Exception>(() => _sut.CreateExportFromInvoice(dto));
            Assert.That(ex!.Message, Is.EqualTo(ExportMsgs.MSG_INVOICE_HAS_NO_DETAILS));

            _invoices.VerifyAll();
        }

        [Test]
        public void CreateExportFromInvoice_WhenMaterialNotInWarehouse_ThrowsFormatted()
        {
            var dto = new ExportFromInvoiceDto { InvoiceCode = "INV-1", WarehouseId = 1, CreatedBy = 10 };

            var invoice = MakeInvoice("INV-1",
                InvDetail(101, 2, 10, Mat(101, "M101", "Cement", "kg"))
            );

            _invoices.Setup(r => r.GetByCode("INV-1")).Returns(invoice);

            _inventories.Setup(r => r.GetByWarehouseAndMaterial(1, 101))
                        .Returns((Inventory?)null);

            var ex = Assert.Throws<Exception>(() => _sut.CreateExportFromInvoice(dto));
            Assert.That(ex!.Message,
                Is.EqualTo(string.Format(ExportMsgs.MSG_MATERIAL_NOT_IN_WAREHOUSE, "Cement")));

            _invoices.VerifyAll();
            _inventories.VerifyAll();
        }

        [Test]
        public void CreateExportFromInvoice_WhenNotEnoughStock_ThrowsFormatted()
        {
            var dto = new ExportFromInvoiceDto { InvoiceCode = "INV-1", WarehouseId = 1, CreatedBy = 10 };

            var invoice = MakeInvoice("INV-1",
                InvDetail(101, 10, 10, Mat(101, "M101", "Cement", "kg"))
            );

            _invoices.Setup(r => r.GetByCode("INV-1")).Returns(invoice);

            _inventories.Setup(r => r.GetByWarehouseAndMaterial(1, 101))
                        .Returns(Inv(1, 101, qty: 3));

            var ex = Assert.Throws<Exception>(() => _sut.CreateExportFromInvoice(dto));
            Assert.That(ex!.Message,
                Is.EqualTo(string.Format(ExportMsgs.MSG_NOT_ENOUGH_STOCK, "Cement", 3, 10)));

            _invoices.VerifyAll();
            _inventories.VerifyAll();
        }

        [Test]
        public void CreateExportFromInvoice_Success_GeneratesCode_AddsExportDetails_UpdatesInvoice()
        {
            var dto = new ExportFromInvoiceDto
            {
                InvoiceCode = "INV-OK",
                WarehouseId = 1,
                CreatedBy = 10,
                Notes = null
            };

            var invoice = MakeInvoice("INV-OK",
                InvDetail(101, 2, 10, Mat(101, "M101", "Cement", "kg")),
                InvDetail(102, 1, 20, Mat(102, "M102", "Sand", "kg"))
            );

            _invoices.Setup(r => r.GetByCode("INV-OK")).Returns(invoice);

            _inventories.Setup(r => r.GetByWarehouseAndMaterial(1, 101)).Returns(Inv(1, 101, 100));
            _inventories.Setup(r => r.GetByWarehouseAndMaterial(1, 102)).Returns(Inv(1, 102, 100));

            // GenerateNextExportCode() dùng _exports.GetAll()
            // Giả lập đã có EXP-001 và EXP-003 => next = EXP-002
            _exports.Setup(r => r.GetAll()).Returns(new List<Export>
            {
                new Export { ExportCode = "EXP-001" },
                new Export { ExportCode = "EXP-003" }
            });

            Export? addedExport = null;
            _exports.Setup(r => r.Add(It.IsAny<Export>()))
                    .Callback<Export>(e =>
                    {
                        e.ExportId = 500; // giả lập DB
                        addedExport = e;
                    });

            var addedDetails = new List<ExportDetail>();
            _exportDetails.Setup(r => r.Add(It.IsAny<ExportDetail>()))
                          .Callback<ExportDetail>(d => addedDetails.Add(d));

            _invoices.Setup(r => r.Update(invoice));

            var export = _sut.CreateExportFromInvoice(dto);

            Assert.That(export, Is.Not.Null);
            Assert.That(addedExport, Is.SameAs(export));
            Assert.That(export.ExportCode, Is.EqualTo("EXP-002"));
            Assert.That(export.Status, Is.EqualTo(StatusEnum.Pending.ToStatusString()));
            Assert.That(export.CreatedAt, Is.Not.EqualTo(default(DateTime)));
            Assert.That(export.Notes, Is.EqualTo("Export from Invoice INV-OK"));

            Assert.That(addedDetails.Count, Is.EqualTo(2));
            Assert.That(addedDetails.All(d => d.ExportId == 500), Is.True);
            Assert.That(addedDetails[0].LineTotal, Is.EqualTo(2 * 10));
            Assert.That(addedDetails[1].LineTotal, Is.EqualTo(1 * 20));

            Assert.That(invoice.ExportStatus, Is.EqualTo(StatusEnum.Success.ToStatusString()));
            Assert.That(invoice.UpdatedAt, Is.Not.EqualTo(default(DateTime)));

            _invoices.VerifyAll();
            _inventories.VerifyAll();
            _exports.VerifyAll();
            _exportDetails.VerifyAll();
        }

        // =========================================================
        // GetAllExports / GetById
        // =========================================================

        [Test]
        public void GetAllExports_ReturnsMappedList()
        {
            var list = new List<Export> { new Export { ExportId = 1 }, new Export { ExportId = 2 } };
            var mapped = new List<ExportResponseDto> { new ExportResponseDto(), new ExportResponseDto() };

            _exports.Setup(r => r.GetAll()).Returns(list);
            _mapper.Setup(m => m.Map<List<ExportResponseDto>>(list)).Returns(mapped);

            var result = _sut.GetAllExports();

            Assert.That(result, Is.SameAs(mapped));

            _exports.VerifyAll();
            _mapper.VerifyAll();
        }

        [Test]
        public void GetById_WhenNotFound_ReturnsNull()
        {
            _exports.Setup(r => r.GetExportById(99)).Returns((Export?)null);

            var result = _sut.GetById(99);

            Assert.That(result, Is.Null);
            _exports.VerifyAll();
        }

        [Test]
        public void GetById_WhenFound_ReturnsMapped()
        {
            var exp = new Export { ExportId = 10 };
            var dto = new ExportResponseDto();

            _exports.Setup(r => r.GetExportById(10)).Returns(exp);
            _mapper.Setup(m => m.Map<ExportResponseDto>(exp)).Returns(dto);

            var result = _sut.GetById(10);

            Assert.That(result, Is.SameAs(dto));
            _exports.VerifyAll();
            _mapper.VerifyAll();
        }

        // =========================================================
        // GetExportsByPartnerOrManager
        // =========================================================

        [Test]
        public void GetExportsByPartnerOrManager_FiltersByPartnerId_LoadsDetails_AndMaps()
        {
            int partnerId = 10;

            var e1 = new Export
            {
                ExportId = 1,
                Warehouse = new Warehouse
                {
                    Manager = new User { PartnerId = partnerId }
                }
            };

            var e2 = new Export
            {
                ExportId = 2,
                Warehouse = new Warehouse
                {
                    Manager = new User { PartnerId = 999 }
                }
            };

            var exports = new List<Export> { e1, e2 };

            _exports.Setup(r => r.GetAllWithWarehouse()).Returns(exports);

            _exportDetails.Setup(r => r.GetByExportId(1))
                          .Returns(new List<ExportDetail> { new ExportDetail { ExportId = 1, MaterialId = 1 } });

            // Mapper map list filtered (chỉ còn 1)
            _mapper.Setup(m => m.Map<List<ExportResponseDto>>(It.Is<List<Export>>(l => l.Count == 1 && l[0].ExportId == 1)))
                   .Returns(new List<ExportResponseDto> { new ExportResponseDto() });

            var result = _sut.GetExportsByPartnerOrManager(partnerId: partnerId, managerId: null);

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(e1.ExportDetails, Is.Not.Null);

            _exports.VerifyAll();
            _exportDetails.VerifyAll();
            _mapper.VerifyAll();
        }

        [Test]
        public void GetExportsByPartnerOrManager_FiltersByManagerId_LoadsDetails_AndMaps()
        {
            int managerId = 77;

            var e1 = new Export
            {
                ExportId = 1,
                Warehouse = new Warehouse { ManagerId = managerId }
            };

            var e2 = new Export
            {
                ExportId = 2,
                Warehouse = new Warehouse { ManagerId = 999 }
            };

            _exports.Setup(r => r.GetAllWithWarehouse()).Returns(new List<Export> { e1, e2 });

            _exportDetails.Setup(r => r.GetByExportId(1))
                          .Returns(new List<ExportDetail> { new ExportDetail { ExportId = 1, MaterialId = 1 } });

            _mapper.Setup(m => m.Map<List<ExportResponseDto>>(It.Is<List<Export>>(l => l.Count == 1 && l[0].ExportId == 1)))
                   .Returns(new List<ExportResponseDto> { new ExportResponseDto() });

            var result = _sut.GetExportsByPartnerOrManager(partnerId: null, managerId: managerId);

            Assert.That(result, Has.Count.EqualTo(1));

            _exports.VerifyAll();
            _exportDetails.VerifyAll();
            _mapper.VerifyAll();
        }
    }
}
