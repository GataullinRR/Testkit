using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using System;
using Utilities.Types;

namespace MessageHub
{
    [Service(ServiceLifetime.Singleton)]
    class MessageProducer : IMessageProducer
    {
        readonly MessageHubOptions _options;
        readonly IProducer<Null, TestRecordedMessage> _testRecordedProducer;
        readonly IProducer<Null, TestExecutedMessage> _testExecutedProducer;
        readonly IProducer<Null, TestAcquiredMessage> _testAcquiredProducer;

        public MessageProducer()
        {
            _options = new MessageHubOptions();

            var conf = new ProducerConfig { BootstrapServers = _options.ServerURI };

            Action<DeliveryReport<Null, string>> handler = r =>
                Console.WriteLine(!r.Error.IsError
                    ? $"Delivered message to {r.TopicPartitionOffset}"
                    : $"Delivery Error: {r.Error.Reason}");

            _testRecordedProducer = new ProducerBuilder<Null, TestRecordedMessage>(conf).Build();
            _testExecutedProducer = new ProducerBuilder<Null, TestExecutedMessage>(conf).Build();
        }

        public void FireTestExecuted(TestExecutedMessage args)
        {
            _testExecutedProducer.Produce(_options.TestExecutedTopic, new Message<Null, TestExecutedMessage> { Value = args });
        }

        public void FireTestRecorded(TestRecordedMessage args)
        {
            _testRecordedProducer.Produce(_options.TestRecordedTopic, new Message<Null, TestRecordedMessage> { Value = args });
        }

        public void FireTestAcquired(TestAcquiredMessage args)
        {
            _testAcquiredProducer.Produce(_options.TestAcquiredTopic, new Message<Null, TestAcquiredMessage> { Value = args });
        }
    }
}
