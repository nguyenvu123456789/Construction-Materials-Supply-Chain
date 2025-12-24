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

using ImportMsgs = Application.Constants.Messages.ImportMessages;

namespace Services.Tests
{
    [TestFixture]
    public class ImportReportServiceTests
    {
        private Mock<IImportReportRepository> _reports = null!;
        private Mock<IInvoiceRepository> _invoices = null!;
        private Mock<IInventoryRepository> _inventories = null!;
        private Mock<IImportReportDetailRepository> _reportDetails = null!;
        private Mock<IMaterialRepository> _materials = null!;
        private Mock<IImportRepository> _imports = null!;
        private Mock<IImportDetailRepository> _importDetails = null!;
        private Mock<IHandleRequestRepository> _handleRequests = null!;
        private Mock<IExportRepository> _exports = null!;

        private ImportReportService _sut = null!;

        [SetUp]
        public void SetUp()
        {
            _reports = new Mock<IImportReportRepository>(MockBehavior.Strict);
            _invoices = new Mock<IInvoiceRepository>(MockBehavior.Strict);
            _inventories = new Mock<IInventoryRepository>(MockBehavior.Strict);
            _reportDetails = new Mock<IImportReportDetailRepository>(MockBehavior.Strict);
            _materials = new Mock<IMaterialRepository>(MockBehavior.Strict);
            _imports = new Mock<IImportRepository>(MockBehavior.Strict);
            _importDetails = new Mock<IImportDetailRepository>(MockBehavior.Strict);
            _handleRequests = new Mock<IHandleRequestRepository>(MockBehavior.Strict);
            _exports = new Mock<IExportRepository>(MockBehavior.Strict);

            _sut = new ImportReportService(
                _reports.Object,
                _invoices.Object,
                _inventories.Object,
                _reportDetails.Object,
                _materials.Object,
                _imports.Object,
                _importDetails.Object,
                _handleRequests.Object,
                _exports.Object
            );
        }

        // -------------------------
        // Helpers
        // -------------------------
        private static Material Mat(int id, string code = "M", string name = "Mat", string? unit = "kg")
            => new Material { MaterialId = id, MaterialCode = code, MaterialName = name, Unit = unit };

        // =========================================================
        // CreateReport
        // =========================================================

        [Test]
        public void CreateReport_WhenInvoiceCodeMissing_Throws()
        {
            var dto = new CreateImportReportDto
            {
                InvoiceCode = "",
                CreatedBy = 1,
                Notes = "n",
                Details = new List<CreateImportReportDetailDto>()
            };

            var ex = Assert.Throws<Exception>(() => _sut.CreateReport(dto));
            Assert.That(ex!.Message, Is.EqualTo(ImportMsgs.MSG_INVOICE_CODE_REQUIRED));
        }

        [Test]
        public void CreateReport_WhenInvoiceNotFound_Throws()
        {
            var dto = new CreateImportReportDto
            {
                InvoiceCode = "INV-404",
                CreatedBy = 1,
                Notes = "n",
                Details = new List<CreateImportReportDetailDto>()
            };

            _invoices.Setup(r => r.GetByCode("INV-404")).Returns((Invoice?)null);

            var ex = Assert.Throws<Exception>(() => _sut.CreateReport(dto));
            Assert.That(ex!.Message, Is.EqualTo(ImportMsgs.MSG_INVOICE_NOT_FOUND));

            _invoices.VerifyAll();
        }

