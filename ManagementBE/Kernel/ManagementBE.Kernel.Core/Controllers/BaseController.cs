using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using ManagementBE.Kernel.Core.Wrappers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ManagementBE.Kernel.Core.Constants;

namespace ManagementBE.Kernel.Core.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected static IHttpContextAccessor _httpContextAccessor { get { return new HttpContextAccessor(); } }


        //public override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    base.OnActionExecuting(context);
        //}
        protected ActionResult ProcessResponse<T>(T data)
        {
            var response = new Response<T>(data);

            if (data == null)
            {
                response.Succeeded = false;
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = "Error"; 
                return StatusCode((int)response.StatusCode, response);
            }

            response.Message = "Success";
            return StatusCode((int)response.StatusCode, response);
        }
        protected ActionResult Ok(Response response)
        {
            if (string.IsNullOrEmpty(response.Message))
            {
                if (response.Succeeded)
                    response.Message = "Success"; 
                else
                    response.Message = "Error";
            }
            return StatusCode((int)response.StatusCode, response);
        }
        protected string ConvertViewToString(ControllerContext controllerContext, PartialViewResult pvr, ICompositeViewEngine _viewEngine)
        {
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = _viewEngine.FindView(controllerContext, pvr.ViewName, false);
                ViewContext viewContext = new ViewContext(controllerContext, vResult.View, pvr.ViewData, pvr.TempData, writer, new HtmlHelperOptions());
                vResult.View.RenderAsync(viewContext);
                return writer.GetStringBuilder().ToString();
            }
        }
    }
}
