using ManagementBE.Kernel.Core.Configurations;
using ManagementBE.Kernel.Core.Infrastructure;
using ManagementBE.Kernel.Domain.Identity;
using ManagementBE.Kernel.Host.Base.Configurations.Auth.Jwt;
using ManagementBE.Kernel.Host.Base.Configurations.Auth.Permissions;
using ManagementBE.Kernel.Infrastructure.Persistence.UnitOfWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Host.Base.Configurations.Auth
{
    internal static class AuthenticationRegistration
    {
        public static Configuration _configuration;
        internal static IServiceCollection AddAuth(this IServiceCollection services, Configuration configuration)
        {
            services
                .AddCurrentUser()
                .AddPermissions()
                .AddIdentity(configuration);

            return services.AddJwtAuth();
        }

        internal static IServiceCollection AddIdentity(this IServiceCollection services, Configuration configuration) =>


        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.User.RequireUniqueEmail = false;
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(configuration.IdentitySettings.LockoutDurationInMinutes);
            options.Lockout.MaxFailedAccessAttempts = configuration.IdentitySettings.MaxFailedAccessAttempts;
        
        
        })
        .AddEntityFrameworkStores<UnitOfWork>()
        .AddDefaultTokenProviders().Services;


        internal static IApplicationBuilder UseCurrentUser(this IApplicationBuilder app) =>
                app.UseMiddleware<CurrentUserMiddleware>();

        private static IServiceCollection AddCurrentUser(this IServiceCollection services) =>
            services
                .AddScoped<CurrentUserMiddleware>()
                .AddScoped<ICurrentUser, CurrentUser>()
                .AddScoped(sp => (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUser>());

        private static IServiceCollection AddPermissions(this IServiceCollection services) =>
            services
                .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
                .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
    }
}
