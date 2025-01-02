using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrQuote.Domain.Models.RequestModel;
using MrQuote.Services.IServices;
using MrQuote.Services.Services;

namespace MrQuote.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(ILogger<AccountController> logger, IAccountService accountService)
        {
            _accountService = accountService;
            _logger = logger;
        }
        [HttpPost("AuthenticateUser")]
        public async Task<IActionResult> AuthenticateUser(LoginReqModel rq)
        {
            var result = await _accountService.AuthenticateUser(rq);
            // return result.data == null ? NotFound(result) : Ok(result);
            return Ok(result);
        }
        [HttpPost("SetOTP")]
        public async Task<IActionResult> SetOTP(RequestModel rq)
        {
            var result = await _accountService.SetOTP(rq);
            return Ok(result);
        }
        [HttpPost("ValidateOTP")]
        public async Task<IActionResult> ValidateOTP(RequestModel rq)
        {
            var result = await _accountService.ValidateOTP(rq);
            return Ok(result);
        }
        [HttpPost("SendForgotPasswordEmail")]
        public async Task<IActionResult> SendForgotPasswordEmail(RequestModel rq)
        {
            var result = await _accountService.SendForgotPasswordEmail(rq, null);
            return Ok(result);
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(RequestModel rq)
        {
            var result = await _accountService.ResetPassword(rq);
            return Ok(result);
        }
    }
}
