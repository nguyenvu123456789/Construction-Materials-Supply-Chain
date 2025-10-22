using Application.DTOs;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IPersonnelService
    {
        PersonResponseDto Create(PersonCreateDto dto);
        PersonResponseDto? Get(string type, int id);
        List<PersonResponseDto> GetAll(string type);
        List<PersonResponseDto> Search(string type, string? q, bool? active, int? top);
        void Update(string type, int id, PersonUpdateDto dto);
        void Delete(string type, int id);

        AvailabilityResponseDto GetAvailability(string type, DateTimeOffset at, int durationMin);
    }
}
