using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Contexts;
using NSwag.Generation.Processors;
using NSwag;
using System.Reflection;

namespace ManagementBE.Kernel.Host.Base.Configurations.Swagger
{

    internal static class ObjectExtensions
    {
        public static T? TryGetPropertyValue<T>(this object? obj, string propertyName, T? defaultValue = default) =>
            obj?.GetType().GetRuntimeProperty(propertyName) is PropertyInfo propertyInfo
                ? (T?)propertyInfo.GetValue(obj)
                : defaultValue;
    }

    /// <summary>
    /// The default NSwag AspNetCoreOperationProcessor doesn't take .RequireAuthorization() calls into account
    /// Unless the AllowAnonymous attribute is defined, this processor will always add the security scheme
    /// when it's not already there, so effectively adding "Global Auth".
    /// </summary>
    public class SwaggerGlobalAuthProcessor : IOperationProcessor
    {
        private readonly string _name;

        public SwaggerGlobalAuthProcessor()
            : this(JwtBearerDefaults.AuthenticationScheme)
        {
        }

        public SwaggerGlobalAuthProcessor(string name)
        {
            _name = name;
        }

        public bool Process(OperationProcessorContext context)
        {
            IList<object>? list = ((AspNetCoreOperationProcessorContext)context).ApiDescription?.ActionDescriptor?.TryGetPropertyValue<IList<object>>("EndpointMetadata");
            if (list is not null)
            {
                if (list.OfType<AllowAnonymousAttribute>().Any())
                {
                    return true;
                }

                if (context.OperationDescription.Operation.Security?.Any() != true)
                {
                    (context.OperationDescription.Operation.Security ??= new List<OpenApiSecurityRequirement>()).Add(new OpenApiSecurityRequirement
                {
                    {
                        _name,
                        Array.Empty<string>()
                    }
                });
                }
            }

            return true;
        }
    }




    public class SwaggerHeaderAttributeProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            if (context.MethodInfo?.GetCustomAttribute(typeof(SwaggerHeaderAttribute)) is SwaggerHeaderAttribute attribute)
            {
                var parameters = context.OperationDescription.Operation.Parameters;

                var existingParam = parameters.FirstOrDefault(p =>
                    p.Kind == OpenApiParameterKind.Header && p.Name == attribute.HeaderName);
                if (existingParam is not null)
                {
                    parameters.Remove(existingParam);
                }

                parameters.Add(new OpenApiParameter
                {
                    Name = attribute.HeaderName,
                    Kind = OpenApiParameterKind.Header,
                    Description = attribute.Description,
                    IsRequired = attribute.IsRequired,
                    Schema = new NJsonSchema.JsonSchema
                    {
                        Type = NJsonSchema.JsonObjectType.String,
                        Default = attribute.DefaultValue
                    }
                });
            }

            return true;
        }
    }


}
