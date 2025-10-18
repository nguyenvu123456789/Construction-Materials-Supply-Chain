using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAddressService
    {
        AddressResponseDto Create(AddressCreateDto dto);
        AddressResponseDto? Get(int id);
        List<AddressResponseDto> GetAll();
        List<AddressResponseDto> Search(string? q, string? city, int? top);
        void Update(int id, AddressUpdateDto dto);
        void Delete(int id);
    }
}