        [Test]
        public void CreateReport_Success_AddsImport_AddsReport_AddsDetails_AddsHandle_AndReturnsLoadedReport()
        {
            var dto = new CreateImportReportDto
            {
                InvoiceCode = "INV-1",
                CreatedBy = 1,
                Notes = "note",
                Details = new List<CreateImportReportDetailDto>
                {
                    new CreateImportReportDetailDto
                    {
                        MaterialId = 101,
                        TotalQuantity = 10,
                        GoodQuantity = 9,
                        DamagedQuantity = 1,
                        Comment = "ok"
                    }
                }
            };

            var invoice = new Invoice
            {
                InvoiceId = 11,
                InvoiceCode = "INV-1",
                WarehouseId = 5,
            };

            _invoices.Setup(r => r.GetByCode("INV-1")).Returns(invoice);

            Import? addedImport = null;
            _imports.Setup(r => r.Add(It.IsAny<Import>()))
                    .Callback<Import>(i =>
                    {
                        i.ImportId = 100; // giả lập DB
                        addedImport = i;
                    });

            // GenerateImportReportCode uses _reports.GetAll()
            _reports.Setup(r => r.GetAll()).Returns(new List<ImportReport>());

            ImportReport? addedReport = null;
            _reports.Setup(r => r.Add(It.IsAny<ImportReport>()))
                    .Callback<ImportReport>(rp =>
                    {
                        rp.ImportReportId = 200; // giả lập DB
                        addedReport = rp;
                    });

            var addedReportDetails = new List<ImportReportDetail>();
            _reportDetails.Setup(r => r.Add(It.IsAny<ImportReportDetail>()))
                          .Callback<ImportReportDetail>(d => addedReportDetails.Add(d));

            HandleRequest? addedHandle = null;
            _handleRequests.Setup(r => r.Add(It.IsAny<HandleRequest>()))
                           .Callback<HandleRequest>(h => addedHandle = h);

            var loaded = new ImportReport
            {
                ImportReportId = 200,
                ImportReportCode = "IRP-001",
                ImportId = 100,
                InvoiceId = 11,
                Status = StatusEnum.Pending.ToStatusString(),
                ImportReportDetails = new List<ImportReportDetail>
                {
                    new ImportReportDetail{ MaterialId = 101, TotalQuantity = 10, GoodQuantity = 9, DamagedQuantity = 1 }
                }
            };

            _reports.Setup(r => r.GetByIdWithDetails(200)).Returns(loaded);

            var result = _sut.CreateReport(dto);

            Assert.That(result, Is.SameAs(loaded));

            Assert.That(addedImport, Is.Not.Null);
            Assert.That(addedImport!.Status, Is.EqualTo(StatusEnum.Pending.ToStatusString()));
            Assert.That(addedImport.WarehouseId, Is.EqualTo(5));
            Assert.That(addedImport.CreatedAt, Is.Not.EqualTo(default(DateTime)));

            Assert.That(addedReport, Is.Not.Null);
            Assert.That(addedReport!.ImportReportCode, Is.EqualTo("IRP-001"));
            Assert.That(addedReport.ImportId, Is.EqualTo(100));
            Assert.That(addedReport.InvoiceId, Is.EqualTo(11));

            Assert.That(addedReportDetails, Has.Count.EqualTo(1));
            Assert.That(addedReportDetails[0].ImportReportId, Is.EqualTo(200));

            Assert.That(addedHandle, Is.Not.Null);
            Assert.That(addedHandle!.RequestType, Is.EqualTo(StatusEnum.ImportReport.ToStatusString()));
            Assert.That(addedHandle.RequestId, Is.EqualTo(200));
            Assert.That(addedHandle.ActionType, Is.EqualTo(StatusEnum.Pending.ToStatusString()));
            Assert.That(addedHandle.Note, Is.EqualTo(ImportMsgs.MSG_IMPORT_REPORT_CREATED));
            Assert.That(addedHandle.HandledAt, Is.Not.EqualTo(default(DateTime)));

            _invoices.VerifyAll();
            _imports.VerifyAll();
            _reports.VerifyAll();
            _reportDetails.VerifyAll();
            _handleRequests.VerifyAll();
        }

        // =========================================================
        // ReviewReport
        // =========================================================

