using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Core.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        DatabaseFacade Database { get; }

        void BeginTransaction();
        Task<int> SaveChangesAsync();
        void Commit();
        void Rollback();
        IEnumerable<string> GetMigrations();
        Task<bool> CanConnectAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> GetPendingMigrationsAsync(CancellationToken cancellationToken = default);
        Task MigrateAsync(CancellationToken cancellationToken = default);

    }
}
