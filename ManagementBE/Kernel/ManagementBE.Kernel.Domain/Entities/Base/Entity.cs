using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementBE.Kernel.Domain.Entities.Base
{
    public abstract class Entity<T>
    {
        public T Id { get; set; } = default!;
    }
}