        [Test]
        public void ReviewReport_WhenReportNotFound_Throws()
        {
            _reports.Setup(r => r.GetByIdWithDetails(1)).Returns((ImportReport?)null);

            var ex = Assert.Throws<Exception>(() =>
                _sut.ReviewReport(1, new ReviewImportReportDto { ReviewedBy = 1, Status = StatusEnum.Success.ToStatusString(), Note = "ok" }));

            Assert.That(ex!.Message, Is.EqualTo(ImportMsgs.MSG_IMPORT_REPORT_NOT_FOUND));
            _reports.VerifyAll();
        }

        [Test]
        public void ReviewReport_WhenNotPending_Throws()
        {
            var report = new ImportReport { ImportReportId = 1, Status = StatusEnum.Success.ToStatusString() };
            _reports.Setup(r => r.GetByIdWithDetails(1)).Returns(report);

            var ex = Assert.Throws<Exception>(() =>
                _sut.ReviewReport(1, new ReviewImportReportDto { ReviewedBy = 2, Status = StatusEnum.Rejected.ToStatusString(), Note = "no" }));

            Assert.That(ex!.Message, Is.EqualTo(ImportMsgs.MSG_ONLY_PENDING_CAN_BE_REVIEWED));
            _reports.VerifyAll();
        }

        [Test]
        public void ReviewReport_SuccessApprove_WhenReportHasNoImport_CreatesImport_AddsImportDetails_AndUpdatesInventory()
        {
            var report = new ImportReport
            {
                ImportReportId = 10,
                ImportReportCode = "IRP-010",
                Status = StatusEnum.Pending.ToStatusString(),
                Import = null,
                Invoice = new Invoice { PartnerId = 99 }, // code uses report.Invoice?.PartnerId in WarehouseId (dù hơi lạ)
                ImportReportDetails = new List<ImportReportDetail>
                {
                    new ImportReportDetail { MaterialId = 101, GoodQuantity = 5, TotalQuantity = 5 },
                    new ImportReportDetail { MaterialId = 102, GoodQuantity = 0, TotalQuantity = 2 }, // skip
                }
            };

            _reports.Setup(r => r.GetByIdWithDetails(10)).Returns(report);
            _reports.Setup(r => r.Update(report)); // called at least once

            _handleRequests.Setup(r => r.Add(It.IsAny<HandleRequest>()));

            Import? addedImport = null;
            _imports.Setup(r => r.Add(It.IsAny<Import>()))
                    .Callback<Import>(i => { i.ImportId = 500; addedImport = i; });

            // when report.Import null => set report.ImportId and update report again
            _reports.Setup(r => r.Update(report));

            _materials.Setup(r => r.GetById(101)).Returns(Mat(101, "M101", "Mat101", "kg"));

            _importDetails.Setup(r => r.Add(It.IsAny<ImportDetail>()));

            // Inventory: null => Add new
            _inventories.Setup(r => r.GetByWarehouseAndMaterial(It.IsAny<int>(), 101))
                        .Returns((Inventory?)null);
            _inventories.Setup(r => r.Add(It.IsAny<Inventory>()));

            var dto = new ReviewImportReportDto
            {
                ReviewedBy = 7,
                Status = StatusEnum.Success.ToStatusString(),
                Note = "approve"
            };

            var res = _sut.ReviewReport(10, dto);

            Assert.That(res.ImportReportId, Is.EqualTo(10));
            Assert.That(report.Status, Is.EqualTo(StatusEnum.Success.ToStatusString()));
            Assert.That(addedImport, Is.Not.Null);
            Assert.That(report.ImportId, Is.EqualTo(500));

            _reports.VerifyAll();
            _imports.VerifyAll();
            _materials.VerifyAll();
            _importDetails.VerifyAll();
            _inventories.VerifyAll();
            _handleRequests.VerifyAll();
        }

