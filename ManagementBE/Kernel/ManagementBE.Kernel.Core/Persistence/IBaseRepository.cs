using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementBE.Kernel.Core.Persistence
{
    public interface IBaseRepository
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
