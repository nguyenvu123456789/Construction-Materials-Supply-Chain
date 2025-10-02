using API.DTOs;
using AutoMapper;
using BusinessObjects;

namespace API.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Warehouse, WarehouseDto>().ReverseMap();
            CreateMap<Transport, TransportDto>().ReverseMap();
            CreateMap<ActivityLog, ActivityLogDto>().ReverseMap();
            CreateMap<ShippingLog, ShippingLogDto>().ReverseMap();
            CreateMap<ImportRequest, ImportRequestDto>().ReverseMap();
            CreateMap<ImportRequestDetail, ImportRequestDetailDto>().ReverseMap();
            CreateMap<ExportRequest, ExportRequestDto>().ReverseMap();
            CreateMap<ExportRequestDetail, ExportRequestDetailDto>().ReverseMap();


            CreateMap<Partner, PartnerDto>()
                .ForMember(dest => dest.PartnerTypeName, opt => opt.MapFrom(src => src.PartnerType.TypeName));
            CreateMap<PartnerDto, Partner>();

            CreateMap<PartnerType, PartnerTypeDto>().ReverseMap();
        }
    }
}
