using FluentValidation;
using ManagementBE.Kernel.Core.Controllers;
using ManagementBE.Kernel.Core.Infrastructure;
using ManagementBE.Kernel.Domain.DTOs.Identity;
using ManagementBE.Kernel.Domain.Identity;
using ManagementBE.Presentation.Application.Services.Identity.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ManagementBE.Presentation.API.Controllers.Identity.Tokens
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Tokens")]
    public class TokensController : BaseApiController
    {
        readonly ITokenService _tokenService;

        public TokensController(
            ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        /// <summary>
        /// Login user and get access token + refresh token
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var result = await _tokenService.Login(model);
            return Ok(result);
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        [HttpPost("refresh-token")]
        [Authorize(Roles = "Admin,User")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto model)
        {
            var result = await _tokenService.RefreshToken(model);
            return Ok(result);
        }

        /// <summary>
        /// Revoke refresh token
        /// </summary>
        [HttpPost("revoke-token")]
        [Authorize(Roles = "Admin,User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RevokeToken()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _tokenService.RevokeToken(userId);
            return Ok(result);
        }

        /// <summary>
        /// Change user password
        /// </summary>
        [HttpPost("change-password")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _tokenService.ChangePassword(model,userId);
            return Ok(result);
        }

        /// <summary>
        /// Logout - Revoke current refresh token
        /// </summary>
        [HttpPost("logout")]
        [Authorize(Roles = "Admin,User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _tokenService.Logout(userId);
            return Ok(result);
        }

        /// <summary>
        /// Admin only endpoint
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnly()
        {
            return Ok(new { Message = "This is an admin-only endpoint" });
        }
    }
}
