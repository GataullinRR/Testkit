using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using Utilities.Types;

namespace MessageHub
{
    [Service(ServiceLifetime.Singleton)]
    class MessageProducer : IMessageProducer
    {
        readonly MessageHubOptions _options;
        readonly ILogger<MessageProducer> _logger;

        readonly IProducer<Null, TestAddedMessage> _testAddedProducer;
        readonly IProducer<Null, TestExecutedMessage> _testExecutedProducer;
        readonly IProducer<Null, TestAcquiringResultMessage> _testAcquiredProducer;
        readonly IProducer<Null, TestCompletedMessage> _testCompletedProducer;
        readonly IProducer<Null, TestCompletedOnSourceMessage> _testCompletedOnSourceProducer;
        readonly IProducer<Null, TestDeletedMessage> _testDeletedProducer;
        readonly IProducer<Null, BeginTestMessage> _beginTestProducer;
        readonly IProducer<Null, BeginAddTestMessage> _beginAddTestProducer;
        readonly IProducer<Null, StopAddTestMessage> _stopAddTestProducer;
        readonly IProducer<Null, TestRecordedMessage> _testRecordedProducer;

        public MessageProducer(ILogger<MessageProducer> logger, JsonSerializerSettings serializerSettings)
        {
            _logger = logger;
            _options = new MessageHubOptions();

            var conf = new ProducerConfig { BootstrapServers = _options.ServerURI };

            _testAddedProducer = new ProducerBuilder<Null, TestAddedMessage>(conf).Build(serializerSettings);
            _testExecutedProducer = new ProducerBuilder<Null, TestExecutedMessage>(conf).Build(serializerSettings);
            _testAcquiredProducer = new ProducerBuilder<Null, TestAcquiringResultMessage>(conf).Build(serializerSettings);
            _testCompletedProducer = new ProducerBuilder<Null, TestCompletedMessage>(conf).Build(serializerSettings);
            _testCompletedOnSourceProducer = new ProducerBuilder<Null, TestCompletedOnSourceMessage>(conf).Build(serializerSettings);
            _testDeletedProducer = new ProducerBuilder<Null, TestDeletedMessage>(conf).Build(serializerSettings);
            _beginTestProducer = new ProducerBuilder<Null, BeginTestMessage>(conf).Build(serializerSettings);
            _beginAddTestProducer = new ProducerBuilder<Null, BeginAddTestMessage>(conf).Build(serializerSettings);
            _stopAddTestProducer = new ProducerBuilder<Null, StopAddTestMessage>(conf).Build(serializerSettings);
            _testRecordedProducer = new ProducerBuilder<Null, TestRecordedMessage>(conf).Build(serializerSettings);
        }

        public void FireTestExecuted(TestExecutedMessage args)
        {
            _logger.LogTrace("Sending: {0}", args);

            _testExecutedProducer.Produce(_options.TestExecutedTopic, new Message<Null, TestExecutedMessage> { Value = args });
        }

        public void FireTestAdded(TestAddedMessage args)
        {
            _logger.LogTrace("Sending: {0}", args);

            _testAddedProducer.Produce(_options.TestAddedTopic, new Message<Null, TestAddedMessage> { Value = args });
        }

        public void FireTestAcquired(TestAcquiringResultMessage args)
        {
            _logger.LogTrace("Sending: {0}", args);

            Action<DeliveryReport<Null, TestAcquiringResultMessage>> handler = r =>
            Console.WriteLine(!r.Error.IsError
                ? $"Delivered message to {r.TopicPartitionOffset}"
                : $"Delivery Error: {r.Error.Reason}");

            _testAcquiredProducer.Produce(_options.TestAcquiredTopic, new Message<Null, TestAcquiringResultMessage> { Value = args }, handler);
        }

        public void FireTestCompleted(TestCompletedMessage args)
        {
            _testCompletedProducer.Produce(_options.TestCompletedTopic, new Message<Null, TestCompletedMessage> { Value = args });
        }

        public void FireTestCompletedOnSource(TestCompletedOnSourceMessage args)
        {
            _testCompletedOnSourceProducer.Produce(_options.TestCompletedOnSourceTopic, new Message<Null, TestCompletedOnSourceMessage> { Value = args });
        }

        public void FireTestDeleted(TestDeletedMessage args)
        {
            _testDeletedProducer.Produce(_options.TestDeletedTopic, new Message<Null, TestDeletedMessage> { Value = args });
        }

        public void FireBeginTest(BeginTestMessage args)
        {
            _beginTestProducer.Produce(_options.BeginTestTopic, new Message<Null, BeginTestMessage> { Value = args });
        }

        public void FireBeginAddTest(BeginAddTestMessage args)
        {
            _beginAddTestProducer.Produce(_options.BeginAddTestTopic, new Message<Null, BeginAddTestMessage> { Value = args });
        }

        public void FireStopAddTest(StopAddTestMessage args)
        {
            _stopAddTestProducer.Produce(_options.StopAddTestTopic, new Message<Null, StopAddTestMessage> { Value = args });
        }

        public void FireTestRecorded(TestRecordedMessage args)
        {
            _testRecordedProducer.Produce(_options.TestRecordedTopic, new Message<Null, TestRecordedMessage> { Value = args });
        }
    }
}
