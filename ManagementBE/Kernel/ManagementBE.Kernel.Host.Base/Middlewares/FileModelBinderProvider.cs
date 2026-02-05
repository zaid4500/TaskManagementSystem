using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Host.Base.Middlewares
{

    public class FileModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var file = bindingContext.ActionContext.HttpContext.Request.Form.Files[0];
            bindingContext.Result = ModelBindingResult.Success(file);
            return Task.CompletedTask;
        }
    }


    public class FileModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(IFormFile))
            {
                return new BinderTypeModelBinder(typeof(FileModelBinder));
            }

            return null;
        }
    }
}
