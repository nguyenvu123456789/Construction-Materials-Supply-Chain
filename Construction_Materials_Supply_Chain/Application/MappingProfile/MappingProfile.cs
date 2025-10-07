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
                .ForMember(dest => dest.Roles, opt =>
                    opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.RoleName).ToList()));
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore());
            CreateMap<User, AuthResponseDto>();

            CreateMap<ActivityLog, ActivityLogDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : null));
            CreateMap<ActivityLogDto, ActivityLog>()
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<Warehouse, WarehouseDto>().ReverseMap();
            CreateMap<Transport, TransportDto>().ReverseMap();
            CreateMap<ShippingLog, ShippingLogDto>().ReverseMap();
            CreateMap<Import, ImportDto>().ReverseMap();
            CreateMap<ImportDetail, ImportDetailDto>().ReverseMap();
            CreateMap<ImportRequestDto, Import>().ReverseMap();
            CreateMap<Material, MaterialDto>()
    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
    .ForMember(dest => dest.PartnerName, opt => opt.MapFrom(src => src.Partner.PartnerName))
    .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Inventories.FirstOrDefault()!.Quantity))
    .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Inventories.FirstOrDefault()!.Warehouse.WarehouseName))
    .ReverseMap();

            CreateMap<MaterialCheck, MaterialCheckDto>().ReverseMap();
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();


            CreateMap<Partner, PartnerDto>()
                .ForMember(dest => dest.PartnerTypeName, opt => opt.MapFrom(src => src.PartnerType.TypeName));
            CreateMap<PartnerDto, Partner>();

            CreateMap<PartnerType, PartnerTypeDto>().ReverseMap();
        }
    }
}