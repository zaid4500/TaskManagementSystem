using ManagementBE.Kernel.Core.Services;
using ManagementBE.Kernel.Core.Wrappers;
using ManagementBE.Kernel.Domain.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Presentation.Application.Services.Common
{
    public interface ILookupsService : IApplicationService
    {
        Task<Response<List<LookupDto>>> GetGenders();
        Task<Response<List<LookupDto>>> GetTaskStatuses();
    }
}
