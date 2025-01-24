using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrQuote.Domain.Models.RequestModel;
using MrQuote.Services.IServices;
using MrQuote.Services.Services;

namespace MrQuote.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationService _configurationService;
        public ConfigurationController(IConfigurationService configurationService)
        {
                _configurationService = configurationService;
        }
        [HttpPost("CreateOrSetBranding")]
        public async Task<IActionResult> CreateOrSetBranding(BrandingReqModel req)
        {
            var result = await _configurationService.CreateOrSetBranding(req);
            return Ok(result);
        }
        [HttpGet("GetCompanyBranding")]
        public async Task<IActionResult> GetCompanyBranding(int companyId)
        {
            var result = await _configurationService.GetBrandings(companyId);
            return result == null ? NotFound() : Ok(result);
        }
        [HttpGet("GetCompanyTheme")]
        public async Task<IActionResult> GetCompanyTheme(int companyId)
        {
            var result = await _configurationService.GetCompanyTheme(companyId);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
