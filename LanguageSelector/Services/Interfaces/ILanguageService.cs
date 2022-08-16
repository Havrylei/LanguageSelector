using LanguageSelector.Enums;

namespace LanguageSelector.Services.Interfaces
{
    public interface ILanguageService
    {
        Task<IEnumerable<string>> GetLanguagesAsync(SortType? sortType);
    }
}
