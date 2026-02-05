using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Domain.Entities.Base
{
    public abstract class DomainEvent : IEvent
    {
        public DateTime TriggeredOn { get; protected set; } = DateTime.UtcNow;
    }
}
