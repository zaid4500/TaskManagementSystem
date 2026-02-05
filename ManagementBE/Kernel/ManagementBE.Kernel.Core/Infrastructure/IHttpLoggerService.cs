using ManagementBE.Kernel.Domain.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Core.Infrastructure
{
    public interface IHttpLoggerService
    {
        Task Log(HttpLog httpLog);
    }
}
