using ManagementBE.Kernel.Core.Infrastructure;
using ManagementBE.Kernel.Core.Persistence;
using ManagementBE.Kernel.Core.Services;
using ManagementBE.Kernel.Domain.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Services
{
    public class HttpLoggerService : IHttpLoggerService, IApplicationService 
    {
        readonly IRepository<HttpLog> _repository;
        public HttpLoggerService(IRepository<HttpLog> repository) 
        {
            _repository = repository;
        }

        public async Task Log(HttpLog httpLog)
        {
            await _repository.InsertAsync(httpLog);
            await _repository.UnitOfWork.SaveChangesAsync();
        }
    }
}
