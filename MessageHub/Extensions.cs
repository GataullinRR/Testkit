using Confluent.Kafka;
using Newtonsoft.Json;

namespace MessageHub
{
    static class Extensions
    {
        public static IConsumer<T1, T2> Build<T1, T2>(this ConsumerBuilder<T1, T2> builder, JsonSerializerSettings serializerSettings)
            where T1 : class
            where T2 : class
        {
            return builder
                .SetKeyDeserializer(new JsonNETKafkaSerializer<T1>(serializerSettings))
                .SetValueDeserializer(new JsonNETKafkaSerializer<T2>(serializerSettings))
                .Build();
        }

        public static IProducer<T1, T2> Build<T1, T2>(this ProducerBuilder<T1, T2> builder, JsonSerializerSettings serializerSettings)
            where T1 : class
            where T2 : class
        {
            return builder
                .SetKeySerializer(new JsonNETKafkaSerializer<T1>(serializerSettings))
                .SetValueSerializer(new JsonNETKafkaSerializer<T2>(serializerSettings))
                .Build();
        }
    }
}
