using ManagementBE.Kernel.Core.Exceptions;
using ManagementBE.Kernel.Core.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
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
    internal class ExceptionMiddleware : IMiddleware
    {
        private readonly ICurrentUser _currentUser;
        private readonly ISerializerService _jsonSerializer;
        public ExceptionMiddleware(
        ICurrentUser currentUser,
        ISerializerService jsonSerializer)
        {
            _currentUser = currentUser;
            _jsonSerializer = jsonSerializer;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                string email = _currentUser.GetUserEmail() is string userEmail ? userEmail : "Anonymous";
                var userId = _currentUser.GetUserId();
                if (userId != Guid.Empty) LogContext.PushProperty("UserId", userId);
                LogContext.PushProperty("UserEmail", email);
                string errorId = Guid.NewGuid().ToString();
                LogContext.PushProperty("ErrorId", errorId);
                LogContext.PushProperty("StackTrace", exception.StackTrace);
                var errorResult = new ErrorResult
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

                var response = context.Response;
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
            }
        }
    }
}
