namespace MessageHub
{
    public interface IMessageProducer
    {
        void FireTestAdded(TestAddedMessage args);
        void FireTestExecuted(TestExecutedMessage args);
        void FireTestAcquired(TestAcquiringResultMessage args);
        void FireTestCompleted(TestCompletedMessage args);
        void FireTestCompletedOnSource(TestCompletedOnSourceMessage args);
        void FireTestDeleted(TestDeletedMessage args);
        void FireTestRecorded(TestRecordedMessage args);

        void FireBeginTest(BeginTestMessage args);
        void FireBeginAddTest(BeginAddTestMessage args);
        void FireStopAddTest(StopAddTestMessage args);
        void FireTestAddProgressChanged(TestAddProgressChangedMessage args);
    }
}
