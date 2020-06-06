using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TestsStorageService.API;

namespace MessageHub
{
    /// <summary>
    /// When test acquired on tests source side
    /// </summary>
    public class TestAcquiringResultMessage
    {
        public string? DefaultTestName { get; }

        public string? DefaultTestDescription { get; }

        public string? TestType { get; }

        public byte[]? TestData { get; }

        [Required]
        public Dictionary<string, string> KeyParameters { get; }

        /// <summary>
        /// An XML object
        /// </summary>
        public string? Parameters { get; }

        public TestAcquiringResultMessage(string? defaultTestName, string? defaultTestDescription, string? testType, byte[]? testData, Dictionary<string, string> keyParameters, string? parameters)
        {
            DefaultTestName = defaultTestName;
            DefaultTestDescription = defaultTestDescription;
            TestType = testType;
            TestData = testData;
            KeyParameters = keyParameters ?? throw new ArgumentNullException(nameof(keyParameters));
            Parameters = parameters;
        }
    }
}
