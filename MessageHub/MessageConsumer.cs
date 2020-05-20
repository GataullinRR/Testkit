using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Utilities.Types;
using Utilities;
using Utilities.Extensions;
using Microsoft.Extensions.Logging;

namespace MessageHub
{
    [Service(ServiceLifetime.Singleton)]
    class MessageConsumer : IMessageConsumer
    {
        readonly ILogger<MessageConsumer> _logger;

        public event Func<TestRecordedMessage, Task> TestRecordedAsync;
        public event Func<TestExecutedMessage, Task> TestExecutedAsync;
        public event Func<TestAcquiredMessage, Task> TestAcquiredAsync;

        public MessageConsumer(ILogger<MessageConsumer> logger, MessageHubOptions options)
        {
            _logger = logger;

            var conf = new ConsumerConfig
            {
                GroupId = "test-consumer-group",
                BootstrapServers = options.ServerURI,
                // Note: The AutoOffsetReset property determines the start offset in the event
                // there are not yet any committed offsets for the consumer group for the
                // topic/partitions of interest. By default, offsets are committed
                // automatically, so in this example, consumption will only start from the
                // earliest message in the topic 'my-topic' the first time you run the program.
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            var testExecuted = new ConsumerBuilder<Ignore, TestExecutedMessage>(conf)
                .SetKeyDeserializer(new JsonNETKafkaSerializer<Ignore>())
                .SetValueDeserializer(new JsonNETKafkaSerializer<TestExecutedMessage>())
                .Build();
            var testRecorded = new ConsumerBuilder<Ignore, TestRecordedMessage>(conf)
                .SetKeyDeserializer(new JsonNETKafkaSerializer<Ignore>())
                .SetValueDeserializer(new JsonNETKafkaSerializer<TestRecordedMessage>())
                .Build();
            var testAcquired = new ConsumerBuilder<Ignore, TestAcquiredMessage>(conf)
                .SetKeyDeserializer(new JsonNETKafkaSerializer<Ignore>())
                .SetValueDeserializer(new JsonNETKafkaSerializer<TestAcquiredMessage>())
                .Build();

            cosumeDaemon(testExecuted, options.TestExecutedTopic, m => TestExecutedAsync.InvokeAndWaitAsync(m));
            cosumeDaemon(testRecorded, options.TestRecordedTopic, m => TestRecordedAsync.InvokeAndWaitAsync(m));
            cosumeDaemon(testAcquired, options.TestAcquiredTopic, m => TestAcquiredAsync.InvokeAndWaitAsync(m));
        }

        async void cosumeDaemon<T>(IConsumer<Ignore, T> consumer, string topic, Func<T, Task> fireEventAsync)
        {
            await ThreadingUtils.ContinueAtDedicatedThread();
            
            consumer.Subscribe(topic);
            try
            {
                while (true)
                {
                    try
                    {
                        var cr = consumer.Consume();
                        _logger.LogTrace($"Consumed message '{cr.Message.Value?.ToString()}' at: '{cr.TopicPartitionOffset}'.");
                        await fireEventAsync(cr.Message.Value);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Error occured");

                        Debugger.Break();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Ensure the consumer leaves the group cleanly and final offsets are committed.
                consumer.Close();
            }
        }
    }
}