        [Test]
        public void ReviewReport_Rejected_UpdatesReport_CancelsImportAndInvoice_AndCallsReturnImport()
        {
            var import = new Import { ImportId = 1, Status = "Old" };
            var invoice = new Invoice { InvoiceId = 2, ImportStatus = "Old", ExportStatus = "Old", CreatedBy = 9 };

            var pendingReport = new ImportReport
            {
                ImportReportId = 10,
                ImportReportCode = "IRP-010",
                Status = StatusEnum.Pending.ToStatusString(),
                Import = import,
                Invoice = invoice,
                ImportReportDetails = new List<ImportReportDetail>()
            };

            var rejectedReportForReturn = new ImportReport
            {
                ImportReportId = 10,
                ImportReportCode = "IRP-010",
                Status = StatusEnum.Rejected.ToStatusString(),
                Invoice = invoice,
                ImportReportDetails = new List<ImportReportDetail>() // won't add details
            };

            // ✅ Quan trọng: lần 1 trả Pending (ReviewReport), lần 2 trả Rejected (CreateReturnImportForSeller)
            _reports.SetupSequence(r => r.GetByIdWithDetails(10))
                   .Returns(pendingReport)
                   .Returns(rejectedReportForReturn);

            // ✅ Update có thể được gọi nhiều lần, và object có thể không đúng reference
            _reports.Setup(r => r.Update(It.IsAny<ImportReport>()));

            _handleRequests.Setup(r => r.Add(It.IsAny<HandleRequest>()));

            _imports.Setup(r => r.Update(import));
            _invoices.Setup(r => r.Update(invoice));

            _exports.Setup(r => r.GetByInvoiceId(2))
                    .Returns(new Export { WarehouseId = 3 });

            _imports.Setup(r => r.Add(It.IsAny<Import>()))
                    .Callback<Import>(i => i.ImportId = 99);

            var dto = new ReviewImportReportDto
            {
                ReviewedBy = 7,
                Status = StatusEnum.Rejected.ToStatusString(),
                Note = "reject"
            };

            var res = _sut.ReviewReport(10, dto);

            Assert.That(pendingReport.Status, Is.EqualTo(StatusEnum.Rejected.ToStatusString()));
            Assert.That(import.Status, Is.EqualTo(StatusEnum.Cancelled.ToStatusString()));
            Assert.That(invoice.ImportStatus, Is.EqualTo(StatusEnum.Rejected.ToStatusString()));
            Assert.That(invoice.ExportStatus, Is.EqualTo(StatusEnum.Cancelled.ToStatusString()));

            _reports.VerifyAll();
            _handleRequests.VerifyAll();
            _imports.VerifyAll();
            _invoices.VerifyAll();
            _exports.VerifyAll();
        }

        // =========================================================
        // CreateReturnImportForSeller
        // =========================================================

        [Test]
        public void CreateReturnImportForSeller_WhenReportNotFound_Throws()
        {
            _reports.Setup(r => r.GetByIdWithDetails(1)).Returns((ImportReport?)null);

            var ex = Assert.Throws<Exception>(() => _sut.CreateReturnImportForSeller(1, 1));
            Assert.That(ex!.Message, Is.EqualTo(ImportMsgs.MSG_IMPORT_REPORT_NOT_FOUND));

            _reports.VerifyAll();
        }

        [Test]
        public void CreateReturnImportForSeller_WhenNotRejected_Throws()
        {
            _reports.Setup(r => r.GetByIdWithDetails(1)).Returns(new ImportReport { ImportReportId = 1, Status = StatusEnum.Pending.ToStatusString() });

            var ex = Assert.Throws<Exception>(() => _sut.CreateReturnImportForSeller(1, 1));
            Assert.That(ex!.Message, Is.EqualTo(ImportMsgs.MSG_ONLY_REJECTED_REPORT_CAN_RETURN));

            _reports.VerifyAll();
        }

        [Test]
        public void CreateReturnImportForSeller_WhenInvoiceMissing_Throws()
        {
            _reports.Setup(r => r.GetByIdWithDetails(1)).Returns(new ImportReport { ImportReportId = 1, Status = StatusEnum.Rejected.ToStatusString(), Invoice = null });

            var ex = Assert.Throws<Exception>(() => _sut.CreateReturnImportForSeller(1, 1));
            Assert.That(ex!.Message, Is.EqualTo(ImportMsgs.MSG_INVOICE_REQUIRED_FOR_RETURN));

            _reports.VerifyAll();
        }

