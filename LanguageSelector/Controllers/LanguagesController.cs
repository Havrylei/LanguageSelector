using LanguageSelector.Enums;
using LanguageSelector.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LanguageSelector.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LanguagesController : ControllerBase
    {
        private readonly ILanguageService _languageService;

        public LanguagesController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        [HttpGet]
        public async Task<ActionResult> GetLanguagesAsync([FromQuery] SortType? sortType)
        {
            var result = await _languageService.GetLanguagesAsync(sortType);

            return Ok(result);
        }
    }
}