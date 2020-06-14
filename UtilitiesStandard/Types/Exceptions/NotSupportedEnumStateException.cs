using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities.Types
{
    public class NotSupportedEnumStateException : Exception
    {
        public NotSupportedEnumStateException(Type enumType, object state)
            : this(enumType, state, null, null)
        {

        }
        public NotSupportedEnumStateException(Type enumType, object state, string additionalMessage, Exception innerException)
            : base(getMessage(enumType, state, additionalMessage), innerException)
        {

        }

        static string getMessage(Type enumType, object state, string additionalMessage)
        {
            return "Not supported enum state detected. EnumType: {0}; State: {1}; AdditionalMessage {2};"
                .Format(enumType, state, additionalMessage);
        }
    }
}
