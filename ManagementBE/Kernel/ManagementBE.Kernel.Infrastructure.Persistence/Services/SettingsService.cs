using ManagementBE.Kernel.Core.Exceptions;
using ManagementBE.Kernel.Core.Infrastructure;
using ManagementBE.Kernel.Core.Persistence;
using ManagementBE.Kernel.Core.Services;
using ManagementBE.Kernel.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Services
{
    public class SettingsService:ISettingsService, IApplicationService
    {
        readonly IRepository<Setting> _settingRepository;
        readonly IApplicationLoggerService _logger;
        public SettingsService(IRepository<Setting> settingRepository, IApplicationLoggerService logger)
        {
            _settingRepository = settingRepository;
            _logger = logger;

        }

        public async Task<T> GetValue<T>(string key)
        {
            try
            {
                var setting = await _settingRepository.GetAll(s => s.Id.ToLower() == key.ToLower()).AsNoTracking().FirstOrDefaultAsync();
                if (setting != null)
                    return (T)Convert.ChangeType(setting.Value, typeof(T));
                else
                    return default(T);
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, "SettingsService-GetValue", key);
                throw new InternalServerException("Error Occurred on process your request");
            }
        }
    }
}