        [Test]
        public void CreateReturnImportForSeller_Success_AddsImport_AndAddsDetails()
        {
            var invoice = new Invoice { InvoiceId = 2, CreatedBy = 9 };
            var report = new ImportReport
            {
                ImportReportId = 10,
                ImportReportCode = "IRP-010",
                Status = StatusEnum.Rejected.ToStatusString(),
                Invoice = invoice,
                ImportReportDetails = new List<ImportReportDetail>
                {
                    new ImportReportDetail{ MaterialId = 101, GoodQuantity = 3 },
                    new ImportReportDetail{ MaterialId = 102, GoodQuantity = 0 } // skip
                }
            };

            _reports.Setup(r => r.GetByIdWithDetails(10)).Returns(report);
            _exports.Setup(r => r.GetByInvoiceId(2)).Returns(new Export { WarehouseId = 7 });

            Import? addedImport = null;
            _imports.Setup(r => r.Add(It.IsAny<Import>()))
                    .Callback<Import>(i => { i.ImportId = 999; addedImport = i; });

            _materials.Setup(r => r.GetById(101)).Returns(Mat(101, "M101", "Mat101", "kg"));
            _importDetails.Setup(r => r.Add(It.IsAny<ImportDetail>()));

            var res = _sut.CreateReturnImportForSeller(10, createdBy: 1);

            Assert.That(res, Is.SameAs(addedImport));
            Assert.That(res.Status, Is.EqualTo(StatusEnum.Pending.ToStatusString()));
            Assert.That(res.WarehouseId, Is.EqualTo(7));
            Assert.That(res.CreatedBy, Is.EqualTo(9));
            Assert.That(res.Notes, Does.Contain("IRP-010"));

            _reports.VerifyAll();
            _exports.VerifyAll();
            _imports.VerifyAll();
            _materials.VerifyAll();
            _importDetails.VerifyAll();
        }

        // =========================================================
        // ReviewReturnImport
        // =========================================================

        [Test]
        public void ReviewReturnImport_WhenImportNotFound_Throws()
        {
            _imports.Setup(r => r.GetByIdWithDetails(1)).Returns((Import?)null);

            var ex = Assert.Throws<Exception>(() =>
                _sut.ReviewReturnImport(1, new ReviewImportReportDto { ReviewedBy = 1, Status = StatusEnum.Success.ToStatusString(), Note = "ok" }));

            Assert.That(ex!.Message, Is.EqualTo(ImportMsgs.MSG_IMPORT_PENDING_NOT_FOUND));
            _imports.VerifyAll();
        }

        [Test]
        public void ReviewReturnImport_WhenNotPending_Throws()
        {
            var imp = new Import { ImportId = 1, Status = StatusEnum.Success.ToStatusString() };
            _imports.Setup(r => r.GetByIdWithDetails(1)).Returns(imp);

            var ex = Assert.Throws<Exception>(() =>
                _sut.ReviewReturnImport(1, new ReviewImportReportDto { ReviewedBy = 1, Status = StatusEnum.Rejected.ToStatusString(), Note = "no" }));

            Assert.That(ex!.Message, Is.EqualTo(ImportMsgs.MSG_ONLY_PENDING_CAN_BE_REJECTED));
            _imports.VerifyAll();
        }

        [Test]
        public void ReviewReturnImport_WhenReject_UpdatesStatusRejected()
        {
            var imp = new Import { ImportId = 1, Status = StatusEnum.Pending.ToStatusString() };
            _imports.Setup(r => r.GetByIdWithDetails(1)).Returns(imp);
            _imports.Setup(r => r.Update(imp));

            var res = _sut.ReviewReturnImport(1, new ReviewImportReportDto { ReviewedBy = 1, Status = StatusEnum.Rejected.ToStatusString(), Note = "reject" });

            Assert.That(res.Status, Is.EqualTo(StatusEnum.Rejected.ToStatusString()));
            _imports.VerifyAll();
        }

