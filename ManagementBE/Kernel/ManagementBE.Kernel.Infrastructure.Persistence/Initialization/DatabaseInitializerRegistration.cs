using ManagementBE.Kernel.Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Initialization
{
    public static class DatabaseInitializerRegistration
    {
        public static async Task InitializeDatabasesAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
        {
            using var scope = services.CreateScope();
            await scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>()
            .InitializeDatabasesAsync(cancellationToken);
        }
    }
}
