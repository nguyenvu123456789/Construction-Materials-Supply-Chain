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
            CreateMap<MaterialCheck, MaterialCheckDto>().ReverseMap();
            CreateMap<Role, RoleDto>().ReverseMap();

            CreateMap<Partner, PartnerDto>()
                .ForMember(d => d.PartnerTypeName, o => o.MapFrom(s => s.PartnerType.TypeName));
            CreateMap<PartnerCreateDto, Partner>();
            CreateMap<PartnerUpdateDto, Partner>();
            CreateMap<PartnerType, PartnerTypeDto>()
                .ForMember(d => d.Partners, o => o.Ignore());
        }
    }
}