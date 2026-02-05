using ManagementBE.Kernel.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Core.Infrastructure
{
    public interface ICommonService
    {
        string GetCurrentLanguge();
        string GetIpAddress();
    }
}
