﻿using System;
using System.ComponentModel.DataAnnotations;

namespace TestsStorageService.API
{
    public class TestCase
    {
        [Key]
        public int TestId { get; set; }

        /// <summary>
        /// Null for <see cref="TestCaseState.RecordedButNotSaved"/>
        /// </summary>
        public string? TestName { get; set; }

        [Required]
        public string AuthorName { get; set; }

        public string? DisplayName { get; set; }

        public DateTime CreationDate { get; set; }

        [Required]
        public TestCaseState State { get; set; }

        public TestCaseData? Data { get; set; }
    }
}
