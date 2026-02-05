using FluentValidation;
using ManagementBE.Kernel.Core.Controllers;
using ManagementBE.Kernel.Core.Infrastructure;
using ManagementBE.Kernel.Domain.DTOs.Identity;
using ManagementBE.Kernel.Domain.Identity;
using ManagementBE.Presentation.Application.Services.Identity.Users;
using ManagementBE.Presentation.Application.Validators.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ManagementBE.Presentation.API.Controllers.Identity.Users
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Users")]
    public class UsersController : BaseApiController
    {
        readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }


        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool includeDeleted = false)
        {
            var result = await _usersService.GetAllUsers(page, pageSize, search, includeDeleted);
            return Ok(result);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(string id)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");

            if (id != currentUserId && !isAdmin)
            {
                return Forbid();
            }

            var result = await _usersService.GetUserById(id);
            return Ok(result);
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto model)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");

            if (id != currentUserId && !isAdmin)
            {
                return Forbid();
            }

           var result = await _usersService.UpdateUser(id, model, currentUserId);
            return Ok(result);
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] RegisterDto model)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _usersService.Register(model, currentUserId);
            return Ok(result);
        }


        /// <summary>
        /// Delete user (Admin only) - Soft delete
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _usersService.DeleteUser(id);
            return Ok(result);
        }

        /// <summary>
        /// Activate/Deactivate user (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/change-user-status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeUserStatus(string id)
        {
            var result = await _usersService.ChangeUserStatus(id);
            return Ok(result);
        }
    }
}
