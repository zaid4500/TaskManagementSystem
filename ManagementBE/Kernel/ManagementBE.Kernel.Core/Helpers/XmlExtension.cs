using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ManagementBE.Kernel.Core.Helpers
{
    public static class XmlExtension
    {
        public static string SerializeObjectToXmlString<T>(T obj)
        {

            var xmlSerializer = new XmlSerializer(obj.GetType(), new XmlAttributeOverrides());
            var xmlSerializerNamespaces = new XmlSerializerNamespaces();
            xmlSerializerNamespaces.Add("", ""); // Add an empty namespace to remove xmlns attributes

            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, obj, xmlSerializerNamespaces);
                var xmlResult = stringWriter.ToString();
                return xmlResult;
            }
        }

        public static T DeserializeXml<T>(string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (StringReader reader = new StringReader(xmlString))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}
