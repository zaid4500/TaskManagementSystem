using ManagementBE.Kernel.Core.Enums;
using ManagementBE.Kernel.Core.Infrastructure;
using ManagementBE.Kernel.Core.Persistence;
using ManagementBE.Kernel.Core.Wrappers;
using ManagementBE.Kernel.Domain.Common;
using ManagementBE.Kernel.Domain.DTOs.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Presentation.Application.Services.Common
{
    public class LookupsService : ILookupsService
    {
        readonly IRepository<Lookup> _lookupsRepository;
        readonly IApplicationLoggerService _logger;

        public LookupsService(IRepository<Lookup> lookupsRepository, IApplicationLoggerService logger)
        {
            _lookupsRepository = lookupsRepository;
            _logger = logger;
        }


        public async Task<Response<List<LookupDto>>> GetGenders()
        {
            return await GetLookupsByCategoryCode("GENDER");
        }

        public async Task<Response<List<LookupDto>>> GetTaskStatuses()
        {
            return await GetLookupsByCategoryCode("TASK_STATUS");
        }

        async Task<Response<List<LookupDto>>> GetLookupsByCategoryCode(string categoryCode)
        {
            var lookups = await _lookupsRepository.GetAll()
                .Include(l => l.LookupCategory)
                .Where(l => l.LookupCategory.Code == categoryCode)
                .Select(l => new LookupDto
                {
                    Id = l.Id,
                    LookupCategoryId = l.LookupCategoryId,
                    NameEn = l.NameEn,
                    NameAr = l.NameAr,
                    Code = l.Code
                })
                .ToListAsync();

            return new Response<List<LookupDto>>
            {
                Succeeded = true,
                StatusCode = (int)HttpStatusCode.Ok,
                Data = lookups
            };
        }
    }
}
