using ManagementBE.Kernel.Core.Infrastructure;
using ManagementBE.Kernel.Core.Services;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Services
{
    public class NewtonSoftService : ISerializerService, ITransientDependency
    {
        public T Deserialize<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }

        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter>
            {
                new StringEnumConverter() { CamelCaseText = true }
            }
            });
        }

        public string Serialize<T>(T obj, Type type)
        {
            return JsonConvert.SerializeObject(obj, type, new());
        }
    }
}
