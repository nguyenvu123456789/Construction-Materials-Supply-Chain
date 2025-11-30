using Application.Constants.Enums;
using Application.Constants.Messages;
using Application.DTOs;
using Application.Interfaces;
using Domain.Interface;
using Domain.Models;

namespace Services.Implementations
{
    public class ImportReportService : IImportReportService
    {
        private readonly IImportReportRepository _reports;
        private readonly IInvoiceRepository _invoices;
        private readonly IInventoryRepository _inventories;
        private readonly IImportReportDetailRepository _reportDetails;
        private readonly IMaterialRepository _materials;
        private readonly IImportRepository _imports;
        private readonly IImportDetailRepository _importDetails;
        private readonly IHandleRequestRepository _handleRequests;

        public ImportReportService(
            IImportReportRepository reports,
            IInvoiceRepository invoices,
            IInventoryRepository inventories,
            IImportReportDetailRepository reportDetails,
            IMaterialRepository materials,
            IImportRepository imports,
            IImportDetailRepository importDetails,
            IHandleRequestRepository handleRequests)
        {
            _reports = reports;
            _invoices = invoices;
            _inventories = inventories;
            _reportDetails = reportDetails;
            _materials = materials;
            _imports = imports;
            _importDetails = importDetails;
            _handleRequests = handleRequests;
        }

        public ImportReport CreateReport(CreateImportReportDto dto)
        {
            if (string.IsNullOrEmpty(dto.InvoiceCode))
                throw new Exception(ImportMessages.MSG_INVOICE_CODE_REQUIRED);

            // Lấy invoice
            var invoice = _invoices.GetByCode(dto.InvoiceCode)
                          ?? throw new Exception(ImportMessages.MSG_INVOICE_NOT_FOUND);

            // 🔹 Tạo Import tạm thời (Pending)
            var import = new Import
            {
                ImportCode = $"IMP-{DateTime.Now:yyyyMMddHHmmss}",
                WarehouseId = invoice.PartnerId,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.Now,
                Status = StatusEnum.Pending.ToStatusString()
            };
            _imports.Add(import);

            // 🔹 Tạo ImportReport
            var report = new ImportReport
            {
                ImportId = import.ImportId,
                InvoiceId = invoice.InvoiceId,
                CreatedBy = dto.CreatedBy,
                Notes = dto.Notes,
                CreatedAt = DateTime.Now
            };
            _reports.Add(report);

            // 🔹 Thêm chi tiết report
            foreach (var d in dto.Details)
            {
                var detail = new ImportReportDetail
                {
                    ImportReportId = report.ImportReportId,
                    MaterialId = d.MaterialId,
                    TotalQuantity = d.TotalQuantity,
                    GoodQuantity = d.GoodQuantity,
                    DamagedQuantity = d.DamagedQuantity,
                    Comment = d.Comment
                };
                _reportDetails.Add(detail);
            }

            _handleRequests.Add(new HandleRequest
            {
                RequestType = StatusEnum.ImportReport.ToStatusString(),
                RequestId = report.ImportReportId,
                HandledBy = dto.CreatedBy,
                ActionType = StatusEnum.Pending.ToStatusString(),
                Note = ImportMessages.MSG_IMPORT_REPORT_CREATED,
                HandledAt = DateTime.Now
            });

            return _reports.GetByIdWithDetails(report.ImportReportId)
                   ?? throw new Exception(ImportMessages.MSG_FAILED_LOAD_REPORT);
        }

