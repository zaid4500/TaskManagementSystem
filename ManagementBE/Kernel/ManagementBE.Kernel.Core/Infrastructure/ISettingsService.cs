using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Core.Infrastructure
{
    public interface ISettingsService
    {
        Task<T> GetValue<T>(string key);
    }
}
