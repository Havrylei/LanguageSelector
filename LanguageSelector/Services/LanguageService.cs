using LanguageSelector.Dtos;
using LanguageSelector.Enums;
using LanguageSelector.Services.Interfaces;
using System.Text.Json;

namespace LanguageSelector.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly JsonSerializerOptions _caseInsensitiveOption;

        public LanguageService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _caseInsensitiveOption = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<IEnumerable<string>> GetLanguagesAsync(SortType? sortType)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_configuration.GetValue<string>("culturesUrl"));

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new InvalidOperationException("Service is unavailable");
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            var data =
                await JsonSerializer.DeserializeAsync<IEnumerable<CountryLanguageDto>>(stream, _caseInsensitiveOption);

            if (!data?.Any() ?? true)
            {
                return new List<string>();
            }

            data = data.Where(x => x is not null);

            if (sortType.HasValue)
            {
                SortData(ref data, sortType.Value);
            }

            var result = TrantransformData(data);

            return result;
        }

        private void SortData(ref IEnumerable<CountryLanguageDto> data, SortType sortType)
        {
            data = sortType switch
            {
                SortType.ascending
                    => data.OrderBy(x => x.Language?.Name).ThenBy(x => x.Country?.Name),
                SortType.descending
                    => data.OrderByDescending(x => x.Language?.Name).ThenByDescending(x => x.Country?.Name),
                _ => throw new ArgumentException("Invalid sort type")
            };
        }

        private IEnumerable<string> TrantransformData(IEnumerable<CountryLanguageDto> countryLanguages)
        {
            var list = new List<string>();

            foreach (var item in countryLanguages)
            {
                var str =
                    $"{item.Language?.Name} ({item.Country?.Name}) - {item.Language?.Alpha2Code}-{item.Country?.Alpha2Code}";

                list.Add(str);
            }

            return list;
        }
    }
}
