using ManagementBE.Kernel.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Core.Infrastructure
{
    public interface IAuditService
    {
        Task<List<AuditDto>> GetUserTrailsAsync(Guid userId);
    }
}
