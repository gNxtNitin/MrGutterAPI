using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrQuote.Domain.Models.RequestModel;
using MrQuote.Services.IServices;
using MrQuote.Services.Services;

namespace MrQuote.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class LayoutController : ControllerBase
    {
        private readonly ILayoutService _layoutService;
        public LayoutController(ILayoutService layoutService)
        {
            _layoutService= layoutService;
        }
        [HttpPost("CreateOrSetLayout")]
        public async Task<IActionResult> CreateOrSetLayout(LayoutReqModel req)
        {
            var result = await _layoutService.CreateOrSetLayout(req);
            return Ok(result);
        }
        [HttpGet("GetLayout")]
        public async Task<IActionResult> GetLayout(int companyId)
        {
            var result = await _layoutService.GetLayout(companyId);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
