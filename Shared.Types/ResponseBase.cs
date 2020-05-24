using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.Types
{
    public abstract class ResponseBase
    {
        [Required]
        public ResponseStatus Status { get; }

        public ResponseBase(ResponseStatus status)
        {
            Status = status ?? throw new ArgumentNullException(nameof(status));
        }
    }
}
