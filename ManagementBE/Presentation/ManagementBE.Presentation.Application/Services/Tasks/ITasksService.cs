using ManagementBE.Kernel.Core.Services;
using ManagementBE.Kernel.Core.Wrappers;
using ManagementBE.Kernel.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Presentation.Application.Services.Tasks
{
    public interface ITasksService : IApplicationService
    {
        Task<Response> ChangeTaskStatus(string id, int newStatusId, string loggedInUserId);
        Task<Response<object>> Create(TaskDto model, string loggedInUserId);
        Task<Response> DeleteTask(string id, string loggedInUserId);
        Task<Response<TaskDto>> GetTaskById(string id);
        Task<Response<object>> GetTasks(string loggedInUserId, int page = 1, int pageSize = 10, string? search = null, bool includeDeleted = false, bool includeAllTasks = false);
        Task<Response> UpdateTask(string id, TaskDto model, string loggedInUserId);
    }
}
