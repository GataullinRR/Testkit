using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities.Types
{
    public abstract class UniformException<TError> : Exception
    {
        readonly public TError Error;

        public override string Message
        {
            get => "Message: {0} ErrorCode: {1}".Format(base.Message as object, Error);
        }

        public UniformException(TError errorCode, string additionalMessage, Exception innerException)
            : base(additionalMessage, innerException)
        {
            Error = errorCode;
        }

        public UniformException(TError errorCode, Exception innerException)
            : base("", innerException)
        {
            Error = errorCode;
        }

        public UniformException(TError errorCode, string additionalMessage)
            : base(additionalMessage)
        {
            Error = errorCode;
        }

        public UniformException(TError errorCode)
            : base()
        {
            Error = errorCode;
        }

        public void RethrowIfNot(params TError[] errors)
        {
            if (!errors.Contains(Error))
            {
                throw this;
            }
        }
    }
}
