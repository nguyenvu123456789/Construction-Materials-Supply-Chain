using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Interface;
using Domain.Models;

namespace Application.Services.Implements
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _repo;
        private readonly IMapper _mapper;

        public AddressService(IAddressRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public AddressResponseDto Create(AddressCreateDto dto)
        {
            var a = new Address { Name = dto.Name, Line1 = dto.Line1, City = dto.City, Lat = dto.Lat, Lng = dto.Lng };
            _repo.Add(a);
            return _mapper.Map<AddressResponseDto>(a);
        }

        public AddressResponseDto? Get(int id)
        {
            var a = _repo.GetById(id);
            return a == null ? null : _mapper.Map<AddressResponseDto>(a);
        }

        public List<AddressResponseDto> GetAll() =>
            _repo.GetAll().Select(_mapper.Map<AddressResponseDto>).ToList();

        public List<AddressResponseDto> Search(string? q, string? city, int? top) =>
            _repo.Search(q, city, top).Select(_mapper.Map<AddressResponseDto>).ToList();

        public void Update(int id, AddressUpdateDto dto)
        {
            var a = _repo.GetById(id) ?? throw new KeyNotFoundException();
            a.Name = dto.Name;
            a.Line1 = dto.Line1;
            a.City = dto.City;
            a.Lat = dto.Lat;
            a.Lng = dto.Lng;
            _repo.Update(a);
        }

        public void Delete(int id)
        {
            var a = _repo.GetById(id) ?? throw new KeyNotFoundException();
            _repo.Delete(a);
        }
    }
}
