using Microsoft.EntityFrameworkCore;
using ManagementBE.Kernel.Core.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ManagementBE.Kernel.Domain.Identity;

namespace ManagementBE.Kernel.Infrastructure.Persistence.UnitOfWorks
{
    /// <summary>
    /// Unit Of Work Base
    /// </summary>
    /// <typeparam name="TMappingInterface"></typeparam>
    public abstract class UnitOfWorkBase<TMappingInterface> : IdentityDbContext<ApplicationUser, IdentityRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>, IUnitOfWork
    {
        /// <summary>
        /// disposed variable  
        /// </summary>
        private bool disposed = false;
        public UnitOfWorkBase() { }
        /// <summary>
        /// constructor of Unit Of Wor kBase
        /// </summary>
        /// <param name="options"></param>
        protected UnitOfWorkBase(DbContextOptions options) : base(options)
        {
        }
        /// <summary>
        /// On Model Creating method 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            RegisterMappings(modelBuilder);
        }
        /// <summary>
        /// Register Mappings
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void RegisterMappings(ModelBuilder modelBuilder)
        {
            var mappingAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.ManifestModule != null && x.ManifestModule.Name.StartsWith("ManagementBE.", StringComparison.OrdinalIgnoreCase)
            && x.ManifestModule.Name.EndsWith("Persistence.dll", StringComparison.OrdinalIgnoreCase)).ToList();
            var mappingTypes = PickMappingTypes(mappingAssemblies);
            foreach (var type in mappingTypes)
            {
                modelBuilder.ApplyConfiguration((dynamic)Activator.CreateInstance(type));
            }
        }
        /// <summary>
        /// Pick Mapping Types
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        private IEnumerable<Type> PickMappingTypes(IEnumerable<Assembly> assemblies)
        {
            return assemblies.SelectMany(x => x.GetTypes()).Where(x => x.IsClass && !x.IsAbstract && typeof(TMappingInterface).IsAssignableFrom(x));
        }
        /// <summary>
        /// Begin Transaction
        /// </summary>
        public void BeginTransaction()
        {
            if (Database.CurrentTransaction == null)
                Database.BeginTransaction();
        }
        /// <summary>
        /// Commit
        /// </summary>
        public void Commit()
        {
            if (Database.CurrentTransaction != null)
                Database.CommitTransaction();
        }
        /// <summary>
        /// Roll back
        /// </summary>
        public void Rollback()
        {
            if (Database.CurrentTransaction != null)
                Database.RollbackTransaction();
        }
        /// <summary>
        /// Save Changes Asyncronce 
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveChangesAsync()
        {
            var result = await base.SaveChangesAsync();
            return result;
        }
        /// <summary>
        /// Dispose 
        /// </summary>
        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IEnumerable<string> GetMigrations()
        {
            return Database.GetMigrations();
        }

        public Task<bool> CanConnectAsync(CancellationToken cancellationToken = default)
        {
            return Database.CanConnectAsync(cancellationToken);
        }

        public Task<IEnumerable<string>> GetPendingMigrationsAsync(CancellationToken cancellationToken = default)
        {
            return Database.GetPendingMigrationsAsync(cancellationToken);
        }

        public Task MigrateAsync(CancellationToken cancellationToken = default)
        {
            return Database.MigrateAsync(cancellationToken);
        }
    }
}
