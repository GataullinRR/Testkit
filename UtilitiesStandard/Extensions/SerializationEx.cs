using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Utilities;
using Utilities.Extensions;

namespace Utilities.Extensions
{
    public static class SerializationEx
    {
        public static object Deserialize(this XmlSerializer serializer, string data)
        {
            var stream = new MemoryStream(data.GetBytes(Encoding.Unicode));
            return serializer.Deserialize(stream);
        }

        public static object Deserialize(this IFormatter serializer, byte[] data)
        {
            var stream = new MemoryStream(data);
            return serializer.Deserialize(stream);
        }

        public static byte[] Serialize(this IFormatter serializer, object graph)
        {
            var stream = new MemoryStream();
            serializer.Serialize(stream, graph);

            return stream.ToArray();
        }
    }
}
