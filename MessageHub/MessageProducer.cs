﻿using Confluent.Kafka;
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

        readonly IProducer<Null, TestRecordedMessage> _testRecordedProducer;
        readonly IProducer<Null, TestExecutedMessage> _testExecutedProducer;
        readonly IProducer<Null, TestAcquiredMessage> _testAcquiredProducer;
        readonly IProducer<Null, TestCompletedMessage> _testCompletedProducer;
        readonly IProducer<Null, TestCompletedOnSourceMessage> _testCompletedOnSourceProducer;

        public MessageProducer(ILogger<MessageProducer> logger, JsonSerializerSettings serializerSettings)
        {
            _logger = logger;
            _options = new MessageHubOptions();

            var conf = new ProducerConfig { BootstrapServers = _options.ServerURI };

            _testRecordedProducer = new ProducerBuilder<Null, TestRecordedMessage>(conf)
                .SetKeySerializer(new JsonNETKafkaSerializer<Null>(serializerSettings))
                .SetValueSerializer(new JsonNETKafkaSerializer<TestRecordedMessage>(serializerSettings))
                .Build();
            _testExecutedProducer = new ProducerBuilder<Null, TestExecutedMessage>(conf)
                .SetKeySerializer(new JsonNETKafkaSerializer<Null>(serializerSettings))
                .SetValueSerializer(new JsonNETKafkaSerializer<TestExecutedMessage>(serializerSettings))
                .Build();
            _testAcquiredProducer = new ProducerBuilder<Null, TestAcquiredMessage>(conf)
                .SetKeySerializer(new JsonNETKafkaSerializer<Null>(serializerSettings))
                .SetValueSerializer(new JsonNETKafkaSerializer<TestAcquiredMessage>(serializerSettings))
                .Build();
            _testCompletedProducer = new ProducerBuilder<Null, TestCompletedMessage>(conf)
                .SetKeySerializer(new JsonNETKafkaSerializer<Null>(serializerSettings))
                .SetValueSerializer(new JsonNETKafkaSerializer<TestCompletedMessage>(serializerSettings))
                .Build();
            _testCompletedOnSourceProducer = new ProducerBuilder<Null, TestCompletedOnSourceMessage>(conf)
                .SetKeySerializer(new JsonNETKafkaSerializer<Null>(serializerSettings))
                .SetValueSerializer(new JsonNETKafkaSerializer<TestCompletedOnSourceMessage>(serializerSettings))
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

        public void FireTestCompleted(TestCompletedMessage args)
        {
            _testCompletedProducer.Produce(_options.TestCompletedTopic, new Message<Null, TestCompletedMessage> { Value = args });
        }

        public void FireTestCompletedOnSource(TestCompletedOnSourceMessage args)
        {
            _testCompletedOnSourceProducer.Produce(_options.TestCompletedOnSourceTopic, new Message<Null, TestCompletedOnSourceMessage> { Value = args });
        }
    }
}