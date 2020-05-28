using TestsStorageService.API;

namespace MessageHub
{
    public enum AcquiringResult
    {
        Done,
        TargetNotFound,
    }

    public class TestAcquiringResultMessage
    {
        public string AuthorName { get; set; }
        public AcquiringResult ResultCode { get; set; }
        public string TestType { get; set; }
        public byte[] TestData { get; set; }

        /// <summary>
        /// An XML object
        /// </summary>
        public string Parameters { get; set; }
    }
}
