namespace MessageHub
{
    public class CancelTestMessage
    {
        public int TestId { get; }

        public CancelTestMessage(int testId)
        {
            TestId = testId;
        }
    }
}
