using System.Text;
using System.Xml.Serialization;

namespace Footballers.Extensions
{
    public static class XmlSerializationExtension
    {
        public static string SerializeXml<T>(this T data, string rootElement) where T : class
        {
            string result = null!;

            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootElement));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();
            using (StringWriter sr = new StringWriter(sb))
            {
                serializer.Serialize(sr, data, ns);

                result = sb.ToString().TrimEnd();
            }

            return result;
        }

        public static T? DeserializeXml<T>(this string inputXml, string rootElement) where T : class
        {
            XmlRootAttribute xmlRoot = new(rootElement);

            XmlSerializer serializer = new XmlSerializer(typeof(T), xmlRoot);

            using StringReader reader = new StringReader(inputXml);

            T? deserializedObject = (T?)serializer.Deserialize(reader);

            return deserializedObject;
        }
    }
}