        [Test]
        public void ReviewReturnImport_WhenApprove_UpdatesInventoryAndMarksSuccess()
        {
            var imp = new Import
            {
                ImportId = 1,
                WarehouseId = 10,
                Status = StatusEnum.Pending.ToStatusString(),
                ImportDetails = new List<ImportDetail>
                {
                    new ImportDetail{ MaterialId = 101, Quantity = 2 },
                    new ImportDetail{ MaterialId = 102, Quantity = 1 }
                }
            };

            _imports.Setup(r => r.GetByIdWithDetails(1)).Returns(imp);
            _imports.Setup(r => r.Update(imp));

            // 101 null => Add
            _inventories.Setup(r => r.GetByWarehouseAndMaterial(10, 101)).Returns((Inventory?)null);
            _inventories.Setup(r => r.Add(It.IsAny<Inventory>()));

            // 102 exists => Update
            var inv102 = new Inventory { WarehouseId = 10, MaterialId = 102, Quantity = 5 };
            _inventories.Setup(r => r.GetByWarehouseAndMaterial(10, 102)).Returns(inv102);
            _inventories.Setup(r => r.Update(inv102));

            var res = _sut.ReviewReturnImport(1, new ReviewImportReportDto { ReviewedBy = 1, Status = StatusEnum.Success.ToStatusString(), Note = "ok" });

            Assert.That(res.Status, Is.EqualTo(StatusEnum.Success.ToStatusString()));
            Assert.That(inv102.Quantity, Is.EqualTo(6));

            _imports.VerifyAll();
            _inventories.VerifyAll();
        }

        // =========================================================
        // GetByIdResponse
        // =========================================================

        [Test]
        public void GetByIdResponse_WhenNotFound_Throws()
        {
            _reports.Setup(r => r.GetByIdWithDetails(1)).Returns((ImportReport?)null);

            var ex = Assert.Throws<Exception>(() => _sut.GetByIdResponse(1));
            Assert.That(ex!.Message, Is.EqualTo(ImportMsgs.MSG_IMPORT_REPORT_NOT_FOUND));

            _reports.VerifyAll();
        }

        [Test]
        public void GetByIdResponse_Success_UsesLastHandleForReviewerName()
        {
            var report = new ImportReport
            {
                ImportReportId = 10,
                ImportReportCode = "IRP-010",
                CreatedBy = 1,
                CreatedAt = DateTime.Now,
                Status = StatusEnum.Pending.ToStatusString(),
                CreatedByNavigation = new User { FullName = "Creator" },
                ImportReportDetails = new List<ImportReportDetail>()
            };

            _reports.Setup(r => r.GetByIdWithDetails(10)).Returns(report);

            var handles = new List<HandleRequest>
            {
                new HandleRequest
                {
                    HandledBy = 2,
                    ActionType = StatusEnum.Pending.ToStatusString(),
                    Note = "created",
                    HandledAt = DateTime.Now.AddMinutes(-5),
                    HandledByNavigation = new User { FullName = "Reviewer Old" }
                },
                new HandleRequest
                {
                    HandledBy = 3,
                    ActionType = StatusEnum.Success.ToStatusString(),
                    Note = "approved",
                    HandledAt = DateTime.Now,
                    HandledByNavigation = new User { FullName = "Reviewer New" }
                }
            };

            // GetByRequest called twice (lastHandle + creatorHandle)
            _handleRequests.Setup(r => r.GetByRequest(StatusEnum.ImportReport.ToStatusString(), 10))
                           .Returns(handles);

            var res = _sut.GetByIdResponse(10);

            Assert.That(res.ImportReportId, Is.EqualTo(10));
            Assert.That(res.CreatedByName, Is.EqualTo("Creator"));
            Assert.That(res.HandleHistory, Has.Count.EqualTo(1));
            Assert.That(res.HandleHistory[0].HandledByName, Is.EqualTo("Reviewer New"));

            _reports.VerifyAll();
            _handleRequests.VerifyAll();
        }

