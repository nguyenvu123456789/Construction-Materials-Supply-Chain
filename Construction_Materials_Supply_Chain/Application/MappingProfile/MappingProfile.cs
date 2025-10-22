using Application.DTOs;
using AutoMapper;
using Domain.Models;
using System;
using System.Linq;

namespace Application.MappingProfile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ===== USER =====
            CreateMap<User, UserDto>()
                .ForMember(d => d.Roles, o => o.MapFrom(s => s.UserRoles != null
                    ? s.UserRoles.Select(ur => ur.Role.RoleName).ToList()
                    : new List<string>()))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status));

            CreateMap<User, UserDto>()
                .ForMember(d => d.Roles, o => o.MapFrom(s => s.UserRoles.Select(ur => ur.Role.RoleName).ToList()))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status))
                .ForMember(d => d.AvatarBase64, o => o.MapFrom(s => s.AvatarBase64));

            CreateMap<UserCreateDto, User>()
                .ForMember(d => d.UserId, o => o.Ignore())
                .ForMember(d => d.UserRoles, o => o.Ignore())
                .ForMember(d => d.AvatarBase64, o => o.MapFrom(s => s.AvatarBase64));

            CreateMap<UserUpdateDto, User>()
                .ForMember(d => d.UserId, o => o.Ignore())
                .ForMember(d => d.UserRoles, o => o.Ignore())
                .ForMember(d => d.AvatarBase64, o => o.MapFrom(s => s.AvatarBase64));

            // ===== ROLE =====
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<RoleCreateDto, Role>()
                .ForMember(d => d.RoleId, o => o.Ignore());
            CreateMap<RoleUpdateDto, Role>()
                .ForMember(d => d.RoleId, o => o.Ignore());

            // ===== AUDIT & ACTIVITY LOG =====
            CreateMap<AuditLog, AuditLogDto>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User != null ? s.User.UserName : null));

            CreateMap<User, AuthResponseDto>();

            CreateMap<ActivityLog, ActivityLogDto>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User != null ? s.User.UserName : null));
            CreateMap<ActivityLogDto, ActivityLog>()
                .ForMember(d => d.User, o => o.Ignore());

            // ===== WAREHOUSE & TRANSPORT =====
            CreateMap<Warehouse, WarehouseDto>().ReverseMap();
            CreateMap<Transport, TransportDto>().ReverseMap();
            CreateMap<ShippingLog, ShippingLogDto>().ReverseMap();

            // ===== IMPORT =====
            CreateMap<Import, ImportResponseDto>()
                .ForMember(d => d.InvoiceCode, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreatedAt ?? DateTime.UtcNow))
                .ForMember(d => d.Materials, o => o.MapFrom(s => s.ImportDetails));
            CreateMap<ImportRequestDto, Import>().ReverseMap();
            CreateMap<ImportReport, ImportReportResponseDto>()
            .ForMember(dest => dest.Invoice, opt => opt.MapFrom(src => src.Invoice))
            .ForMember(dest => dest.Import, opt => opt.MapFrom(src => src.Import))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.ImportReportDetails));

            // Các map phụ
            CreateMap<Invoice, SimpleInvoiceDto>();
            CreateMap<Import, SimpleImportDto>();
            CreateMap<ImportReportDetail, ImportReportDetailDto>();

            CreateMap<Import, PendingImportResponseDto>()
                .ForMember(d => d.Materials, o => o.MapFrom(s => s.ImportDetails));

            CreateMap<ImportDetail, PendingImportMaterialResponseDto>();
            CreateMap<ImportReport, ImportReportResponseDto>()
                .ForMember(d => d.Details, o => o.MapFrom(s => s.ImportReportDetails));

            CreateMap<ImportReportDetail, ImportReportDetailDto>()
                .ForMember(d => d.MaterialCode, o => o.MapFrom(s => s.Material.MaterialCode))
                .ForMember(d => d.MaterialName, o => o.MapFrom(s => s.Material.MaterialName));

            CreateMap<Import, SimpleImportDto>();
            // ===== Invoice
            CreateMap<CreateInvoiceDto, Invoice>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pending"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.InvoiceDetails, opt => opt.MapFrom(src => src.Details));

            CreateMap<CreateInvoiceDetailDto, InvoiceDetail>()
                .ForMember(dest => dest.LineTotal, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));
            // ===== MATERIAL =====
           

            // ===== EXPORT =====
            CreateMap<Export, ExportResponseDto>()
                .ForMember(d => d.Details, o => o.MapFrom(s => s.ExportDetails));
            CreateMap<ExportDetail, ExportDetailResponseDto>();
            CreateMap<ExportReport, ExportReportResponseDto>()
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.ExportReportDetails));

            CreateMap<ExportReportDetail, ExportReportDetailResponseDto>()
                .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material.MaterialName));

            // ===== CATEGORY =====
            CreateMap<Category, CategoryDto>().ReverseMap();

            // ===== PARTNER =====
            CreateMap<Partner, PartnerDto>()
                .ForMember(d => d.PartnerTypeName, o => o.MapFrom(s => s.PartnerType != null ? s.PartnerType.TypeName : null))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status));

            CreateMap<PartnerCreateDto, Partner>()
                .ForMember(d => d.Status, o => o.MapFrom(s => string.IsNullOrEmpty(s.Status) ? "Active" : s.Status));

            CreateMap<PartnerUpdateDto, Partner>()
                .ForMember(d => d.Status, o => o.MapFrom(s => string.IsNullOrEmpty(s.Status) ? "Active" : s.Status));

            CreateMap<PartnerType, PartnerTypeDto>()
                .ForMember(d => d.Partners, o => o.Ignore());

            // ===== MATERIAL CHECK =====
            CreateMap<MaterialCheck, MaterialCheckDto>().ReverseMap();

            // ===== ACCOUNTING: JOURNAL, RECEIPT, PAYMENT =====
            CreateMap<JournalLine, LedgerLineDto>()
                .ForMember(d => d.PostingDate, o => o.MapFrom(s => s.JournalEntry.PostingDate))
                .ForMember(d => d.SourceType, o => o.MapFrom(s => s.JournalEntry.SourceType))
                .ForMember(d => d.SourceId, o => o.MapFrom(s => s.JournalEntry.SourceId))
                .ForMember(d => d.ReferenceNo, o => o.MapFrom(s => s.JournalEntry.ReferenceNo))
                .ForMember(d => d.PartnerId, o => o.MapFrom(s => s.PartnerId))
                .ForMember(d => d.InvoiceId, o => o.MapFrom(s => s.InvoiceId))
                .ForMember(d => d.Debit, o => o.MapFrom(s => s.Debit))
                .ForMember(d => d.Credit, o => o.MapFrom(s => s.Credit));

            CreateMap<Receipt, CashbookItemDto>()
                .ForMember(d => d.Type, o => o.MapFrom(_ => "Receipt"))
                .ForMember(d => d.Date, o => o.MapFrom(s => s.Date))
                .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                .ForMember(d => d.Method, o => o.MapFrom(s => s.Method))
                .ForMember(d => d.PartnerId, o => o.MapFrom(s => s.PartnerId))
                .ForMember(d => d.InvoiceId, o => o.MapFrom(s => s.InvoiceId))
                .ForMember(d => d.Reference, o => o.MapFrom(s => s.Reference))
                .ForMember(d => d.Id, o => o.MapFrom(s => s.ReceiptId));

            CreateMap<Payment, CashbookItemDto>()
                .ForMember(d => d.Type, o => o.MapFrom(_ => "Payment"))
                .ForMember(d => d.Date, o => o.MapFrom(s => s.Date))
                .ForMember(d => d.Amount, o => o.MapFrom(s => -s.Amount)) 
                .ForMember(d => d.Method, o => o.MapFrom(s => s.Method))
                .ForMember(d => d.PartnerId, o => o.MapFrom(s => s.PartnerId))
                .ForMember(d => d.InvoiceId, o => o.MapFrom(s => s.InvoiceId))
                .ForMember(d => d.Reference, o => o.MapFrom(s => s.Reference))
                .ForMember(d => d.Id, o => o.MapFrom(s => s.PaymentId));

            // ===== BANK RECON =====
            CreateMap<BankStatementLine, BankReconLineDto>()
                .ForMember(d => d.BankStatementLineId, o => o.MapFrom(s => s.BankStatementLineId))
                .ForMember(d => d.Date, o => o.MapFrom(s => s.Date))
                .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
                .ForMember(d => d.ExternalRef, o => o.MapFrom(s => s.ExternalRef))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status))
                .ForMember(d => d.ReceiptId, o => o.MapFrom(s => s.ReceiptId))
                .ForMember(d => d.PaymentId, o => o.MapFrom(s => s.PaymentId));

            CreateMap<Address, AddressResponseDto>().ReverseMap();
            CreateMap<Address, AddressCreateDto>().ReverseMap();
            CreateMap<Address, AddressUpdateDto>().ReverseMap();

            CreateMap<TransportStop, TransportStopDto>()
                .ForMember(d => d.StopType, o => o.MapFrom(s => s.StopType.ToString()))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.AddressName, o => o.MapFrom(s => s.Address.Name))
                .ForMember(d => d.AddressLine1, o => o.MapFrom(s => s.Address.Line1))
                .ForMember(d => d.City, o => o.MapFrom(s => s.Address.City))
                .ForMember(d => d.Lat, o => o.MapFrom(s => s.Address.Lat))
                .ForMember(d => d.Lng, o => o.MapFrom(s => s.Address.Lng));

            CreateMap<TransportOrder, TransportOrderDto>()
                .ForMember(d => d.OrderCode, o => o.MapFrom(s => s.Order.OrderCode))
                .ForMember(d => d.CustomerName, o => o.MapFrom(s => s.Order.CustomerName));
            CreateMap<TransportPorter, TransportPorterDto>()
                .ForMember(d => d.PorterName, o => o.MapFrom(s => s.Porter.FullName))
                .ForMember(d => d.Phone, o => o.MapFrom(s => s.Porter.Phone));

            CreateMap<Transport, TransportResponseDto>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.DepotName, o => o.MapFrom(s => s.Stops.Where(x => x.Seq == 0).Select(x => x.Address.Name).FirstOrDefault() ?? ""))
                .ForMember(d => d.VehicleCode, o => o.MapFrom(s => s.VehicleId != null ? s.Vehicle.Code : null))
                .ForMember(d => d.VehiclePlate, o => o.MapFrom(s => s.VehicleId != null ? s.Vehicle.PlateNumber : null))
                .ForMember(d => d.DriverName, o => o.MapFrom(s => s.DriverId != null ? s.Driver.FullName : null))
                .ForMember(d => d.DriverPhone, o => o.MapFrom(s => s.DriverId != null ? s.Driver.Phone : null))
                .ForMember(d => d.Stops, o => o.MapFrom(s => s.Stops.OrderBy(x => x.Seq)))
                .ForMember(d => d.Orders, o => o.MapFrom(s => s.TransportOrders))
                .ForMember(d => d.Porters, o => o.MapFrom(s => s.TransportPorters));

            CreateMap<ShippingLog, ShippingLogDto>().ReverseMap();
        }
    }
}
