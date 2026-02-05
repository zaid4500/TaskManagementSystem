using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Core.Infrastructure
{
    public interface ISerializerService
    {
        string Serialize<T>(T obj);

        string Serialize<T>(T obj, Type type);

        T Deserialize<T>(string text);
    }
}