        public ImportReportResponseDto ReviewReport(int reportId, ReviewImportReportDto dto)
        {
            var report = _reports.GetByIdWithDetails(reportId)
                         ?? throw new Exception(ImportMessages.MSG_IMPORT_REPORT_NOT_FOUND);

            // 🔹 Cập nhật trạng thái ImportReport
            report.Status = dto.Status;
            _reports.Update(report);

            // 🔹 Lưu lịch sử xử lý
            var handle = new HandleRequest
            {
                RequestType = StatusEnum.ImportReport.ToStatusString(),
                RequestId = report.ImportReportId,
                HandledBy = dto.ReviewedBy,
                ActionType = dto.Status,
                Note = dto.Note,
                HandledAt = DateTime.Now
            };
            _handleRequests.Add(handle);

            // 🔹 Nếu được duyệt
            if (dto.Status == StatusEnum.Success.ToStatusString())
            {
                var import = report.Import ?? new Import
                {
                    ImportCode = $"IMP-{DateTime.Now:yyyyMMddHHmmss}",
                    WarehouseId = report.Invoice?.PartnerId ?? 0,
                    CreatedBy = dto.ReviewedBy,
                    CreatedAt = DateTime.Now,
                    Status = StatusEnum.Success.ToStatusString()
                };

                if (report.Import == null)
                {
                    _imports.Add(import);
                    report.ImportId = import.ImportId;
                    _reports.Update(report);
                }

                // 🔹 Cập nhật tồn kho
                foreach (var detail in report.ImportReportDetails.Where(d => d.GoodQuantity > 0))
                {
                    var material = _materials.GetById(detail.MaterialId)
                        ?? throw new Exception(string.Format(ImportMessages.MSG_MATERIAL_NOT_FOUND, detail.MaterialId));

                    var importDetail = new ImportDetail
                    {
                        ImportId = import.ImportId,
                        MaterialId = material.MaterialId,
                        MaterialCode = material.MaterialCode ?? "",
                        MaterialName = material.MaterialName,
                        Unit = material.Unit,
                        Quantity = detail.GoodQuantity,
                        UnitPrice = 0,
                        LineTotal = 0
                    };
                    _importDetails.Add(importDetail);

                    var inventory = _inventories.GetByWarehouseAndMaterial(import.WarehouseId, material.MaterialId);
                    if (inventory == null)
                    {
                        _inventories.Add(new Inventory
                        {
                            WarehouseId = import.WarehouseId,
                            MaterialId = material.MaterialId,
                            Quantity = detail.GoodQuantity,
                            UpdatedAt = DateTime.Now
                        });
                    }
                    else
                    {
                        inventory.Quantity += detail.GoodQuantity;
                        inventory.UpdatedAt = DateTime.Now;
                        _inventories.Update(inventory);
                    }
                }
            }
            else if (dto.Status == StatusEnum.Rejected.ToStatusString())
            {
                if (report.Invoice != null)
                {
                    report.Invoice.ImportStatus = StatusEnum.Rejected.ToStatusString();
                    _invoices.Update(report.Invoice);
                }
            }

            // 🔹 Trả về DTO
            return new ImportReportResponseDto
            {
                ImportReportId = report.ImportReportId,
                Notes = report.Notes,
                CreatedAt = report.CreatedAt,
                ReviewedAt = DateTime.Now,
                Status = report.Status,
                Import = report.Import != null
                    ? new SimpleImportDto
                    {
                        ImportId = report.Import.ImportId,
                        ImportCode = report.Import.ImportCode,
                        CreatedAt = report.Import.CreatedAt ?? DateTime.Now,
                        Status = report.Import.Status
                    }
                    : new SimpleImportDto(),
                Invoice = report.Invoice != null
                    ? new SimpleInvoiceDto
                    {
                        InvoiceId = report.Invoice.InvoiceId,
                        InvoiceCode = report.Invoice.InvoiceCode,
                        InvoiceType = report.Invoice.InvoiceType,
                        IssueDate = report.Invoice.IssueDate
                    }
                    : new SimpleInvoiceDto(),
                Details = report.ImportReportDetails.Select(d => new ImportReportDetailDto
                {
                    MaterialId = d.MaterialId,
                    MaterialCode = d.Material?.MaterialCode ?? "",
                    MaterialName = d.Material?.MaterialName ?? "",
                    TotalQuantity = d.TotalQuantity,
                    GoodQuantity = d.GoodQuantity,
                    DamagedQuantity = d.DamagedQuantity,
                    Comment = d.Comment
                }).ToList()
            };
        }

