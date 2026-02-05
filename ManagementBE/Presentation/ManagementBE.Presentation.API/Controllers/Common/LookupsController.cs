using ManagementBE.Kernel.Core.Controllers;
using ManagementBE.Kernel.Domain.DTOs.Common;
using ManagementBE.Presentation.Application.Services.Common;
using ManagementBE.Presentation.Application.Services.Identity.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementBE.Presentation.API.Controllers.Common
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Lookups")]
    public class LookupsController : BaseApiController
    {
        readonly ILookupsService _lookupsService;
        readonly IUsersService _usersService;

        public LookupsController(ILookupsService lookupsService, IUsersService usersService)
        {
            _lookupsService = lookupsService;
            _usersService = usersService;
        }


        /// <summary>
        /// Get Gender lookups
        /// </summary>
        [HttpGet("genders")]
        [Authorize(Roles = "Admin,User")]
        [ProducesResponseType(typeof(List<LookupDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGenders()
        {
            return Ok(await _lookupsService.GetGenders());
        }

        /// <summary>
        /// Get Task Status lookups
        /// </summary>
        [HttpGet("task-statuses")]
        [Authorize(Roles = "Admin,User")]
        [ProducesResponseType(typeof(List<LookupDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTaskStatuses()
        {
            return Ok(await _lookupsService.GetTaskStatuses());
        }

        /// <summary>
        /// Get all the Roles
        /// </summary>
        [HttpGet("roles")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<LookupDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRoles()
        {
            return Ok(await _usersService.GetRoles());
        }
    }
}
