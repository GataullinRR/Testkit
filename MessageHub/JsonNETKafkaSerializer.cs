using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace MessageHub
{
    class JsonNETKafkaSerializer<T> : ISerializer<T>, IDeserializer<T>
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data.ToArray()));
        }


        public byte[] Serialize(T data, SerializationContext context)
        {
            return JsonConvert.SerializeObject(data).GetUTF8Bytes();
        }

        //public async Task<byte[]> SerializeAsync(T data, SerializationContext context)
        //{
        //    return JsonConvert.SerializeObject(data).GetUTF8Bytes();
        //}
        //public async Task<T> DeserializeAsync(ReadOnlyMemory<byte> data, bool isNull, SerializationContext context)
        //{
        //    return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data.ToArray()));
        //}

    }
}
