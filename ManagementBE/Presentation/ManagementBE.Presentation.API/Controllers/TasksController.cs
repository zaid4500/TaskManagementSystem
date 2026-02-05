using ManagementBE.Kernel.Core.Controllers;
using ManagementBE.Kernel.Domain.DTOs;
using ManagementBE.Kernel.Domain.DTOs.Identity;
using ManagementBE.Presentation.Application.Services.Identity.Users;
using ManagementBE.Presentation.Application.Services.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ManagementBE.Presentation.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Tasks")]
    public class TasksController : BaseApiController
    {
        readonly ITasksService _tasksService;

        public TasksController(ITasksService tasksService)
        {
            _tasksService = tasksService;
        }


        /// <summary>
        /// Get all tasks that are assigned to the loggedIn user
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserTasks(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool includeDeleted = false)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _tasksService.GetTasks(currentUserId,page, pageSize, search, includeDeleted);
            return Ok(result);
        }

        /// <summary>
        /// Get all tasks assigned for all users (Admin only)
        /// </summary>
        [HttpGet("all-tasks")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTasks(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool includeDeleted = false)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _tasksService.GetTasks(currentUserId, page, pageSize, search, includeDeleted,true);
            return Ok(result);
        }

        /// <summary>
        /// Get Task by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTaskById(string id)
        {
            var result = await _tasksService.GetTaskById(id);
            return Ok(result);
        }

        /// <summary>
        /// Update Task profile (Admin Only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateTask(string id, [FromBody] TaskDto model)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _tasksService.UpdateTask(id, model,currentUserId);
            return Ok(result);
        }

        /// <summary>
        /// Create a new Task (Admin Only)
        /// </summary>
        [HttpPost("Create")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] TaskDto model)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _tasksService.Create(model,currentUserId);
            return Ok(result);
        }


        /// <summary>
        /// Delete Task (Admin only) - Soft delete
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteTask(string id)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _tasksService.DeleteTask(id,currentUserId);
            return Ok(result);
        }

        /// <summary>
        /// Change task status
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeTaskStatus(string id, [FromBody] int newStatusId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _tasksService.ChangeTaskStatus(id, newStatusId,currentUserId);
            return Ok(result);
        }
    }
}
