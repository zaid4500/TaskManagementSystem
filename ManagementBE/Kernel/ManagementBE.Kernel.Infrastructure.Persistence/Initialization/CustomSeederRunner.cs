using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Initialization
{
    public class CustomSeederRunner
    {
        private readonly ICustomSeeder[] _seeders;

        public CustomSeederRunner(IServiceProvider serviceProvider) =>
            _seeders = serviceProvider.GetServices<ICustomSeeder>().ToArray();

        public async Task RunSeedersAsync(CancellationToken cancellationToken)
        {
            foreach (var seeder in _seeders)
            {
                await seeder.InitializeAsync(cancellationToken);
            }
        }
    }
}
