using FluentValidation;
using ManagementBE.Kernel.Core.Configurations;
using ManagementBE.Kernel.Core.Enums;
using ManagementBE.Kernel.Core.Infrastructure;
using ManagementBE.Kernel.Core.Validators;
using ManagementBE.Kernel.Core.Wrappers;
using ManagementBE.Kernel.Domain.DTOs.Identity;
using ManagementBE.Kernel.Domain.Identity;
using ManagementBE.Presentation.Application.Validators.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Presentation.Application.Services.Identity.Users
{
    public class UsersService : IUsersService
    {
        readonly UserManager<ApplicationUser> _userManager;
        readonly ICommonService _commonService;
        readonly IApplicationLoggerService _logger;
        readonly IValidatorFactory _validatorFactory;
        readonly Configuration _configs;
        readonly RoleManager<IdentityRole> _roleManager;

        public UsersService(UserManager<ApplicationUser> userManager, ICommonService commonService, IApplicationLoggerService logger, IValidatorFactory validatorFactory, Configuration configs, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _commonService = commonService;
            _logger = logger;
            _validatorFactory = validatorFactory;
            _configs = configs;
            _roleManager = roleManager;
        }


        public async Task<Response<object>> GetAllUsers(int page = 1, int pageSize = 10, string? search = null, bool includeDeleted = false)
        {
            try
            {
                var query = _userManager.Users.AsQueryable();

                if (includeDeleted)
                {
                    query = query.IgnoreQueryFilters();
                }

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(u =>
                        (u.Email != null && u.Email.Contains(search)) ||
                        (u.FirstName != null && u.FirstName.Contains(search)) ||
                        (u.LastName != null && u.LastName.Contains(search)) ||
                        u.FirstNameAr.Contains(search) ||
                        u.LastNameAr.Contains(search) ||
                        u.FullNameEn.Contains(search) ||
                        u.FullNameAr.Contains(search)
                    );
                }

                var totalCount = await query.CountAsync();
                var users = await query
                    .OrderByDescending(u => u.CreatedOn)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var userDtos = new List<UserDto>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userDtos.Add(new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email ?? string.Empty,
                        UserName = user.UserName ?? string.Empty,
                        PhoneNumber = user.PhoneNumber,
                        FirstName = user.FirstName,
                        MiddleName = user.MiddleName,
                        LastName = user.LastName,
                        FullNameEn = user.FullNameEn,
                        FirstNameAr = user.FirstNameAr,
                        MiddleNameAr = user.MiddleNameAr,
                        LastNameAr = user.LastNameAr,
                        FullNameAr = user.FullNameAr,
                        FullName = user.FullName,
                        DateOfBirth = user.DateOfBirth,
                        GenderId = user.Gender,
                        GenderDisplay = user.Gender == 1 ? "Male" : user.Gender == 2 ? "Female" : null,
                        Nationality = user.Nationality,
                        MobileNumber = user.MobileNumber,
                        IsActive = user.IsActive,
                        CreatedBy = user.CreatedBy,
                        CreatedOn = user.CreatedOn,
                        ModifiedBy = user.ModifiedBy,
                        ModifiedOn = user.ModifiedOn,
                        Role = roles.FirstOrDefault()
                    });
                }

                return new Response<object>
                {
                    Succeeded = true,
                    StatusCode = (int)HttpStatusCode.Ok,
                    Data = new
                    {
                        TotalCount = totalCount,
                        Page = page,
                        PageSize = pageSize,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                        Items = userDtos
                    }
                };
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, "GetAllUsers");
                throw;
            }

        }

        public async Task<Response<UserDto>> GetUserById(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return new Response<UserDto> { Message = "User not found" };

                var roles = await _userManager.GetRolesAsync(user);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    PhoneNumber = user.PhoneNumber,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    FullNameEn = user.FullNameEn,
                    FirstNameAr = user.FirstNameAr,
                    MiddleNameAr = user.MiddleNameAr,
                    LastNameAr = user.LastNameAr,
                    FullNameAr = user.FullNameAr,
                    FullName = user.FullName,
                    DateOfBirth = user.DateOfBirth,
                    GenderId = user.Gender,
                    GenderDisplay = user.Gender == 1 ? "Male" : user.Gender == 2 ? "Female" : null,
                    Nationality = user.Nationality,
                    MobileNumber = user.MobileNumber,
                    IsActive = user.IsActive,
                    CreatedBy = user.CreatedBy,
                    CreatedOn = user.CreatedOn,
                    ModifiedBy = user.ModifiedBy,
                    ModifiedOn = user.ModifiedOn,
                    Role = roles.FirstOrDefault()
                };

                return new Response<UserDto>
                {
                    Succeeded = true,
                    StatusCode = (int)HttpStatusCode.Ok,
                    Data = userDto
                };
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, "GetUserById");
                throw;
            }

        }

        public async Task<Response> UpdateUser(string id, UpdateUserDto model, string loggedInUserId)
        {
            try
            {
                var validModel = await Check.ValidateAsync(model, _validatorFactory);
                if (!validModel.Succeeded)
                    return new Response { BrokenRules = validModel.BrokenRules, StatusCode = validModel.StatusCode };

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return new Response { Message = "User not found" };

                if (model.FirstName != null) user.FirstName = model.FirstName;
                if (model.MiddleName != null) user.MiddleName = model.MiddleName;
                if (model.LastName != null) user.LastName = model.LastName;
                if (model.FullNameEn != null) user.FullNameEn = model.FullNameEn;
                if (model.FirstNameAr != null) user.FirstNameAr = model.FirstNameAr;
                if (model.MiddleNameAr != null) user.MiddleNameAr = model.MiddleNameAr;
                if (model.LastNameAr != null) user.LastNameAr = model.LastNameAr;
                if (model.FullNameAr != null) user.FullNameAr = model.FullNameAr;
                if (model.DateOfBirth.HasValue) user.DateOfBirth = model.DateOfBirth.Value;
                if (model.Gender.HasValue) user.Gender = model.Gender.Value;
                if (model.Nationality != null) user.Nationality = model.Nationality;
                if (model.PhoneNumber != null) user.PhoneNumber = model.PhoneNumber;
                if (model.MobileNumber != null) user.MobileNumber = model.MobileNumber;

                user.ModifiedOn = DateTime.UtcNow;
                user.ModifiedBy = loggedInUserId;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return new Response { Message = String.Join("\n", result.Errors.Select(e => e.Description)) };
                }

                await _logger.LogInformation($"User {user.Email} updated successfully");

                return new Response
                {
                    Succeeded = true,
                    StatusCode = (int)HttpStatusCode.Ok,
                    Message = "User updated successfully"
                };
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, "UpdateUser");
                throw;
            }

        }

        public async Task<Response<object>> Register(RegisterDto model, string loggedInUserId)
        {
            try
            {
                var validModel = await Check.ValidateAsync(model, _validatorFactory);
                if (!validModel.Succeeded)
                    return new Response<AuthResponseDto>() { BrokenRules = validModel.BrokenRules, StatusCode = validModel.StatusCode };

                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return new Response<object> { Message = "Email already registered" };
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = false,
                    FirstName = model.FirstName,
                    MiddleName = model.MiddleName,
                    LastName = model.LastName,
                    FullNameEn = model.FirstName + " " + model.LastName,
                    FirstNameAr = model.FirstNameAr,
                    MiddleNameAr = model.MiddleNameAr ?? "",
                    LastNameAr = model.LastNameAr,
                    FullNameAr = model.FirstNameAr + " " + model.LastNameAr,
                    DateOfBirth = DateTime.UtcNow.AddYears(-25),
                    Gender = model.GenderId,
                    Nationality = model.Nationality,
                    PhoneNumber = model.PhoneNumber,
                    MobileNumber = model.MobileNumber,
                    IsActive = true,
                    CreatedBy = loggedInUserId,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    return new Response<object> { Message = String.Join("\n", result.Errors.Select(e => e.Description)) };
                }

                await _userManager.AddToRoleAsync(user, model.Role);

                await _logger.LogInformation($"User {user.Email} registered successfully");

                return new Response<object>
                {
                    Succeeded = true,
                    StatusCode = (int)HttpStatusCode.Ok,
                    Data = new
                    {
                        Message = "User registered successfully",
                        UserId = user.Id,
                        Email = user.Email
                    }
                };
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, "Register");
                throw;
            }

        }

        public async Task<Response> DeleteUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return new Response { Message = "User not found" };

                user.IsDeleted = true;
                user.IsActive = false;
                user.DeletedOn = DateTime.UtcNow;
                user.DeletedBy = id;

                user.RefreshTokenRevokedOn = DateTime.UtcNow;
                user.RefreshTokenRevokedByIp = _commonService.GetIpAddress();

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return new Response { Message = String.Join("\n", result.Errors.Select(e => e.Description)) };
                }

                await _logger.LogInformation($"User {user.Email} deleted by {id}");

                return new Response
                {
                    Succeeded = true,
                    StatusCode = (int)HttpStatusCode.Ok,
                    Message = "User deleted successfully"
                };
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, "DeleteUser");
                throw;
            }

        }

        public async Task<Response> ChangeUserStatus(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return new Response { Message = "User not found" };

                user.IsActive = !user.IsActive;
                user.ModifiedOn = DateTime.UtcNow;
                user.ModifiedBy = id;

                if (user.IsActive)
                {
                    user.RefreshTokenRevokedOn = DateTime.UtcNow;
                    user.RefreshTokenRevokedByIp = _commonService.GetIpAddress();
                }

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return new Response { Message = String.Join("\n", result.Errors.Select(e => e.Description)) };
                }

                await _logger.LogInformation($"User {user.Email} status changed to {(user.IsActive ? "active" : "inactive")}");

                return new Response { Message = $"User {(user.IsActive ? "activated" : "deactivated")} successfully" };
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, "ChangeUserStatus");
                throw;
            }

        }

        public async Task<Response<object>> GetRoles()
        {
            try
            {
                var roles = _roleManager.Roles.ToList();

                return new Response<object>
                {
                    Succeeded = true,
                    StatusCode = (int)HttpStatusCode.Ok,
                    Data = roles
                };
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, "GetRoles");
                throw;
            }

        }
    }
}
