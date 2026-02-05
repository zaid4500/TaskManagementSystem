using ManagementBE.Kernel.Core.Infrastructure;
using ManagementBE.Kernel.Core.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Initialization
{
    public class ApplicationDbInitializer
    {
        readonly IUnitOfWork _unitOfWork;
        readonly ApplicationDbSeeder _dbSeeder;
        readonly IApplicationLoggerService _logger;

        public ApplicationDbInitializer(IUnitOfWork unitOfWork,
            IApplicationLoggerService logger,
            ApplicationDbSeeder dbSeeder)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _dbSeeder = dbSeeder;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            try
            {
                bool isInMemory = _unitOfWork.Database.IsInMemory();

                if (isInMemory)
                {
                    await _unitOfWork.Database.EnsureCreatedAsync(cancellationToken);
                    await _dbSeeder.SeedDatabaseAsync(cancellationToken);
                }
                else
                {
                    if (_unitOfWork.GetMigrations().Any())
                    {
                        if ((await _unitOfWork.GetPendingMigrationsAsync(cancellationToken)).Any())
                        {
                            await _unitOfWork.MigrateAsync(cancellationToken);
                        }

                        if (await _unitOfWork.CanConnectAsync(cancellationToken))
                        {
                            await _dbSeeder.SeedDatabaseAsync(cancellationToken);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, "ApplicationDbInitializer-InitializeAsync", null);
            }
        }
    }
}
