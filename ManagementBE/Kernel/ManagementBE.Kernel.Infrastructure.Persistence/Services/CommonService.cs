using ManagementBE.Kernel.Core.Constants;
using ManagementBE.Kernel.Core.Infrastructure;
using ManagementBE.Kernel.Core.Persistence;
using ManagementBE.Kernel.Core.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Services
{
    public class CommonService : ICommonService, IApplicationService
    {
        readonly ICacheService _cache;
        readonly IApplicationLoggerService _logger;
        readonly IHttpContextAccessor _httpContextAccessor;

        public CommonService(
            IHttpContextAccessor httpContextAccessor,
            ICacheService cache,
            IApplicationLoggerService logger
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
            _logger = logger;
        }

        public string GetCurrentLanguge()
        {
            string culture = Languages.Arabic;

            HttpContext context = _httpContextAccessor.HttpContext;
            if (context.Request.Headers.TryGetValue(RequestHeaders.Language, out var languageValues))
                culture = languageValues.FirstOrDefault()?.ToLower();

            if (culture != Languages.English && culture != Languages.Arabic)
                culture = Languages.Arabic;

            return culture;
        }

        public DateTime ConvertToLocalTime(DateTime dateTime)
        {
            try
            {

                var jordanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Jordan Standard Time");
                var jordanTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, jordanTimeZone);
                return jordanTime;


            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public string GetIpAddress()
        {
            HttpContext context = _httpContextAccessor.HttpContext;
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
                return context.Request.Headers["X-Forwarded-For"].ToString();

            return context.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "Unknown";
        }
    }
}