        // =========================================================
        // GetAllByPartner
        // =========================================================

        [Test]
        public void GetAllByPartner_FiltersByPartnerIdAndCreatedBy()
        {
            var r1 = new ImportReport
            {
                ImportReportId = 1,
                ImportReportCode = "IRP-001",
                CreatedBy = 10,
                CreatedAt = DateTime.Now,
                Status = StatusEnum.Pending.ToStatusString(),
                CreatedByNavigation = new User { FullName = "U10" },
                ImportReportDetails = new List<ImportReportDetail>()
            };
            var r2 = new ImportReport
            {
                ImportReportId = 2,
                ImportReportCode = "IRP-002",
                CreatedBy = 11,
                CreatedAt = DateTime.Now,
                Status = StatusEnum.Pending.ToStatusString(),
                CreatedByNavigation = new User { FullName = "U11" },
                ImportReportDetails = new List<ImportReportDetail>()
            };

            _reports.Setup(r => r.GetAllWithDetails()).Returns(new List<ImportReport> { r1, r2 });

            // lastHandle for r1: partnerId=99
            _handleRequests.Setup(r => r.GetByRequest(StatusEnum.ImportReport.ToStatusString(), 1))
                           .Returns(new List<HandleRequest>
                           {
                               new HandleRequest
                               {
                                   HandledBy = 100,
                                   ActionType = StatusEnum.Pending.ToStatusString(),
                                   Note = "x",
                                   HandledAt = DateTime.Now,
                                   HandledByNavigation = new User { PartnerId = 99, FullName = "P99" }
                               }
                           });

            // lastHandle for r2: partnerId=55
            _handleRequests.Setup(r => r.GetByRequest(StatusEnum.ImportReport.ToStatusString(), 2))
                           .Returns(new List<HandleRequest>
                           {
                               new HandleRequest
                               {
                                   HandledBy = 101,
                                   ActionType = StatusEnum.Pending.ToStatusString(),
                                   Note = "y",
                                   HandledAt = DateTime.Now,
                                   HandledByNavigation = new User { PartnerId = 55, FullName = "P55" }
                               }
                           });

            // Filter partnerId=99 and createdByUserId=10 => only r1
            var res = _sut.GetAllByPartner(partnerId: 99, createdByUserId: 10);

            Assert.That(res.Count, Is.EqualTo(1));
            Assert.That(res[0].ImportReportId, Is.EqualTo(1));

            _reports.VerifyAll();
            _handleRequests.VerifyAll();
        }

        // =========================================================
        // MarkAsViewed
        // =========================================================

        [Test]
        public void MarkAsViewed_WhenReportNotFound_Throws()
        {
            _reports.Setup(r => r.GetById(1)).Returns((ImportReport?)null);

            var ex = Assert.Throws<Exception>(() => _sut.MarkAsViewed(1));
            Assert.That(ex!.Message, Is.EqualTo(ImportMsgs.MSG_IMPORT_REPORT_NOT_FOUND));

            _reports.VerifyAll();
        }

        [Test]
        public void MarkAsViewed_WhenPending_UpdatesToViewed()
        {
            var report = new ImportReport { ImportReportId = 1, Status = StatusEnum.Pending.ToStatusString() };

            _reports.Setup(r => r.GetById(1)).Returns(report);
            _reports.Setup(r => r.Update(report));

            _sut.MarkAsViewed(1);

            Assert.That(report.Status, Is.EqualTo(StatusEnum.Viewed.ToStatusString()));

            _reports.VerifyAll();
        }

        [Test]
        public void MarkAsViewed_WhenNotPending_DoesNothing()
        {
            var report = new ImportReport { ImportReportId = 1, Status = StatusEnum.Success.ToStatusString() };

            _reports.Setup(r => r.GetById(1)).Returns(report);

            _sut.MarkAsViewed(1);

            _reports.VerifyAll();
        }
    }
}
