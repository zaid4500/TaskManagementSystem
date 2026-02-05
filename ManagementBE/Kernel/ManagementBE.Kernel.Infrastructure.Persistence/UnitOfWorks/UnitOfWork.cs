using Microsoft.EntityFrameworkCore;
using ManagementBE.Kernel.Core.Persistence;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ManagementBE.Kernel.Infrastructure.Persistence.UnitOfWorks
{
    /// <summary>
    /// Unit Of Work
    /// </summary>
    public class UnitOfWork : UnitOfWorkBase<IEntityMapping>
    {
        public UnitOfWork(DbContextOptions<UnitOfWork> options) : base(options) { }
    }
}
