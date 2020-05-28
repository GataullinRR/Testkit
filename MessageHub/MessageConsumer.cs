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
using System.Threading;

namespace MessageHub
{
    [Service(ServiceLifetime.Singleton)]
    class MessageConsumer : IMessageConsumer
    {
        readonly ILogger<MessageConsumer> _logger;

        public event Func<TestAddedMessage, Task> TestAddedAsync = m => Task.CompletedTask;
        public event Func<TestExecutedMessage, Task> TestExecutedAsync = m => Task.CompletedTask;
        public event Func<TestAcquiringResultMessage, Task> TestAcquiredAsync = m => Task.CompletedTask;
        public event Func<TestCompletedMessage, Task> TestCompletedAsync = m => Task.CompletedTask;
        public event Func<TestCompletedOnSourceMessage, Task> TestCompletedOnSourceAsync = m => Task.CompletedTask;
        public event Func<TestDeletedMessage, Task> TestDeletedAsync = m => Task.CompletedTask;
        public event Func<BeginTestMessage, Task> BeginTestAsync = m => Task.CompletedTask;
        public event Func<BeginAddTestMessage, Task> BeginAddTestAsync = m => Task.CompletedTask;
        public event Func<StopAddTestMessage, Task> StopAddTestAsync = m => Task.CompletedTask;

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
                .Build(serializerSettings);
            var testRecorded = new ConsumerBuilder<Ignore, TestAddedMessage>(conf)
                .Build(serializerSettings);
            var testAcquired = new ConsumerBuilder<Ignore, TestAcquiringResultMessage>(conf)
                .Build(serializerSettings);
            var testCompleted = new ConsumerBuilder<Ignore, TestCompletedMessage>(conf)
                .Build(serializerSettings);
            var testCompletedOnSource = new ConsumerBuilder<Ignore, TestCompletedOnSourceMessage>(conf)
                .Build(serializerSettings);
            var testDeleted = new ConsumerBuilder<Ignore, TestDeletedMessage>(conf)
                .Build(serializerSettings);
            var beginTest = new ConsumerBuilder<Ignore, BeginTestMessage>(conf)
                .Build(serializerSettings);
            var beginAddTest = new ConsumerBuilder<Ignore, BeginAddTestMessage>(conf)
                .Build(serializerSettings);
            var stopAddTest = new ConsumerBuilder<Ignore, StopAddTestMessage>(conf)
                .Build(serializerSettings);

            cosumeDaemon(testExecuted, options.TestExecutedTopic, m => TestExecutedAsync.InvokeAndWaitAsync(m));
            cosumeDaemon(testRecorded, options.TestRecordedTopic, m => TestAddedAsync.InvokeAndWaitAsync(m));
            cosumeDaemon(testAcquired, options.TestAcquiredTopic, m => TestAcquiredAsync.InvokeAndWaitAsync(m));
            cosumeDaemon(testCompleted, options.TestCompletedTopic, m => TestCompletedAsync.InvokeAndWaitAsync(m));
            cosumeDaemon(testCompletedOnSource, options.TestCompletedOnSourceTopic, m => TestCompletedOnSourceAsync.InvokeAndWaitAsync(m));
            cosumeDaemon(testDeleted, options.TestDeletedTopic, m => TestDeletedAsync.InvokeAndWaitAsync(m));
            cosumeDaemon(beginTest, options.BeginTestTopic, m => BeginTestAsync.InvokeAndWaitAsync(m));
            cosumeDaemon(beginAddTest, options.BeginAddTestTopic, m => BeginAddTestAsync.InvokeAndWaitAsync(m));
            cosumeDaemon(stopAddTest, options.StopAddTestTopic, m => StopAddTestAsync.InvokeAndWaitAsync(m));
        }

        async void cosumeDaemon<T>(IConsumer<Ignore, T> consumer, string topic, Func<T, Task> fireEventAsync)
        {
            await Task.Delay(3000); // To remove deadlock
            await ThreadingUtils.ContinueAtDedicatedThread();

            consumer.Subscribe(topic);
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
    }
}
