using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Utilities.Types;
using Utilities;
using Utilities.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.Extensions.Options;

namespace MessageHub
{
    [Service(ServiceLifetime.Singleton)]
    class MessageConsumer : IMessageConsumer
    {
        readonly ILogger<MessageConsumer> _logger;

        public event Func<TestRecordedMessage, Task> TestRecordedAsync = m => Task.CompletedTask;
        public event Func<TestExecutedMessage, Task> TestExecutedAsync = m => Task.CompletedTask;
        public event Func<TestAcquiredMessage, Task> TestAcquiredAsync = m => Task.CompletedTask;
        public event Func<TestCompletedMessage, Task> TestCompletedAsync = m => Task.CompletedTask;
        public event Func<TestCompletedOnSourceMessage, Task> TestCompletedOnSourceAsync = m => Task.CompletedTask;

        public MessageConsumer(ILogger<MessageConsumer> logger, JsonSerializerSettings serializerSettings, MessageHubOptions options, 
            IOptions<MessageConsumerOptions> consumerOptions)
        {
            _logger = logger;

            var conf = new ConsumerConfig
            {
                GroupId = consumerOptions.Value.GroupId, //"test-consumer-group",
                BootstrapServers = options.ServerURI,
                // Note: The AutoOffsetReset property determines the start offset in the event
                // there are not yet any committed offsets for the consumer group for the
                // topic/partitions of interest. By default, offsets are committed
                // automatically, so in this example, consumption will only start from the
                // earliest message in the topic 'my-topic' the first time you run the program.
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            var testExecuted = new ConsumerBuilder<Ignore, TestExecutedMessage>(conf)
                .SetKeyDeserializer(new JsonNETKafkaSerializer<Ignore>(serializerSettings))
                .SetValueDeserializer(new JsonNETKafkaSerializer<TestExecutedMessage>(serializerSettings))
                .Build();
            var testRecorded = new ConsumerBuilder<Ignore, TestRecordedMessage>(conf)
                .SetKeyDeserializer(new JsonNETKafkaSerializer<Ignore>(serializerSettings))
                .SetValueDeserializer(new JsonNETKafkaSerializer<TestRecordedMessage>(serializerSettings))
                .Build();
            var testAcquired = new ConsumerBuilder<Ignore, TestAcquiredMessage>(conf)
                .SetKeyDeserializer(new JsonNETKafkaSerializer<Ignore>(serializerSettings))
                .SetValueDeserializer(new JsonNETKafkaSerializer<TestAcquiredMessage>(serializerSettings))
                .Build();
            var testCompleted = new ConsumerBuilder<Ignore, TestCompletedMessage>(conf)
                .SetKeyDeserializer(new JsonNETKafkaSerializer<Ignore>(serializerSettings))
                .SetValueDeserializer(new JsonNETKafkaSerializer<TestCompletedMessage>(serializerSettings))
                .Build();
            var testCompletedOnSource = new ConsumerBuilder<Ignore, TestCompletedOnSourceMessage>(conf)
                .SetKeyDeserializer(new JsonNETKafkaSerializer<Ignore>(serializerSettings))
                .SetValueDeserializer(new JsonNETKafkaSerializer<TestCompletedOnSourceMessage>(serializerSettings))
                .Build();

            cosumeDaemon(testExecuted, options.TestExecutedTopic, m => TestExecutedAsync.InvokeAndWaitAsync(m));
            cosumeDaemon(testRecorded, options.TestRecordedTopic, m => TestRecordedAsync.InvokeAndWaitAsync(m));
            cosumeDaemon(testAcquired, options.TestAcquiredTopic, m => TestAcquiredAsync.InvokeAndWaitAsync(m));
            cosumeDaemon(testCompleted, options.TestCompletedTopic, m => TestCompletedAsync.InvokeAndWaitAsync(m));
            cosumeDaemon(testCompletedOnSource, options.TestCompletedOnSourceTopic, m => TestCompletedOnSourceAsync.InvokeAndWaitAsync(m));
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
