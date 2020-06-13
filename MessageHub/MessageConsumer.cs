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
using System.Linq;
using Namotion.Reflection;

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
        public event Func<TestRecordedMessage, Task> TestRecordedAsync = m => Task.CompletedTask;
        public event Func<CancelTestMessage, Task> CancelTestAsync = m => Task.CompletedTask;
        public event Func<TestCancelledMessage, Task> TestCancelledAsync = m => Task.CompletedTask;

        public event Func<UpdateTestResultStateMessage, Task> UpdateTestResultAsync = m => Task.CompletedTask;
        public event Func<TestResultStateAcquiredMessage, Task> TestResultStateAcquiredAsync = m => Task.CompletedTask;
        public event Func<TestResultStateUpdatedMessage, Task> TestResultStateUpdatedAsync = m => Task.CompletedTask;

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
            var testAdded = new ConsumerBuilder<Ignore, TestAddedMessage>(conf)
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
            var testRecorded = new ConsumerBuilder<Ignore, TestRecordedMessage>(conf)
                .Build(serializerSettings);

            consumeDaemon(testExecuted, options.TestExecutedTopic, m => TestExecutedAsync.InvokeAndWaitAsync(m));
            consumeDaemon(testAdded, options.TestAddedTopic, m => TestAddedAsync.InvokeAndWaitAsync(m));
            consumeDaemon(testAcquired, options.TestAcquiredTopic, m => TestAcquiredAsync.InvokeAndWaitAsync(m));
            consumeDaemon(testCompleted, options.TestCompletedTopic, m => TestCompletedAsync.InvokeAndWaitAsync(m));
            consumeDaemon(testCompletedOnSource, options.TestCompletedOnSourceTopic, m => TestCompletedOnSourceAsync.InvokeAndWaitAsync(m));
            consumeDaemon(testDeleted, options.TestDeletedTopic, m => TestDeletedAsync.InvokeAndWaitAsync(m));
            consumeDaemon(beginTest, options.BeginTestTopic, m => BeginTestAsync.InvokeAndWaitAsync(m));
            consumeDaemon(testRecorded, options.TestRecordedTopic, m => TestRecordedAsync.InvokeAndWaitAsync(m));

            startConsumeDaemon<CancelTestMessage>(conf, serializerSettings, options.CancelTestTopic, m => CancelTestAsync.InvokeAndWaitAsync(m));
            startConsumeDaemon<TestCancelledMessage>(conf, serializerSettings, options.TestCancelledTopic, m => TestCancelledAsync.InvokeAndWaitAsync(m));
            
            startConsumeDaemon<UpdateTestResultStateMessage>(conf, serializerSettings, null, m => UpdateTestResultAsync.InvokeAndWaitAsync(m));
            startConsumeDaemon<TestResultStateAcquiredMessage>(conf, serializerSettings, null, m => TestResultStateAcquiredAsync.InvokeAndWaitAsync(m));
            startConsumeDaemon<TestResultStateUpdatedMessage>(conf, serializerSettings, null, m => TestResultStateUpdatedAsync.InvokeAndWaitAsync(m));
        }

        async void startConsumeDaemon<TMessage>(ConsumerConfig config, JsonSerializerSettings serializerSettings, string? topic, Func<TMessage, Task> fireEventAsync) 
            where TMessage : class
        {
            topic = topic ?? typeof(TMessage).Name;//.Split(".").TakeFromEnd(1).Single().Where(char.IsLetterOrDigit).Aggregate();
            var testExecuted = new ConsumerBuilder<Ignore, TMessage>(config).Build(serializerSettings);
            consumeDaemon(testExecuted, topic, fireEventAsync);
        }

        async void consumeDaemon<T>(IConsumer<Ignore, T> consumer, string topic, Func<T, Task> fireEventAsync)
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
                }
            }
        }
    }
}
