using ManagementBE.Kernel.Core.Configurations;
using ManagementBE.Kernel.Core.Infrastructure;

namespace ManagementBE.Kernel.Core.Common
{
    public interface ICrossCuttingDependencies
    {
        IApplicationLoggerService Logger { get; }
        Configuration Configuration { get; }
    }
}
