using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrQuote.Domain.Models.RequestModel;
using MrQuote.Services.IServices;

namespace MrQuote.API.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    public class EstimateController : ControllerBase
    {
        private readonly IEstimateService _estimateService;
        public EstimateController(IEstimateService estimateService)
        {
            _estimateService = estimateService;
        }
        [HttpPost("CreateOrSetEstimate")]
        public async Task<IActionResult> CreateOrSetEstimate(EstimateReqModel req)
        {
            var result = await _estimateService.CreateOrSetEstimate(req);
            return Ok(result);
        }
        [HttpGet("GetEstimate")]
        public async Task<IActionResult> GetEstimate([FromQuery] EstimateQueryParameters queryParameters)
        {
            var result = await _estimateService.GetEstimate(queryParameters);
            return result == null ? NotFound() : Ok(result);
        }
        [HttpGet("GetStatus")]
        public async Task<IActionResult> GetStatus(string? statusId)
        {
            var result = await _estimateService.GetStatus(statusId);
            return result == null ? NotFound() : Ok(result);
        }

    }
}
