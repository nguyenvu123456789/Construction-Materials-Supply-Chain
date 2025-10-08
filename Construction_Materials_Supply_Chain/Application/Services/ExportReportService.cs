using Application.DTOs;
using Application.Interfaces;
using Domain.Interface;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

public class ExportReportService : IExportReportService
{
    private readonly IExportReportRepository _reports;
    private readonly IExportRepository _exports;
    private readonly IInventoryRepository _inventories;

    public ExportReportService(
        IExportReportRepository reports,
        IExportRepository exports,
        IInventoryRepository inventories)
    {
        _reports = reports;
        _exports = exports;
        _inventories = inventories;
    }

    public ExportReport CreateReport(CreateExportReportDto dto)
    {
        var export = _exports.GetById(dto.ExportId) ?? throw new Exception("Export not found.");

        var report = new ExportReport
        {
            ExportId = export.ExportId,
            ReportedBy = dto.ReportedBy,
            Notes = dto.Notes,
            Status = "Pending",
            ReportDate = DateTime.UtcNow
        };

        foreach (var d in dto.Details)
        {
            var detail = new ExportReportDetail
            {
                ExportReport = report,
                MaterialId = d.MaterialId,
                Quantity = d.Quantity,
                Reason = d.Reason,
                Keep = d.Keep
            };
            report.ExportReportDetails.Add(detail);
        }

        _reports.Add(report);
        return report;
    }

    public void ReviewReport(int reportId, ReviewExportReportDto dto)
    {
        var report = _reports.GetById(reportId) ?? throw new Exception("Report not found.");

        report.Status = dto.Approve ? "Approved" : "Rejected";
        report.DecidedBy = dto.DecidedBy;
        report.DecidedAt = DateTime.UtcNow;

        if (dto.Approve)
        {
            foreach (var d in report.ExportReportDetails)
            {
                if (!d.Keep)
                {
                    var inventory = _inventories.GetByMaterialId(d.MaterialId, report.Export.WarehouseId);

                    if (inventory == null)
                        throw new Exception($"Không đủ vật tư {d.MaterialId} trong kho {report.Export.WarehouseId}");

                    inventory.Quantity -= d.Quantity;
                    _inventories.Update(inventory);
                }
                else
                {
                    // Logic tìm vật tư khác bù đủ xuất
                }
            }
        }

        _reports.Update(report);
    }

    public ExportReport? GetById(int reportId) => _reports.GetById(reportId);

    public List<ExportReport> GetAllPending() =>
        _reports.GetAll().Where(r => r.Status == "Pending").ToList();
}
