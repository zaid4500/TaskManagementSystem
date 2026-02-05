using ManagementBE.Kernel.Core.Services;
using ManagementBE.Kernel.Core.Wrappers;
using ManagementBE.Kernel.Domain.DTOs.Identity;
using ManagementBE.Kernel.Domain.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Presentation.Application.Services.Identity.Tokens
{
    public interface ITokenService : IApplicationService
    {
        Task<Response> ChangePassword(ChangePasswordDto model, string userId);
        string GenerateAccessToken(ApplicationUser user, IList<string> roles);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
        Task<Response<AuthResponseDto>> Login(LoginDto model);
        Task<Response> Logout(string userId);
        Task<Response<AuthResponseDto>> RefreshToken(RefreshTokenDto model);
        Task<Response> RevokeToken(string userId);
    }
}
