using FluentValidation;
using ManagementBE.Kernel.Core.Enums;
using ManagementBE.Kernel.Core.Infrastructure;
using ManagementBE.Kernel.Core.Persistence;
using ManagementBE.Kernel.Core.Validators;
using ManagementBE.Kernel.Core.Wrappers;
using ManagementBE.Kernel.Domain.DTOs;
using ManagementBE.Kernel.Domain.DTOs.Identity;
using ManagementBE.Kernel.Domain.Identity;
using ManagementBE.Kernel.Domain.Tasks;
using ManagementBE.Kernel.Infrastructure.Persistence.Services;
using ManagementBE.Presentation.Application.Validators.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Presentation.Application.Services.Tasks
{
    public class TasksService : ITasksService
    {
        readonly IRepository<TaskEntity> _tasksRepository;
        readonly IApplicationLoggerService _logger;
        readonly IValidatorFactory _validatorFactory;

        public TasksService(IRepository<TaskEntity> tasksRepository, IApplicationLoggerService logger, IValidatorFactory validatorFactory)
        {
            _tasksRepository = tasksRepository;
            _logger = logger;
            _validatorFactory = validatorFactory;
        }

        public async Task<Response<object>> GetTasks(string loggedInUserId, int page = 1, int pageSize = 10, string? search = null, bool includeDeleted = false, bool includeAllTasks = false)
        {
            try
            {
                var query = _tasksRepository.GetAll().Include(x => x.Status)
                                                     .Include(x => x.AssignedToUser)
                                                     .AsQueryable();

                if (!includeAllTasks)
                {
                    query = query.Where(u => u.AssignedToUserId == loggedInUserId);
                }

                if (includeDeleted)
                {
                    query = query.IgnoreQueryFilters();
                }

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(u =>
                        (!String.IsNullOrEmpty(u.Title) && u.Title.Contains(search))
                    );
                }

                var totalCount = await query.CountAsync();
                var tasks = await query
                    .OrderByDescending(u => u.CreatedOn)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var taskDtos = new List<TaskDto>();
                foreach (var task in tasks)
                {
                    taskDtos.Add(new TaskDto
                    {
                        Id = task.Id,
                        Title = task.Title,
                        Descreption = task.Descreption,
                        StatusId = task.StatusId,
                        AssignedToUserId = task.AssignedToUserId,
                        CreatedBy = task.CreatedBy,
                        CreatedOn = task.CreatedOn,
                        ModifiedBy = task.ModifiedBy,
                        ModifiedOn = task.ModifiedOn,
                        Status = new Kernel.Domain.DTOs.Common.LookupDto
                        {
                            Id = task.Status.Id,
                            LookupCategoryId = task.Status.LookupCategoryId,
                            NameEn = task.Status.NameEn,
                            NameAr = task.Status.NameAr,
                            Code = task.Status.Code
                        },
                        AssignedToUser = new UserDto
                        {
                            Id = task.AssignedToUser.Id,
                            Email = task.AssignedToUser.Email ?? string.Empty,
                            UserName = task.AssignedToUser.UserName ?? string.Empty,
                            PhoneNumber = task.AssignedToUser.PhoneNumber,
                            FirstName = task.AssignedToUser.FirstName,
                            MiddleName = task.AssignedToUser.MiddleName,
                            LastName = task.AssignedToUser.LastName,
                            FullNameEn = task.AssignedToUser.FullNameEn,
                            FirstNameAr = task.AssignedToUser.FirstNameAr,
                            MiddleNameAr = task.AssignedToUser.MiddleNameAr,
                            LastNameAr = task.AssignedToUser.LastNameAr,
                            FullNameAr = task.AssignedToUser.FullNameAr,
                            FullName = task.AssignedToUser.FullName,
                            DateOfBirth = task.AssignedToUser.DateOfBirth,
                            GenderId = task.AssignedToUser.Gender,
                            GenderDisplay = task.AssignedToUser.Gender == 1 ? "Male" : task.AssignedToUser.Gender == 2 ? "Female" : null,
                            Nationality = task.AssignedToUser.Nationality,
                            MobileNumber = task.AssignedToUser.MobileNumber,
                            IsActive = task.AssignedToUser.IsActive,
                            CreatedBy = task.AssignedToUser.CreatedBy,
                            CreatedOn = task.AssignedToUser.CreatedOn,
                            ModifiedBy = task.AssignedToUser.ModifiedBy,
                            ModifiedOn = task.AssignedToUser.ModifiedOn,
                        }
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
                        Items = taskDtos
                    }
                };
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, "GetUserTasks");
                throw;
            }

        }

        public async Task<Response<TaskDto>> GetTaskById(string id)
        {
            try
            {
                var task = await _tasksRepository.FindAsync(id);
                if (task == null)
                    return new Response<TaskDto> { Message = "Task not found" };

                var taskDto = new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Descreption = task.Descreption,
                    AssignedToUserId = task.AssignedToUserId,
                    StatusId = task.StatusId,
                    CreatedBy = task.CreatedBy,
                    CreatedOn = task.CreatedOn,
                    ModifiedBy = task.ModifiedBy,
                    ModifiedOn = task.ModifiedOn,
                };

                return new Response<TaskDto>
                {
                    Succeeded = true,
                    StatusCode = (int)HttpStatusCode.Ok,
                    Data = taskDto
                };
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, "GetTaskById");
                throw;
            }

        }

        public async Task<Response> UpdateTask(string id, TaskDto model, string loggedInUserId)
        {
            try
            {
                var validModel = await Check.ValidateAsync(model, _validatorFactory);
                if (!validModel.Succeeded)
                    return new Response { BrokenRules = validModel.BrokenRules, StatusCode = validModel.StatusCode };

                var task = await _tasksRepository.FindAsync(Guid.Parse(id));
                if (task == null)
                    return new Response { Message = "Task not found" };

                task.Title = model.Title;
                task.Descreption = model.Descreption;
                task.AssignedToUserId = model.AssignedToUserId;
                task.StatusId = model.StatusId;

                task.ModifiedOn = DateTime.UtcNow;
                task.ModifiedBy = loggedInUserId;

                _tasksRepository.Update(task);
                await _tasksRepository.UnitOfWork.SaveChangesAsync();

                await _logger.LogInformation($"Task {task.Id} updated successfully");

                return new Response
                {
                    Succeeded = true,
                    StatusCode = (int)HttpStatusCode.Ok,
                    Message = "Task updated successfully"
                };
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, "UpdateTask");
                throw;
            }

        }

        public async Task<Response<object>> Create(TaskDto model, string loggedInUserId)
        {
            try
            {
                var validModel = await Check.ValidateAsync(model, _validatorFactory);
                if (!validModel.Succeeded)
                    return new Response<object>() { BrokenRules = validModel.BrokenRules, StatusCode = validModel.StatusCode };


                var task = new TaskEntity
                {
                    Title = model.Title,
                    Descreption = model.Descreption,
                    AssignedToUserId = model.AssignedToUserId,
                    StatusId = model.StatusId,
                    CreatedBy = loggedInUserId,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _tasksRepository.InsertAsync(task);
                await _tasksRepository.UnitOfWork.SaveChangesAsync();

                await _logger.LogInformation($"Task {task.Title} Created successfully");

                return new Response<object>
                {
                    Succeeded = true,
                    StatusCode = (int)HttpStatusCode.Ok,
                    Data = new
                    {
                        Message = "Task Created successfully",
                        TaskId = task.Id,
                        Title = task.Title
                    }
                };
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, "Tasks - Create");
                throw;
            }

        }

        public async Task<Response> DeleteTask(string id, string loggedInUserId)
        {
            try
            {
                var task = await _tasksRepository.FindAsync(Guid.Parse(id));
                if (task == null)
                    return new Response { Message = "Task not found" };

                task.IsDeleted = true;
                task.DeletedOn = DateTime.UtcNow;
                task.DeletedBy = loggedInUserId;


                _tasksRepository.Update(task);
                await _tasksRepository.UnitOfWork.SaveChangesAsync();

                await _logger.LogInformation($"Task {task.Id} deleted by {loggedInUserId}");

                return new Response
                {
                    Succeeded = true,
                    StatusCode = (int)HttpStatusCode.Ok,
                    Message = "Task deleted successfully"
                };
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, "DeleteTask");
                throw;
            }

        }

        public async Task<Response> ChangeTaskStatus(string id, int newStatusId, string loggedInUserId)
        {
            try
            {
                var task = await _tasksRepository.FindAsync(id);
                if (task == null)
                    return new Response { Message = "Task not found" };

                task.StatusId = newStatusId;
                task.ModifiedOn = DateTime.UtcNow;
                task.ModifiedBy = loggedInUserId;

                _tasksRepository.Update(task);
                await _tasksRepository.UnitOfWork.SaveChangesAsync();

                await _logger.LogInformation($"Task with Id {task.Id} status changed to new status Id {task.StatusId}");

                return new Response { Message = $"Task status changed successfully" };
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, "ChangeTaskStatus");
                throw;
            }

        }

    }
}
