using ManagementBE.Kernel.Core.Authorization;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Host.Base.Configurations.Auth.Permissions
{
    public class MustHavePermissionAttribute : AuthorizeAttribute
    {
        public MustHavePermissionAttribute(string action, string resource) =>
            Policy = ManagementBEPermission.NameFor(action, resource);
    }
}
