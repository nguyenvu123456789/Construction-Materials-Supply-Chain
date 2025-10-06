using API.DTOs;
using AutoMapper;
using Domain;

namespace API.Profiles
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

            CreateMap<ActivityLog, ActivityLogDto>()
                .ForMember(dest => dest.UserName, opt =>
                    opt.MapFrom(src => src.User != null ? src.User.UserName : null));

            CreateMap<ActivityLogDto, ActivityLog>()
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<Warehouse, WarehouseDto>().ReverseMap();
            CreateMap<Transport, TransportDto>().ReverseMap();
            CreateMap<ShippingLog, ShippingLogDto>().ReverseMap();

            CreateMap<Import, ImportDto>().ReverseMap();
            CreateMap<ImportDetail, ImportDetailDto>().ReverseMap();
            CreateMap<ImportRequestDto, Import>().ReverseMap();
            CreateMap<ImportReportCreateDto, ImportReport>().ReverseMap(); ;   
            CreateMap<ImportReportDetailCreateDto, ImportReportDetail>().ReverseMap(); 

            CreateMap<ExportReport, ExportReportCreateDto>().ReverseMap();
            CreateMap<ExportReportDetail, ExportReportDetailDto>().ReverseMap();
            CreateMap<MaterialCheck, MaterialCheckDto>().ReverseMap();
            CreateMap<Role, RoleDto>().ReverseMap();

            CreateMap<Partner, PartnerDto>()
                .ForMember(dest => dest.PartnerTypeName,
                           opt => opt.MapFrom(src => src.PartnerType.TypeName));
            CreateMap<PartnerDto, Partner>();

            CreateMap<PartnerType, PartnerTypeDto>().ReverseMap();
        }
    }
}