        public ImportReportResponseDto GetByIdResponse(int reportId)
        {
            var report = _reports.GetByIdWithDetails(reportId)
                         ?? throw new Exception(ImportMessages.MSG_IMPORT_REPORT_NOT_FOUND);

            var lastHandle = _handleRequests.GetByRequest(StatusEnum.ImportReport.ToStatusString(), report.ImportReportId)
                                            .OrderByDescending(h => h.HandledAt)
                                            .FirstOrDefault();

            var reviewerName = lastHandle?.HandledByNavigation?.FullName
                               ?? lastHandle?.HandledByNavigation?.UserName
                               ?? ImportMessages.MSG_NOT_YET_REVIEWED;

            var handleHistory = lastHandle != null
                ? new List<HandleRequestDto>
                {
            new HandleRequestDto
            {
                HandledBy = lastHandle.HandledBy,
                HandledByName = reviewerName,
                ActionType = lastHandle.ActionType,
                Note = lastHandle.Note,
                HandledAt = lastHandle.HandledAt
            }
                }
                : new List<HandleRequestDto>();

            var creatorHandle = _handleRequests.GetByRequest(StatusEnum.ImportReport.ToStatusString(), report.ImportReportId)
                                               .OrderBy(h => h.HandledAt)
                                               .FirstOrDefault();
            var createdByName = creatorHandle?.HandledByNavigation?.FullName
                                ?? creatorHandle?.HandledByNavigation?.UserName
                                ?? ImportMessages.MSG_UNKNOWN_CREATOR;

            return new ImportReportResponseDto
            {
                ImportReportId = report.ImportReportId,
                Notes = report.Notes,
                CreatedAt = report.CreatedAt,
                CreatedBy = report.CreatedBy,
                CreatedByName = createdByName,
                Status = report.Status ?? StatusEnum.Pending.ToStatusString(),
                Details = report.ImportReportDetails.Select(d => new ImportReportDetailDto
                {
                    MaterialId = d.MaterialId,
                    MaterialCode = d.Material?.MaterialCode ?? "",
                    MaterialName = d.Material?.MaterialName ?? "",
                    TotalQuantity = d.TotalQuantity,
                    GoodQuantity = d.GoodQuantity,
                    DamagedQuantity = d.DamagedQuantity,
                    Comment = d.Comment
                }).ToList(),
                HandleHistory = handleHistory
            };
        }

        // 🔹 Lấy tất cả báo cáo theo Partner hoặc người tạo
        public List<ImportReportResponseDto> GetAllByPartner(int? partnerId = null, int? createdByUserId = null)
        {
            var reports = _reports.GetAllWithDetails()
                                  .OrderByDescending(r => r.CreatedAt)
                                  .ToList();

            var result = new List<ImportReportResponseDto>();

            foreach (var report in reports)
            {
                // Lấy HandleRequest mới nhất
                var lastHandle = _handleRequests
                    .GetByRequest(StatusEnum.ImportReport.ToStatusString(), report.ImportReportId)
                    .OrderByDescending(h => h.HandledAt)
                    .FirstOrDefault();

                // Filter theo partner nếu có
                if (partnerId.HasValue && lastHandle?.HandledByNavigation?.PartnerId != partnerId.Value)
                    continue;

                // Filter theo người tạo nếu có
                if (createdByUserId.HasValue && report.CreatedBy != createdByUserId.Value)
                    continue;

                // Tạo lịch sử xử lý
                var handleHistory = lastHandle != null
                    ? new List<HandleRequestDto>
                    {
                new HandleRequestDto
                {
                    HandledBy = lastHandle.HandledBy,
                    HandledByName = lastHandle.HandledByNavigation?.FullName
                                    ?? lastHandle.HandledByNavigation?.UserName
                                    ?? ImportMessages.MSG_UNKNOWN_USER,
                    ActionType = lastHandle.ActionType,
                    Note = lastHandle.Note,
                    HandledAt = lastHandle.HandledAt
                }
                    }
                    : new List<HandleRequestDto>();

                // Tên người tạo
                var createdByName = lastHandle?.HandledByNavigation?.FullName
                                    ?? lastHandle?.HandledByNavigation?.UserName
                                    ?? ImportMessages.MSG_UNKNOWN_USER;

                result.Add(new ImportReportResponseDto
                {
                    ImportReportId = report.ImportReportId,
                    CreatedBy = report.CreatedBy,
                    CreatedByName = createdByName,
                    Notes = report.Notes,
                    CreatedAt = report.CreatedAt,
                    Status = report.Status ?? StatusEnum.Pending.ToStatusString(),
                    Details = report.ImportReportDetails.Select(d => new ImportReportDetailDto
                    {
                        MaterialId = d.MaterialId,
                        MaterialCode = d.Material?.MaterialCode ?? "",
                        MaterialName = d.Material?.MaterialName ?? "",
                        TotalQuantity = d.TotalQuantity,
                        GoodQuantity = d.GoodQuantity,
                        DamagedQuantity = d.DamagedQuantity,
                        Comment = d.Comment
                    }).ToList(),
                    HandleHistory = handleHistory
                });
            }

            return result;
        }

        // 🔹 Đánh dấu báo cáo là "Đã xem"
        public void MarkAsViewed(int reportId)
        {
            var report = _reports.GetById(reportId)
                         ?? throw new Exception(ImportMessages.MSG_IMPORT_REPORT_NOT_FOUND);

            if (report.Status == StatusEnum.Pending.ToStatusString())
            {
                report.Status = StatusEnum.Viewed.ToStatusString();
                _reports.Update(report);
            }
        }
    }
}
