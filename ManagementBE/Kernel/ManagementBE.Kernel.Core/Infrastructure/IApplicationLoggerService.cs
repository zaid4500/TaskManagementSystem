using ManagementBE.Kernel.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Core.Infrastructure
{
    public interface IApplicationLoggerService
    {
        Task LogError(Exception ex, string service, object request = null);
        Task LogWarning(string message, object details = null, string service = null);
        Task LogInformation(string message, object details = null, string service = null, object request = null, object response = null, HttpStatusCode statusCode = HttpStatusCode.None, LogServerity resultType = LogServerity.Info);
        Task LogDebug(string message, object details = null, string service = null, object request = null, object response = null);
    }
}
