using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Utilities.Types;

namespace MessageHub
{
    [Service(ServiceLifetime.Singleton)]
    class MessageProducer : IMessageProducer
    {
        readonly ILogger<MessageProducer> _logger;

        readonly MessageHubOptions _options;
        readonly IProducer<Null, TestRecordedMessage> _testRecordedProducer;
        readonly IProducer<Null, TestExecutedMessage> _testExecutedProducer;
        readonly IProducer<Null, TestAcquiredMessage> _testAcquiredProducer;

        public MessageProducer(ILogger<MessageProducer> logger)
        {
            _logger = logger;
            _options = new MessageHubOptions();

            var conf = new ProducerConfig { BootstrapServers = _options.ServerURI };

            _testRecordedProducer = new ProducerBuilder<Null, TestRecordedMessage>(conf)
                .SetKeySerializer(new JsonNETKafkaSerializer<Null>())
                .SetValueSerializer(new JsonNETKafkaSerializer<TestRecordedMessage>())
                .Build();
            _testExecutedProducer = new ProducerBuilder<Null, TestExecutedMessage>(conf)
                .SetKeySerializer(new JsonNETKafkaSerializer<Null>())
                .SetValueSerializer(new JsonNETKafkaSerializer<TestExecutedMessage>())
                .Build();
            _testAcquiredProducer = new ProducerBuilder<Null, TestAcquiredMessage>(conf)
                .SetKeySerializer(new JsonNETKafkaSerializer<Null>())
                .SetValueSerializer(new JsonNETKafkaSerializer<TestAcquiredMessage>())
                .Build();
        }

        public void FireTestExecuted(TestExecutedMessage args)
        {
            _logger.LogTrace("Sending: {0}", args);


            _testExecutedProducer.Produce(_options.TestExecutedTopic, new Message<Null, TestExecutedMessage> { Value = args });
        }

        public void FireTestRecorded(TestRecordedMessage args)
        {
            _logger.LogTrace("Sending: {0}", args);

            _testRecordedProducer.Produce(_options.TestRecordedTopic, new Message<Null, TestRecordedMessage> { Value = args });
        }

        public void FireTestAcquired(TestAcquiredMessage args)
        {
            _logger.LogTrace("Sending: {0}", args);

            Action<DeliveryReport<Null, TestAcquiredMessage>> handler = r =>
            Console.WriteLine(!r.Error.IsError
                ? $"Delivered message to {r.TopicPartitionOffset}"
                : $"Delivery Error: {r.Error.Reason}");

            _testAcquiredProducer.Produce(_options.TestAcquiredTopic, new Message<Null, TestAcquiredMessage> { Value = args }, handler);
        }
    }
}
