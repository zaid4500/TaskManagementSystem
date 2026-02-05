using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Core.Infrastructure
{
    public interface ICurrentUser
    {
        string? Name { get; }

        Guid GetUserId();

        string? GetUserEmail();

        bool IsAuthenticated();

        bool IsInRole(string role);

        IEnumerable<Claim>? GetUserClaims();
    }
}
