using FluentValidation;
using ManagementBE.Kernel.Core.Configurations;
using ManagementBE.Kernel.Core.Enums;
using ManagementBE.Kernel.Core.Infrastructure;
using ManagementBE.Kernel.Core.Validators;
using ManagementBE.Kernel.Core.Wrappers;
using ManagementBE.Kernel.Domain.DTOs.Identity;
using ManagementBE.Kernel.Domain.Identity;
using ManagementBE.Presentation.Application.Validators.Forms;
using ManagementBE.Presentation.Application.Validators.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Presentation.Application.Services.Identity.Tokens
{
    public class TokenService : ITokenService
    {
        readonly UserManager<ApplicationUser> _userManager;
        readonly SignInManager<ApplicationUser> _signInManager;
        readonly ICommonService _commonService;
        readonly IApplicationLoggerService _logger;
        readonly IValidatorFactory _validatorFactory;
        readonly Configuration _configs;

        public TokenService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ICommonService commonService, IApplicationLoggerService logger, IValidatorFactory validatorFactory, Configuration configs)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _commonService = commonService;
            _logger = logger;
            _validatorFactory = validatorFactory;
            _configs = configs;
        }

        public string GenerateAccessToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("FullNameEn", user.FullNameEn),
                new Claim("FullNameAr", user.FullNameAr),
                new Claim("FullName", user.FullName)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            if (user.Gender.HasValue)
            {
                claims.Add(new Claim("Gender", user.Gender.Value.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configs.SecuritySettings.JwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configs.SecuritySettings.JwtSettings.Provider,
                audience: String.Empty,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_configs.SecuritySettings.JwtSettings.TokenExpirationInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            using var rng = RandomNumberGenerator.Create();
            var randomBytes = new byte[64];
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configs.SecuritySettings.JwtSettings.Provider,
                ValidAudience = String.Empty,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configs.SecuritySettings.JwtSettings.Key)),
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Response<AuthResponseDto>> Login(LoginDto model)
        {
            try
            {
                var validModel = await Check.ValidateAsync(model, _validatorFactory);
                if (!validModel.Succeeded)
                    return new Response<AuthResponseDto>() { BrokenRules = validModel.BrokenRules, StatusCode = validModel.StatusCode };


                var user = await _userManager.Users.Where(x=>x.Email == model.Email).FirstOrDefaultAsync();
                if (user == null)
                {
                    await _logger.LogWarning($"Login attempt failed for {model.Email} - User not found");
                    return new Response<AuthResponseDto> { Message = "Invalid email or password" };
                }

                if (!user.IsActive)
                {
                    await _logger.LogWarning($"Login attempt for inactive user: {model.Email}");
                    return new Response<AuthResponseDto> { Message = "Account is inactive" };
                }

                if (user.IsDeleted == true)
                {
                    await _logger.LogWarning($"Login attempt for deleted user: {model.Email}");
                    return new Response<AuthResponseDto> { Message = "Account not found" };
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);

                if (result.IsLockedOut)
                {
                    await _logger.LogWarning($"User account locked: {model.Email}");
                    return new Response<AuthResponseDto> { Message = "Account locked due to multiple failed login attempts" };
                }

                if (!result.Succeeded)
                {
                    await _logger.LogWarning($"Login attempt failed for {model.Email}");
                    return new Response<AuthResponseDto> { Message = "Invalid email or password" };
                }

                var roles = await _userManager.GetRolesAsync(user);
                var accessToken = GenerateAccessToken(user, roles);
                var refreshToken = GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiresOn = DateTime.UtcNow.AddDays(7);
                user.RefreshTokenCreatedByIp = _commonService.GetIpAddress();
                user.RefreshTokenCreatedOn = DateTime.UtcNow;
                user.RefreshTokenRevokedByIp = null;
                user.RefreshTokenRevokedOn = null;
                user.ModifiedOn = DateTime.UtcNow;
                user.ModifiedBy = user.Id;

                await _userManager.UpdateAsync(user);

                await _logger.LogInformation($"User {user.Email} logged in successfully");

                return new Response<AuthResponseDto>
                {
                    Succeeded = true,
                    StatusCode = (int)HttpStatusCode.Ok,
                    Data = new AuthResponseDto
                    {
                        Token = accessToken,
                        RefreshToken = refreshToken,
                        TokenExpiration = DateTime.UtcNow.AddMinutes(60),
                        RefreshTokenExpiration = user.RefreshTokenExpiresOn.Value,
                        Email = user.Email ?? string.Empty,
                        User = new UserDto
                        {
                            Id = user.Id,
                            Email = user.Email,
                            FirstNameAr = user.FirstNameAr,
                            LastNameAr = user.LastNameAr,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            PhoneNumber = user.PhoneNumber,
                            GenderId = user.Gender,
                            Role = roles.FirstOrDefault()
                        },
                        FullName = user.FullName,
                        FullNameAr = user.FullNameAr,
                        Roles = roles.ToList()
                    }
                };
            }
            catch (Exception ex)
            {

                await _logger.LogError(ex, "Login");
                throw;
            }

        }


        public async Task<Response<AuthResponseDto>> RefreshToken(RefreshTokenDto model)
        {
            try
            {
                var validModel = await Check.ValidateAsync(model, _validatorFactory);
                if (!validModel.Succeeded)
                    return new Response<AuthResponseDto> { BrokenRules = validModel.BrokenRules, StatusCode = validModel.StatusCode };


                var principal = GetPrincipalFromExpiredToken(model.AccessToken);
                if (principal == null)
                {
                    return new Response<AuthResponseDto> { Message = "Invalid access token" };
                }

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return new Response<AuthResponseDto> { Message = "Invalid token claims" };
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new Response<AuthResponseDto> { Message = "User not found" };
                }

                if (user.RefreshToken != model.RefreshToken)
                {
                    return new Response<AuthResponseDto> { Message = "Invalid refresh token" };
                }

                if (!user.IsRefreshTokenActive)
                {
                    return new Response<AuthResponseDto> { Message = "Refresh token is not active or expired" };
                }

                var roles = await _userManager.GetRolesAsync(user);
                var newAccessToken = GenerateAccessToken(user, roles);
                var newRefreshToken = GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiresOn = DateTime.UtcNow.AddDays(7);
                user.RefreshTokenCreatedByIp = _commonService.GetIpAddress();
                user.RefreshTokenCreatedOn = DateTime.UtcNow;
                user.RefreshTokenRevokedByIp = null;
                user.RefreshTokenRevokedOn = null;
                user.ModifiedOn = DateTime.UtcNow;
                user.ModifiedBy = userId;

                await _userManager.UpdateAsync(user);

                await _logger.LogInformation($"Tokens refreshed for user {user.Email}");

                return new Response<AuthResponseDto>
                {
                    Succeeded = true,
                    StatusCode = (int)HttpStatusCode.Ok,
                    Data = new AuthResponseDto
                    {
                        Token = newAccessToken,
                        RefreshToken = newRefreshToken,
                        TokenExpiration = DateTime.UtcNow.AddMinutes(60),
                        RefreshTokenExpiration = user.RefreshTokenExpiresOn.Value,
                        Email = user.Email ?? string.Empty,
                        User = new UserDto
                        {
                            Id = user.Id,
                            Email = user.Email,
                            FirstNameAr = user.FirstNameAr,
                            LastNameAr = user.LastNameAr,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            PhoneNumber = user.PhoneNumber,
                            GenderId = user.Gender,
                            Role = roles.FirstOrDefault()
                        },
                        FullName = user.FullName,
                        FullNameAr = user.FullNameAr,
                        Roles = roles.ToList()
                    }
                };
            }
            catch (Exception ex)
            {

                await _logger.LogError(ex, "RefreshToken");
                throw;
            }

        }

        public async Task<Response> RevokeToken(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return new Response { Message = "User not found" };

                if (string.IsNullOrEmpty(user.RefreshToken))
                    return new Response { Message = "No active refresh token found" };

                user.RefreshTokenRevokedByIp = _commonService.GetIpAddress();
                user.RefreshTokenRevokedOn = DateTime.UtcNow;
                user.ModifiedOn = DateTime.UtcNow;
                user.ModifiedBy = userId;

                await _userManager.UpdateAsync(user);

                await _logger.LogInformation($"Refresh token revoked for user {user.Email}");

                return new Response
                {
                    Succeeded = true,
                    StatusCode = (int)HttpStatusCode.Ok,
                    Message = "Refresh token revoked successfully"
                };
            }
            catch (Exception ex)
            {

                await _logger.LogError(ex, "RevokeToken");
                throw;
            }

        }

        public async Task<Response> ChangePassword(ChangePasswordDto model, string userId)
        {
            try
            {
                var validModel = await Check.ValidateAsync(model, _validatorFactory);
                if (!validModel.Succeeded)
                    return new Response() { BrokenRules = validModel.BrokenRules, StatusCode = validModel.StatusCode };


                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return new Response { Message = "User not found" };

                var result = await _userManager.ChangePasswordAsync(user,
                    model.CurrentPassword, model.NewPassword);

                if (!result.Succeeded)
                {
                    return new Response { Message = String.Join("\n", result.Errors.Select(e => e.Description)) };
                }

                user.RefreshTokenRevokedByIp = _commonService.GetIpAddress();
                user.RefreshTokenRevokedOn = DateTime.UtcNow;
                user.ModifiedOn = DateTime.UtcNow;
                user.ModifiedBy = userId;

                await _userManager.UpdateAsync(user);

                await _logger.LogInformation($"User {user.Email} changed password successfully");

                return new Response
                {
                    Succeeded = true,
                    StatusCode = (int)HttpStatusCode.Ok,
                    Message = "Password changed successfully. Please login again with your new password."
                };
            }
            catch (Exception ex)
            {

                await _logger.LogError(ex, "ChangePassword");
                throw;
            }

        }

        public async Task<Response> Logout(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null && !string.IsNullOrEmpty(user.RefreshToken))
                {
                    user.RefreshTokenRevokedByIp = _commonService.GetIpAddress();
                    user.RefreshTokenRevokedOn = DateTime.UtcNow;
                    user.ModifiedOn = DateTime.UtcNow;
                    user.ModifiedBy = userId;

                    await _userManager.UpdateAsync(user);
                }

                await _logger.LogInformation($"User {userId} logged out");

                return new Response { Message = "Logged out successfully" };
            }
            catch (Exception ex)
            {

                await _logger.LogError(ex, "Logout");
                throw;
            }
        }

    }
}
