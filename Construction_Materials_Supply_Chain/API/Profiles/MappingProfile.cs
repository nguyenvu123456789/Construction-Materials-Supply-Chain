using API.DTOs;
using AutoMapper;
using BusinessObjects;

namespace API.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            // Vendor
            CreateMap<Vendor, VendorDto>();
            CreateMap<VendorDto, Vendor>();

            // Supplier
            CreateMap<Supplier, SupplierDto>();
            CreateMap<SupplierDto, Supplier>();

            // Warehouse
            CreateMap<Warehouse, WarehouseDto>();
            CreateMap<WarehouseDto, Warehouse>();

            // Transport
            CreateMap<Transport, TransportDto>();
            CreateMap<TransportDto, Transport>();

            // ActivityLog
            CreateMap<ActivityLog, ActivityLogDto>();
            CreateMap<ActivityLogDto, ActivityLog>();

            // ShippingLog
            CreateMap<ShippingLog, ShippingLogDto>();
            CreateMap<ShippingLogDto, ShippingLog>();
        }
    }
}
