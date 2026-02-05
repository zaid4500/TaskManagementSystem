using ManagementBE.Kernel.Core.Configurations;
using ManagementBE.Kernel.Core.Exceptions;
using ManagementBE.Kernel.Core.Infrastructure;
using ManagementBE.Kernel.Domain.Logs;
using Elastic.Apm.Api;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Host.Base.Middlewares
{
    internal class RequestResponseLoggerMiddleware : IMiddleware
    {
        readonly ICurrentUser _currentUser;
        readonly IHttpLoggerService _logger;
        readonly ISerializerService _jsonSerializer;
        Configuration _config { get; }

        public RequestResponseLoggerMiddleware(Configuration config, ICurrentUser currentUser, 
            IHttpLoggerService logger, ISerializerService jsonSerializer)
        {
            _config = config;
            _currentUser = currentUser;
            _logger = logger;
            _jsonSerializer = jsonSerializer;
        }

        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            if (!_config.RequestResponseLogger.Enabled || httpContext.Request.Path.ToString().Contains("/swagger/"))
            {
                await next(httpContext);
                return;
            }

            HttpLog log = new HttpLog();
            HttpRequest request = httpContext.Request;

            log.RequestDateTime = DateTime.Now;
            var ip = request.HttpContext.Connection.RemoteIpAddress;
            log.IPAddress = ip == null ? null : ip.ToString();
            if (log.IPAddress == "::1")
                log.IPAddress = "127.0.0.1";

            log.Node = _config.RequestResponseLogger.Name;

            /*request*/
            log.RequestMethod = request.Method;
            log.RequestPath = request.Path;
            log.RequestQuery = request.QueryString.ToString();
            log.RequestQueries = JsonConvert.SerializeObject(FormatQueries(request.QueryString.ToString()));
            log.RequestHeaders = JsonConvert.SerializeObject(FormatHeaders(request.Headers));
            log.RequestBody = JsonConvert.SerializeObject(await ReadBodyFromRequest(request));
            log.RequestScheme = request.Scheme;
            log.RequestHost = request.Host.ToString();
            log.RequestContentType = request.ContentType;


            HttpResponse response = httpContext.Response;
            var originalResponseBody = response.Body;
            using var newResponseBody = new MemoryStream();
            response.Body = newResponseBody;

            ErrorResult errorResult = new ErrorResult();
            string email = string.Empty;
            try
            {
                await next(httpContext);
            }
            catch (Exception exception)
            {
                email = _currentUser.GetUserEmail() is string userEmail ? userEmail : "Anonymous";
                var userId = _currentUser.GetUserId();
                if (userId != Guid.Empty) LogContext.PushProperty("UserId", userId);
                LogContext.PushProperty("UserEmail", email);
                string errorId = Guid.NewGuid().ToString();
                LogContext.PushProperty("ErrorId", errorId);
                LogContext.PushProperty("StackTrace", exception.StackTrace);
                errorResult = new ErrorResult
                {
                    Source = exception.TargetSite?.DeclaringType?.FullName,
                    Exception = exception.Message.Trim(),
                    ErrorId = errorId,
                    Message = String.Format("Provide the ErrorId {0} to the support team for further analysis.", errorId)
                };

                if (exception is not CustomException && exception.InnerException != null)
                {
                    while (exception.InnerException != null)
                    {
                        exception = exception.InnerException;
                    }
                }

                switch (exception)
                {
                    case CustomException e:
                        errorResult.StatusCode = (int)e.StatusCode;
                        if (e.ErrorMessages is not null)
                        {
                            //errorResult.Messages = e.ErrorMessages;
                        }

                        break;

                    case KeyNotFoundException:
                        errorResult.StatusCode = (int)HttpStatusCode.NotFound;
                        break;

                    case FluentValidation.ValidationException:
                        errorResult.StatusCode = (int)ManagementBE.Kernel.Core.Enums.HttpStatusCode.BusinessRuleViolation;
                        break;

                    default:
                        errorResult.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                Log.Error($"{errorResult.Exception} Request failed with Status Code {errorResult.StatusCode} and Error Id {errorId}.");

                if (!response.HasStarted)
                {
                    response.ContentType = "application/json";
                    response.StatusCode = errorResult.StatusCode;
                    await response.WriteAsync(_jsonSerializer.Serialize(errorResult));
                }
                else
                {
                    Log.Warning("Can't write error response. Response has already started.");
                }

                LogError(log, exception);
            }

            newResponseBody.Seek(0, SeekOrigin.Begin);
            string responseBody;
            if (request.Path.ToString().Contains("tokens") || request.Path.ToString().Contains("token"))
                responseBody = "[Redacted] Contains Sensitive Information.";
            else
                responseBody = await new StreamReader(response.Body).ReadToEndAsync();

            newResponseBody.Seek(0, SeekOrigin.Begin);
            await newResponseBody.CopyToAsync(originalResponseBody);

            /*response*/
            log.ResponseContentType = response.ContentType;
            log.ResponseStatus = response.StatusCode.ToString();
            log.ResponseHeaders = JsonConvert.SerializeObject(FormatHeaders(response.Headers));
            log.ResponseBody = responseBody;
            log.ResponseDateTime = DateTime.UtcNow;
            TimeSpan ts = log.ResponseDateTime.Value - log.RequestDateTime;
            log.ActionPerionTime = ts.TotalMilliseconds;

            var contextFeature =
                httpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (contextFeature != null && contextFeature.Error != null)
            {
                Exception exception = contextFeature.Error;
                LogError(log, exception);
            }

            log.Username = !string.IsNullOrEmpty(email) ? email : null;

            await _logger.Log(log);
        }



        private async void LogError(HttpLog log, Exception exception)
        {
            log.IsExceptionActionLevel = true;
            log.ExceptionMessage = exception.Message;
            log.ExceptionStackTrace = exception.StackTrace;
            log.ExceptionDetails = JsonConvert.SerializeObject(exception);
        }

        private Dictionary<string, string> FormatHeaders(IHeaderDictionary headers)
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            foreach (var header in headers)
            {
                pairs.Add(header.Key, header.Value);
            }
            return pairs;
        }

        private List<KeyValuePair<string, string>> FormatQueries(string queryString)
        {
            List<KeyValuePair<string, string>> pairs =
                 new List<KeyValuePair<string, string>>();
            string key, value;
            foreach (var query in queryString.TrimStart('?').Split("&"))
            {
                var items = query.Split("=");
                key = items.Count() >= 1 ? items[0] : string.Empty;
                value = items.Count() >= 2 ? items[1] : string.Empty;
                if (!String.IsNullOrEmpty(key))
                {
                    pairs.Add(new KeyValuePair<string, string>(key, value));
                }
            }
            return pairs;
        }

        //private async Task<string> ReadBodyFromRequest(HttpRequest request)
        //{
        //    // Ensure the request's body can be read multiple times 
        //    // (for the next middlewares in the pipeline).
        //    request.EnableBuffering();
        //    using var streamReader = new StreamReader(request.Body, leaveOpen: true);
        //    var requestBody = await streamReader.ReadToEndAsync();
        //    // Reset the request's body stream position for 
        //    // next middleware in the pipeline.
        //    request.Body.Position = 0;
        //    return requestBody;
        //}

        private async Task<object> ReadBodyFromRequest(HttpRequest request)
        {
            // Ensure the request's body can be read multiple times 
            // (for the next middlewares in the pipeline).
            request.EnableBuffering();

            if (request.HasFormContentType)
            {
                // If it's a form request, build a dictionary excluding 'files'
                var formData = new Dictionary<string, string>();

                foreach (var formEntry in request.Form)
                {
                    // Exclude 'files' parameter
                    if (formEntry.Key != "files")
                    {
                        formData[formEntry.Key] = formEntry.Value.ToString();
                    }
                }

                // Reset the request's body stream position for 
                // next middleware in the pipeline.
                request.Body.Position = 0;
                return formData;
            }
            else
            {
                // For other content types (assuming JSON), read the entire body
                using var streamReader = new StreamReader(request.Body, leaveOpen: true);
                var requestBody = await streamReader.ReadToEndAsync();
                // Reset the request's body stream position for 
                // next middleware in the pipeline.
                request.Body.Position = 0;
                return requestBody;
            }
        }
    }
}
