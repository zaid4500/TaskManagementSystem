using ManagementBE.Kernel.Core.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Host.Base.Configurations.Auth.Jwt
{
    internal static class JwtAuthenticationRegistration
    {
        internal static IServiceCollection AddJwtAuth(this IServiceCollection services)
        {
            services.AddOptions<JwtSettings>()
                .BindConfiguration($"SecuritySettings:{nameof(JwtSettings)}")
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

            return services
                .AddAuthentication(authentication =>
                {
                    authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, null!)
                .Services;
        }
    }
}
