using ManagementBE.Kernel.Core.Common;
using ManagementBE.Kernel.Core.Configurations;
using ManagementBE.Kernel.Core.Infrastructure;

namespace ManagementBE.Kernel.Core
{
    public class CrossCuttingDependencies : ICrossCuttingDependencies
    {
        public CrossCuttingDependencies(IApplicationLoggerService logger, Configuration config)
        {
            Logger = logger;
            Configuration = config;
        }

        public Configuration Configuration { get; }
        public IApplicationLoggerService Logger { get; }
    }
}
