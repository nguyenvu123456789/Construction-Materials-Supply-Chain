using Application.DTOs;
using AutoMapper;
using Domain.Models;

namespace Application.MappingProfile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(d => d.Roles, o => o.MapFrom(s => s.UserRoles.Select(ur => ur.Role.RoleName).ToList()))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status));
            CreateMap<UserDto, User>()
                .ForMember(d => d.UserRoles, o => o.Ignore());
            CreateMap<UserCreateDto, User>()
                .ForMember(d => d.UserId, o => o.Ignore())
                .ForMember(d => d.UserRoles, o => o.Ignore());
            CreateMap<UserUpdateDto, User>()
                .ForMember(d => d.UserId, o => o.Ignore())
                .ForMember(d => d.UserRoles, o => o.Ignore());

            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<RoleCreateDto, Role>()
                .ForMember(d => d.RoleId, o => o.Ignore());
            CreateMap<RoleUpdateDto, Role>()
                .ForMember(d => d.RoleId, o => o.Ignore());

            CreateMap<AuditLog, AuditLogDto>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User != null ? s.User.UserName : null));

            CreateMap<User, AuthResponseDto>();

            CreateMap<ActivityLog, ActivityLogDto>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User != null ? s.User.UserName : null));
            CreateMap<ActivityLogDto, ActivityLog>()
                .ForMember(d => d.User, o => o.Ignore());

            CreateMap<Warehouse, WarehouseDto>().ReverseMap();
            CreateMap<Transport, TransportDto>().ReverseMap();
            CreateMap<ShippingLog, ShippingLogDto>().ReverseMap();

            CreateMap<Import, ImportResponseDto>()
                .ForMember(d => d.InvoiceCode, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreatedAt ?? DateTime.UtcNow))
                .ForMember(d => d.Materials, o => o.MapFrom(s => s.ImportDetails));
            CreateMap<ImportRequestDto, Import>().ReverseMap();

            CreateMap<Material, MaterialDto>()
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.CategoryName))
                .ForMember(d => d.PartnerName, o => o.MapFrom(s => s.Partner.PartnerName))
                .ForMember(d => d.Quantity, o => o.MapFrom(s => s.Inventories.FirstOrDefault()!.Quantity))
                .ForMember(d => d.WarehouseName, o => o.MapFrom(s => s.Inventories.FirstOrDefault()!.Warehouse.WarehouseName))
                .ReverseMap();

            CreateMap<Import, PendingImportResponseDto>()
                .ForMember(d => d.Materials, o => o.MapFrom(s => s.ImportDetails));
            CreateMap<ImportDetail, PendingImportMaterialResponseDto>();
            CreateMap<ImportReport, ImportReportResponseDto>()
    .ForMember(d => d.Details, o => o.MapFrom(s => s.ImportReportDetails));

            CreateMap<ImportReportDetail, ImportReportDetailDto>()
                .ForMember(d => d.MaterialCode, o => o.MapFrom(s => s.Material.MaterialCode))
                .ForMember(d => d.MaterialName, o => o.MapFrom(s => s.Material.MaterialName));

            CreateMap<Import, SimpleImportDto>();

            CreateMap<Invoice, SimpleInvoiceDto>();

            CreateMap<Export, ExportResponseDto>()
                .ForMember(d => d.Details, o => o.MapFrom(s => s.ExportDetails));
            CreateMap<ExportDetail, ExportDetailResponseDto>();

            CreateMap<MaterialCheck, MaterialCheckDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();

            CreateMap<Partner, PartnerDto>()
                .ForMember(d => d.PartnerTypeName, o => o.MapFrom(s => s.PartnerType.TypeName))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status));
            CreateMap<PartnerCreateDto, Partner>()
                .ForMember(d => d.Status, o => o.MapFrom(s => string.IsNullOrEmpty(s.Status) ? "Active" : s.Status));
            CreateMap<PartnerUpdateDto, Partner>()
                .ForMember(d => d.Status, o => o.MapFrom(s => string.IsNullOrEmpty(s.Status) ? "Active" : s.Status));
            CreateMap<PartnerType, PartnerTypeDto>()
                .ForMember(d => d.Partners, o => o.Ignore());

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

            CreateMap<BankStatementLine, BankReconLineDto>()
                .ForMember(d => d.BankStatementLineId, o => o.MapFrom(s => s.BankStatementLineId))
                .ForMember(d => d.Date, o => o.MapFrom(s => s.Date))
                .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
                .ForMember(d => d.ExternalRef, o => o.MapFrom(s => s.ExternalRef))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status))
                .ForMember(d => d.ReceiptId, o => o.MapFrom(s => s.ReceiptId))
                .ForMember(d => d.PaymentId, o => o.MapFrom(s => s.PaymentId));
        }
    }
}