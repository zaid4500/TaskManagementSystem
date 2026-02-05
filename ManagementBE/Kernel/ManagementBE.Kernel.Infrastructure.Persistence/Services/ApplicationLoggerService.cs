using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ManagementBE.Kernel.Core.Constants;
using ManagementBE.Kernel.Core.Enums;
using ManagementBE.Kernel.Core.Helpers;
using ManagementBE.Kernel.Core.Infrastructure;
using ManagementBE.Kernel.Domain.Logs;
using ManagementBE.Kernel.Infrastructure.Persistence.Repositories;
using ManagementBE.Kernel.Core.Services;
using ManagementBE.Kernel.Core.Persistence;
using System;


namespace ManagementBE.Kernel.Infrastructure.Persistence.Services
{
    /// <summary>
    /// Application Db Logger
    /// </summary>
    public class ApplicationLoggerService : IApplicationLoggerService, IApplicationService
    {
        #region Fields
        readonly IRepository<ApplicationLog> _logger;
        readonly IHttpContextAccessor _httpContextAccessor;

        readonly ICurrentUser _currentUser;
        #endregion

        #region Ctor
        public ApplicationLoggerService(IRepository<ApplicationLog> logger, ICurrentUser currentUser, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _currentUser = currentUser;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Log Debug
        /// </summary>
        /// <param name="message"></param>
        /// <param name="details"></param>
        /// <param name="service"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public async Task LogDebug(string message, object details = null, string service = null, object request = null, object response = null)
        {

            string strDetails = details != null ? JsonConvert.SerializeObject(details) : null;
            string strRequest = request != null ? JsonConvert.SerializeObject(request) : null;
            string strResponse = response != null ? JsonConvert.SerializeObject(response) : null;

            ApplicationLog applicationLog = PrepareLog();
            applicationLog.LogSeverityId = (int)LogServerity.Debug;
            applicationLog.Service = service;
            applicationLog.Message = message;
            applicationLog.Description = strDetails;
            applicationLog.MessageRequest = strRequest;
            applicationLog.MessageResponse = strResponse;

            await _logger.InsertAsync(applicationLog);
            await _logger.UnitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Log Error or Excpetion
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="service"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task LogError(Exception ex, string service, object request = null)
        {
            try
            {
                string error = JsonConvert.SerializeObject(ex);
                string strRequest = request != null ? JsonConvert.SerializeObject(request) : null;

                ApplicationLog applicationLog = PrepareLog();
                applicationLog.LogSeverityId = (int)LogServerity.Error;
                applicationLog.Service = service;
                applicationLog.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                applicationLog.Description = error;
                applicationLog.MessageRequest = strRequest;
                applicationLog.ResultType = LogServerity.Error.ToString();

                await _logger.InsertAsync(applicationLog);
                await _logger.UnitOfWork.SaveChangesAsync();
            }
            catch (Exception exc)
            {
                ApplicationLog applicationLog = PrepareLog();
                applicationLog.LogSeverityId = (int)LogServerity.Error;
                applicationLog.Service = service;
                applicationLog.Message = exc.Message;
                applicationLog.Description = exc.InnerException != null ? exc.InnerException.Message : exc.Message;
                applicationLog.ResultType = LogServerity.Error.ToString();
                await _logger.InsertAsync(applicationLog);
                await _logger.UnitOfWork.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Log Information
        /// </summary>
        /// <param name="message"></param>
        /// <param name="details"></param>
        /// <param name="service"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="statusCode"></param>
        /// <param name="resultType"></param>
        /// <returns></returns>
        public async Task LogInformation(string message, object details = null, string service = null, object request = null, object response = null, HttpStatusCode statusCode = HttpStatusCode.None, LogServerity resultType = LogServerity.Info)
        {
            string strDetails = details != null ? JsonConvert.SerializeObject(details) : null;
            string strRequest = request != null ? JsonConvert.SerializeObject(request) : null;
            string strResponse = response != null ? JsonConvert.SerializeObject(response) : null;

            ApplicationLog applicationLog = PrepareLog();
            applicationLog.LogSeverityId = (int)LogServerity.Info;
            applicationLog.Service = service;
            applicationLog.Message = message;
            applicationLog.Description = strDetails;
            applicationLog.MessageRequest = strRequest;
            applicationLog.MessageResponse = strResponse;
            applicationLog.ResultCode = ((int)statusCode).ToString();
            applicationLog.ResultType = resultType.ToString();


            await _logger.InsertAsync(applicationLog);
            await _logger.UnitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Log Warning
        /// </summary>
        /// <param name="message"></param>
        /// <param name="details"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public async Task LogWarning(string message, object details = null, string service = null)
        {
            string strDetails = details != null ? JsonConvert.SerializeObject(details) : null;

            ApplicationLog applicationLog = PrepareLog();
            applicationLog.LogSeverityId = (int)LogServerity.Info;
            applicationLog.Service = service;
            applicationLog.Message = message;
            applicationLog.Description = strDetails;
            applicationLog.ResultType = LogServerity.Warning.ToString();

            await _logger.InsertAsync(applicationLog);
            await _logger.UnitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Prepare public Log info
        /// </summary>
        /// <returns></returns>
        private ApplicationLog PrepareLog()
        {
            string userAgent = null, browser = null, IPAddress = null;
            if (_httpContextAccessor !=null && _httpContextAccessor.HttpContext != null)
            {
                userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
                browser = UAParser.Parser.GetDefault().Parse(userAgent).UA.Family;
                IPAddress = null;
                var ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
                IPAddress = ip == null ? null : ip.ToString();
                if (IPAddress == "::1")
                    IPAddress = "127.0.0.1";
            }

            return new ApplicationLog()
            {
                ApplicationName = SystemSettings.ApplicationName,
                LoggedAt = DateTime.Now,
                UserName = _currentUser.GetUserEmail()?.ToLower() ?? string.Empty,
                UserAgent = userAgent,
                BrowserName = browser,
                IPAddress = IPAddress,
            };
        }
        #endregion
    }
}
