using Application.DTOs;
using Application.DTOs.Relations;
using Application.DTOs.RelationType;
using AutoMapper;
using Domain.Models;

namespace Application.MappingProfile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(d => d.Roles, o => o.MapFrom(s => s.UserRoles != null
                    ? s.UserRoles.Select(ur => ur.Role.RoleName).ToList()
                    : new List<string>()))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status));

            CreateMap<User, UserDto>()
                .ForMember(d => d.Roles, o => o.MapFrom(s => s.UserRoles != null ? s.UserRoles.Select(ur => ur.Role.RoleName).ToList() : new List<string>()))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status))
                .ForMember(d => d.HasAvatar, o => o.MapFrom(s => !string.IsNullOrEmpty(s.AvatarBase64)))
                .ForMember(d => d.AvatarUrl, o => o.MapFrom(s =>
                    string.IsNullOrEmpty(s.AvatarBase64)
                        ? null
                        : $"/api/users/{s.UserId}/avatar"))
                .ForMember(d => d.ZaloUserId, o => o.MapFrom(s => s.ZaloUserId));

            CreateMap<UserCreateDto, User>()
                .ForMember(d => d.UserId, o => o.Ignore())
                .ForMember(d => d.UserRoles, o => o.Ignore())
                .ForMember(d => d.ZaloUserId, o => o.MapFrom(s => s.ZaloUserId));

            CreateMap<UserUpdateDto, User>()
                .ForMember(d => d.UserId, o => o.Ignore())
                .ForMember(d => d.UserRoles, o => o.Ignore())
                .ForMember(d => d.ZaloUserId, o => o.MapFrom(s => s.ZaloUserId));

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

            CreateMap<Import, ImportResponseDto>()
                .ForMember(d => d.InvoiceCode, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreatedAt ?? DateTime.Now))
                .ForMember(d => d.Materials, o => o.MapFrom(s => s.ImportDetails));
            CreateMap<ImportRequestDto, Import>().ReverseMap();
            CreateMap<ImportReport, ImportReportResponseDto>()
                .ForMember(dest => dest.Invoice, opt => opt.MapFrom(src => src.Invoice))
                .ForMember(dest => dest.Import, opt => opt.MapFrom(src => src.Import))
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.ImportReportDetails))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));

            CreateMap<Invoice, SimpleInvoiceDto>();
            CreateMap<Import, SimpleImportDto>();
            CreateMap<ImportReportDetail, ImportReportDetailDto>();

            CreateMap<Import, PendingImportResponseDto>()
                .ForMember(d => d.Materials, o => o.MapFrom(s => s.ImportDetails));

            CreateMap<ImportDetail, PendingImportMaterialResponseDto>();

            CreateMap<ImportReportDetail, ImportReportDetailDto>()
                .ForMember(d => d.MaterialCode, o => o.MapFrom(s => s.Material.MaterialCode))
                .ForMember(d => d.MaterialName, o => o.MapFrom(s => s.Material.MaterialName));

            CreateMap<CreateInvoiceDto, Invoice>()
                .ForMember(dest => dest.ExportStatus, opt => opt.MapFrom(src => "Pending"))
                .ForMember(dest => dest.ImportStatus, opt => opt.MapFrom(src => "Pending"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.InvoiceDetails, opt => opt.MapFrom(src => src.Details));

            CreateMap<CreateInvoiceDetailDto, InvoiceDetail>()
                .ForMember(dest => dest.LineTotal, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));

            CreateMap<Export, ExportResponseDto>()
                .ForMember(d => d.Details, o => o.MapFrom(s => s.ExportDetails));
            CreateMap<ExportDetail, ExportDetailResponseDto>();
            CreateMap<ExportReport, ExportReportResponseDto>()
                .ForMember(dest => dest.ReportedBy, opt => opt.MapFrom(src => src.ReportedBy))
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.ExportReportDetails));

            CreateMap<ExportReportDetail, ExportReportDetailResponseDto>()
                .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material.MaterialName));

            CreateMap<Category, CategoryDto>().ReverseMap();

            CreateMap<PartnerType, PartnerTypeDto>()
                .ForMember(d => d.Partners, o => o.Ignore());

            CreateMap<Partner, PartnerDto>()
                .ForMember(d => d.PartnerTypeName,
                    o => o.MapFrom(s => s.PartnerType != null ? s.PartnerType.TypeName : string.Empty))
                .ForMember(d => d.Status,
                    o => o.MapFrom(s => s.Status))
                .ForMember(d => d.RegionIds,
                    o => o.MapFrom(s =>
                        s.PartnerRegions != null
                            ? s.PartnerRegions.Select(pr => pr.RegionId).ToList()
                            : new List<int>()))
                .ForMember(d => d.RegionNames,
                    o => o.MapFrom(s =>
                        s.PartnerRegions != null
                            ? s.PartnerRegions
                                .Where(pr => pr.Region != null)
                                .Select(pr => pr.Region.RegionName)
                                .ToList()
                            : new List<string>()))
                .ForMember(d => d.Region,
                    o => o.MapFrom(s =>
                        s.PartnerRegions != null
                            ? string.Join(", ",
                                s.PartnerRegions
                                    .Where(pr => pr.Region != null)
                                    .Select(pr => pr.Region.RegionName))
                            : null));

            CreateMap<PartnerCreateDto, Partner>()
              .ForMember(d => d.PartnerId, o => o.Ignore())
              .ForMember(d => d.PartnerRegions, o => o.Ignore())
              .ForMember(d => d.Status,
                  o => o.MapFrom(s => string.IsNullOrEmpty(s.Status) ? "Active" : s.Status.Trim()));

            CreateMap<PartnerUpdateDto, Partner>()
                .ForMember(d => d.PartnerId, o => o.Ignore())
                .ForMember(d => d.PartnerRegions, o => o.Ignore())
                .ForMember(d => d.Status,
                    o => o.MapFrom(s => string.IsNullOrEmpty(s.Status) ? "Active" : s.Status.Trim()));

            CreateMap<MaterialCheck, MaterialCheckDto>().ReverseMap();

            CreateMap<Address, AddressResponseDto>().ReverseMap();
            CreateMap<Address, AddressCreateDto>().ReverseMap();
            CreateMap<Address, AddressUpdateDto>().ReverseMap();

            CreateMap<TransportStop, TransportStopDto>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(d => d.StopType, o => o.MapFrom(s => s.StopType.ToString()))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.Invoices, o => o.MapFrom(s => s.TransportInvoices.Select(ti => ti.Invoice)));

            CreateMap<TransportPorter, TransportPorterDto>()
                .ForMember(d => d.PorterName, o => o.MapFrom(s => s.Porter.FullName))
                .ForMember(d => d.Phone, o => o.MapFrom(s => s.Porter.Phone));

            CreateMap<Transport, TransportResponseDto>()
                 .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
                 .ForMember(d => d.DepotName, o => o.MapFrom(s => s.Depot.Name))
                 .ForMember(d => d.ProviderPartnerName, o => o.MapFrom(s => s.ProviderPartner.PartnerName))
                 .ForMember(d => d.Stops, o => o.MapFrom(s => s.Stops.OrderBy(x => x.Seq)))
                 .ForMember(d => d.Porters, o => o.MapFrom(s => s.TransportPorters));

            CreateMap<ShippingLog, ShippingLogDto>().ReverseMap();

            CreateMap<NotificationReply, NotificationReplyDto>()
                .ForMember(d => d.Content, o => o.MapFrom(s => s.Message));

            CreateMap<Notification, NotificationResponseDto>()
                .ForMember(d => d.NotificationId, o => o.MapFrom(s => s.NotificationId))
                .ForMember(d => d.RecipientUserIds, o => o.MapFrom(s => s.NotificationRecipients.Select(x => x.UserId)))
                .ForMember(d => d.RecipientRoleIds, o => o.MapFrom(s => s.NotificationRecipientRoles.Select(x => x.RoleId)))
                .ForMember(d => d.Replies, o => o.MapFrom(s => s.NotificationReplies.OrderBy(x => x.CreatedAt)));

            CreateMap<EventNotificationSetting, EventNotifySettingDto>()
                .ForMember(d => d.SettingId, o => o.MapFrom(s => s.EventNotificationSettingId))
                .ForMember(d => d.SendEmail, o => o.MapFrom(s => s.SendEmail))
                .ForMember(d => d.RoleIds, o => o.MapFrom(s => s.Roles.Select(r => r.RoleId)));

            CreateMap<EventNotifySettingUpsertDto, EventNotificationSetting>();

            CreateMap<InventoryAlertRule, AlertRuleUpdateDto>()
                .ForMember(d => d.RuleId, o => o.MapFrom(s => s.InventoryAlertRuleId))
                .ForMember(d => d.RecipientMode, o => o.MapFrom(s => (int)s.RecipientMode))
                .ForMember(d => d.RoleIds, o => o.MapFrom(s => s.Roles.Select(r => r.RoleId)))
                .ForMember(d => d.UserIds, o => o.MapFrom(s => s.Users.Select(u => u.UserId)))
                .ForMember(d => d.MaterialIds,
                    o => o.MapFrom(s =>
                        s.RuleMaterials != null
                            ? s.RuleMaterials.Select(m => m.MaterialId)
                            : Array.Empty<int>()));

            CreateMap<AlertRuleCreateDto, InventoryAlertRule>()
                .ForMember(d => d.InventoryAlertRuleId, o => o.Ignore())
                .ForMember(d => d.RecipientMode, o => o.MapFrom(s => (AlertRecipientMode)s.RecipientMode))
                .ForMember(d => d.SendEmail, o => o.MapFrom(s => s.SendEmail))
                .ForMember(d => d.IsActive, o => o.MapFrom(_ => true));

            CreateMap<AlertRuleUpdateDto, InventoryAlertRule>()
                .ForMember(d => d.InventoryAlertRuleId, o => o.MapFrom(s => s.RuleId))
                .ForMember(d => d.RecipientMode, o => o.MapFrom(s => (AlertRecipientMode)s.RecipientMode));

            CreateMap<Partner, LocationSummaryDto>()
                .ForMember(d => d.PartnerId, o => o.MapFrom(s => s.PartnerId))
                .ForMember(d => d.PartnerName, o => o.MapFrom(s => s.PartnerName))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status))
                .ForMember(d => d.Region, o => o.MapFrom(s =>
                    s.PartnerRegions
                     .Where(pr => pr.Region != null)
                     .Select(pr => pr.Region.RegionName)
                     .FirstOrDefault()))
                .ForMember(d => d.TotalQuantity, o => o.Ignore())
                .ForMember(d => d.TotalRevenue, o => o.Ignore())
                .ForMember(d => d.GrowthRatePercent, o => o.Ignore());

            CreateMap<Inventory, InventorySummaryDto>()
                .ForMember(d => d.MaterialCode, o => o.Ignore())
                .ForMember(d => d.MaterialName, o => o.Ignore())
                .ForMember(d => d.CategoryName, o => o.Ignore())
                .ForMember(d => d.WarehouseName, o => o.Ignore())
                .ForMember(d => d.TotalSoldInPeriod, o => o.Ignore())
                .ForMember(d => d.RevenueInPeriod, o => o.Ignore())
                .ForMember(d => d.AverageInventory, o => o.Ignore())
                .ForMember(d => d.TurnoverRate, o => o.Ignore())
                .ForMember(d => d.IsFastMoving, o => o.Ignore())
                .ForMember(d => d.IsSlowMoving, o => o.Ignore())
                .ForMember(d => d.QuantityOnHand, o => o.MapFrom(s => s.Quantity ?? 0m));

            CreateMap<Region, RegionDto>().ReverseMap();
            CreateMap<RegionCreateDto, Region>();
            CreateMap<RegionUpdateDto, Region>();
            CreateMap<RelationType, RelationTypeDto>().ReverseMap();

            CreateMap<PartnerRelation, PartnerRelationDto>()
                .ForMember(dest => dest.RelationTypeName, opt => opt.MapFrom(src => src.RelationType.Name))
                .ForMember(dest => dest.BuyerPartnerName, opt => opt.MapFrom(src => src.BuyerPartner.PartnerName))
                .ForMember(dest => dest.SellerPartnerName, opt => opt.MapFrom(src => src.SellerPartner.PartnerName));
            CreateMap<PartnerRelationCreateDto, PartnerRelation>();
            CreateMap<PartnerRelationUpdateDto, PartnerRelation>();

            CreateMap<Receipt, ReceiptDTO>();
            CreateMap<Payment, PaymentDTO>();

            CreateMap<PaymentDTO, Payment>()
                .ForMember(dest => dest.PaymentNumber, opt => opt.Ignore())
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateCreated))
                .ForMember(dest => dest.AccountingDate, opt => opt.MapFrom(src => src.AccountingDate))
                .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.PaymentType))
                .ForMember(dest => dest.PartnerName, opt => opt.MapFrom(src => src.PartnerName))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.BankAccountFrom, opt => opt.MapFrom(src => src.BankAccountFrom))
                .ForMember(dest => dest.BankAccountTo, opt => opt.MapFrom(src => src.BankAccountTo))
                .ForMember(dest => dest.DebitAccount, opt => opt.MapFrom(src => src.DebitAccount))
                .ForMember(dest => dest.CreditAccount, opt => opt.MapFrom(src => src.CreditAccount))
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.RequestedBy, opt => opt.MapFrom(src => src.RequestedBy))
                .ForMember(dest => dest.ApprovedBy, opt => opt.MapFrom(src => src.ApprovedBy))
                .ForMember(dest => dest.ApprovalDate, opt => opt.MapFrom(src => src.ApprovalDate))
                .ForMember(dest => dest.PaidBy, opt => opt.MapFrom(src => src.PaidBy))
                .ForMember(dest => dest.Recipient, opt => opt.MapFrom(src => src.Recipient))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.Account, opt => opt.MapFrom(src => src.Account));

            CreateMap<ReceiptDTO, Receipt>()
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateCreated))
                .ForMember(dest => dest.AccountingDate, opt => opt.MapFrom(src => src.AccountingDate))
                .ForMember(dest => dest.ReceiptType, opt => opt.MapFrom(src => src.ReceiptType))
                .ForMember(dest => dest.PartnerName, opt => opt.MapFrom(src => src.PartnerName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.BankAccount, opt => opt.MapFrom(src => src.BankAccount))
                .ForMember(dest => dest.DebitAccount, opt => opt.MapFrom(src => src.DebitAccount))
                .ForMember(dest => dest.CreditAccount, opt => opt.MapFrom(src => src.CreditAccount))
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.Payee, opt => opt.MapFrom(src => src.Payee))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.PartnerId, opt => opt.MapFrom(src => src.PartnerId))
                .ForMember(dest => dest.ReceiptNumber, opt => opt.Ignore());
        }
    }
}
