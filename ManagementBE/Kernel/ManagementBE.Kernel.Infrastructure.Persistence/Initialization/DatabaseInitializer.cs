using ManagementBE.Kernel.Core.Infrastructure;
using ManagementBE.Kernel.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Initialization
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        readonly IServiceProvider _serviceProvider;

        public DatabaseInitializer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task InitializeDatabasesAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            await scope.ServiceProvider.GetRequiredService<ApplicationDbInitializer>()
            .InitializeAsync(cancellationToken);
        }
    }
}
