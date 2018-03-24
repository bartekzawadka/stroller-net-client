using System.IO;
using System.Xml.Serialization;

namespace Stroller.Bll
{
    public static class XmlFileSerializer
    {
        public static T Deserialize<T>(string filePath)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StreamReader(filePath))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        public static void Serialize<T>(T item, string filePath)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new StreamWriter(filePath, false))
            {
                serializer.Serialize(writer, item);
            }
        }
    }
}
