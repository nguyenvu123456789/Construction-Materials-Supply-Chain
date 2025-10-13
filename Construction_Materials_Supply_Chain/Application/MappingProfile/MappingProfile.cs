using Application.DTOs;
using Application.DTOs.Partners;
using AutoMapper;
using Domain.Models;

namespace Application.MappingProfile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(d => d.Roles, o => o.MapFrom(s => s.UserRoles.Select(ur => ur.Role.RoleName).ToList()));
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

            CreateMap<Export, ExportResponseDto>()
                .ForMember(d => d.Details, o => o.MapFrom(s => s.ExportDetails));
            CreateMap<ExportDetail, ExportDetailResponseDto>();

            CreateMap<MaterialCheck, MaterialCheckDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();

            CreateMap<Partner, PartnerDto>()
                .ForMember(d => d.PartnerTypeName, o => o.MapFrom(s => s.PartnerType.TypeName));
            CreateMap<PartnerCreateDto, Partner>();
            CreateMap<PartnerUpdateDto, Partner>();
            CreateMap<PartnerType, PartnerTypeDto>()
                .ForMember(d => d.Partners, o => o.Ignore());
        }
    }
}