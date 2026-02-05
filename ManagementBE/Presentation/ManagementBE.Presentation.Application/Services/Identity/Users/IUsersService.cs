using ManagementBE.Kernel.Core.Services;
using ManagementBE.Kernel.Core.Wrappers;
using ManagementBE.Kernel.Domain.DTOs.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Presentation.Application.Services.Identity.Users
{
    public interface IUsersService : IApplicationService
    {
        Task<Response> ChangeUserStatus(string id);
        Task<Response> DeleteUser(string id);
        Task<Response<object>> GetAllUsers(int page = 1, int pageSize = 10, string? search = null, bool includeDeleted = false);
        Task<Response<object>> GetRoles();
        Task<Response<UserDto>> GetUserById(string id);
        Task<Response<object>> Register(RegisterDto model,string loggedInUserId);
        Task<Response> UpdateUser(string id, UpdateUserDto model, string loggedInUserId);
    }
}
