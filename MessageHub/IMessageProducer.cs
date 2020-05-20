namespace MessageHub
{
    public interface IMessageProducer
    {
        void FireTestRecorded(TestRecordedMessage args);
        void FireTestExecuted(TestExecutedMessage args);
        void FireTestAcquired(TestAcquiredMessage args);
    }
}
