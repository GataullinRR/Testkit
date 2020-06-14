using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;

namespace Utilities.Types
{
    public class ExceptionBuilder
    {
        string _message;

        public void SetException(string message)
        {
            _message = message;
        }

        public Exception InstantiateException()
        {
            return new Exception(_message);
        }
        public Exception InstantiateException(Exception innerException)
        {
            return new Exception(_message, innerException);
        }
    }
}
