﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MrGutter.Domain.Models.RequestModel;
using MrGutter.Services.IServices;

namespace MrGutter.WebAPI.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    
    public class UserManagerController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IUserManager _userManager;
       // private readonly IUserManagerServiceLib _userManager;
        private readonly ILogger<UserManagerController> _logger;

        public UserManagerController(ILogger<UserManagerController> logger, IUserManager userManager, IAccountService accountService)
        {
            _accountService = accountService;
            _logger = logger;
            _userManager = userManager;
        }
        #region "Group"
        // [Authorize(Roles = "SuperAdmin")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetGroups")]
        public async Task<IActionResult> GetGroupMasters(string? encReq)
        {
            var result = await _userManager.GetGroupMaster(encReq);
            return result == null ? NotFound() : Ok(result);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "SuperAdmin")]                  
        [HttpPost("CreateGroup")]                          
        public async Task<IActionResult> CreateGroupMaster([FromBody] RequestModel reqModel)
        {
            var result = await _userManager.CreateOrSetGroupMaster(reqModel,'C');
            return Ok(result);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "SuperAdmin")]
        [HttpPut("UpdateGroup")]
        public async Task<IActionResult> SetGroupMaster([FromBody] RequestModel reqModel)
        {
            var result = await _userManager.CreateOrSetGroupMaster(reqModel, 'U');
            return Ok(result);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("DeleteGroup")]
        public async Task<IActionResult> DeleteGroupMaster([FromBody] RequestModel reqModel)
        {
            var result = await _userManager.CreateOrSetGroupMaster(reqModel, 'D');
            return Ok(result);
        }
        #endregion
        #region "Role Master"
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("GetRoles")]
        public async Task<IActionResult> GetRoleMasters(string? encReq)
        {
            var result = await _userManager.GetRoleMaster(encReq);
            return result == null ? NotFound() : Ok(result);
        }
        [HttpGet("GetRoleByUserId")]
        public async Task<IActionResult> GetRoleByUserId(string encReq)
        {
            var result = await _userManager.GetRoleByUserId(encReq);
            return result == null ? NotFound() : Ok(result);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] RequestModel reqModel)
        {
            var result = await _userManager.CreateOrSetRoleMaster(reqModel, 'C');
            return Ok(result);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "SuperAdmin")]
        [HttpPut("UpdateRole")]
        public async Task<IActionResult> SetRole([FromBody] RequestModel reqModel)
        {
            var result = await _userManager.CreateOrSetRoleMaster(reqModel, 'U');
            return Ok(result);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("DeleteRole")]
        public async Task<IActionResult> DeleteRole([FromBody] RequestModel reqModel)
        {
            var result = await _userManager.CreateOrSetRoleMaster(reqModel, 'D');
            return Ok(result);
        }
        #endregion

        #region "UserMaster"
        //[Authorize]
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers(string? encReq)
        {
            var result = await _userManager.GetUsers(encReq);
            return result == null ? NotFound() : Ok(result);
        }
        [HttpPost("CreateOrSetUser")]
        public async Task<IActionResult> CreateOrSetUsers(UserMasterReqModel req)
        {
            var result = await _userManager.CreateOrSetUser(req);
            return Ok(result);
        }
        //[Authorize(Roles = "SuperAdmin")]
       // [HttpPut("UpdateUser")]
       // public async Task<IActionResult> UpdateUser([FromBody] RequestModel req)
       // {
       //     var result = await _userManager.CreateOrSetUser(req, 'U');
       //     return Ok(result);
       // }
       //// [Authorize(Roles = "SuperAdmin")]
       // [HttpDelete("DeleteUser")]
       // public async Task<IActionResult> DeleteUser([FromBody] RequestModel reqModel)
       // {
       //     var result = await _userManager.CreateOrSetUser(reqModel, 'D');
       //     return Ok(result);
       // }
        #endregion
        #region "Loging"
        //[Authorize]
        [HttpPost("CreateLogHistory")]
        public async Task<IActionResult> CreateLogHistory(RequestModel req)
        {
            var result = await _userManager.CreateLogHistory(req);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("GetLogHistory")]
        public async Task<IActionResult> GetLogHistory()
        {
            var result = await _userManager.GetLogHistory();
            return result == null ? NotFound() : Ok(result);
        }
        #endregion
    }
}
