using Application.DTOs;
using Application.Services.Interfaces;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class VietnamGeoService : IVietnamGeoService
    {
        private readonly Dictionary<string, List<string>> _map = new();
        private readonly Dictionary<string, List<string>> _originalMap = new();
        private readonly Dictionary<string, string> _provinceNames = new();

        public VietnamGeoService()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Data", "vietnam.json");
            var json = File.ReadAllText(path);

            var provinces = JsonSerializer.Deserialize<List<ProvinceDto>>(json) ?? new List<ProvinceDto>();

            foreach (var item in provinces)
            {
                if (string.IsNullOrWhiteSpace(item.Name))
                    continue;

                var normalizedProvince = Normalize(item.Name);
                _provinceNames[normalizedProvince] = item.Name;

                var districtsNormalized = (item.Wards ?? new List<WardDto>())
                    .Select(w => Normalize(w.Name))
                    .ToList();

                var districtsOriginal = (item.Wards ?? new List<WardDto>())
                    .Select(w => w.Name)
                    .ToList();

                _map[normalizedProvince] = districtsNormalized;
                _originalMap[normalizedProvince] = districtsOriginal;
            }
        }

        public bool IsDistrictInProvince(string province, string district)
        {
            var p = Normalize(province);
            var d = Normalize(district);

            if (!_map.ContainsKey(p))
                return false;

            return _map[p].Contains(d);
        }

        public IEnumerable<string> GetProvinces() => _provinceNames.Values;

        public IEnumerable<string> GetDistricts(string province)
        {
            var p = Normalize(province);
            return _originalMap.ContainsKey(p) ? _originalMap[p] : Enumerable.Empty<string>();
        }

        private string Normalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";

            input = input.Trim().ToLower();

            string[] prefixes =
            {
                "quận ", "huyện ", "tp ", "tp. ", "thành phố ",
                "thị xã ", "thị trấn ", "phường "
            };

            foreach (var pre in prefixes)
            {
                if (input.StartsWith(pre))
                    input = input.Substring(pre.Length);
            }

            return RemoveDiacritics(input);
        }

        private string RemoveDiacritics(string s)
        {
            var norm = s.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in norm)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }




    }
}
