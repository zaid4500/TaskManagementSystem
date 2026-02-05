using NJsonSchema.Generation;
using NJsonSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Host.Base.Configurations.Swagger
{
    public class SwaggerGuidSchemaProcessor : ISchemaProcessor
    {
        public void Process(SchemaProcessorContext context)
        {
            var type = context.ContextualType;
            var schema = context.Schema;

            // Check if the type is a Guid
            if (type == typeof(Guid))
            {
                schema.Type = JsonObjectType.String;
                schema.Format = "uuid";
            }
        }
    }
}
