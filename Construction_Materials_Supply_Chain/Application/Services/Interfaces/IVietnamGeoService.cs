namespace Application.Services.Interfaces
{
    public interface IVietnamGeoService
    {
        bool IsDistrictInProvince(string province, string district);
        IEnumerable<string> GetProvinces();
        IEnumerable<string> GetDistricts(string province);
    }
}
