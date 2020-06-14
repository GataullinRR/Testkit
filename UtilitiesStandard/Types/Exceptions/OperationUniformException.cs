using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities.Types
{
    public enum OperationError
    {
        INSTANCE_NOT_INITIALIZED,
        INSTANCE_DESTROYED,
        INVALID_EXECUTION_ORDER,
        PROHIBITED_OPERATION,
        PREVIOUS_OPERATION_MUST_BE_COMPLETED,
        OPERATION_ALREADY_STARTED
    }

    /// <summary>
    /// Для случаев, когда по какой-то причине метод, свойство, индекасатор или конструктор не могут корректно выполниться при текущем состоянии экземпляра или переменных типа.
    /// </summary>
    public class OperationUniformException : UniformException<OperationError>
    {
        public OperationUniformException(OperationError errorCode)
            : base(errorCode) { }

        public OperationUniformException(OperationError errorCode, Exception innerException)
            : base(errorCode, innerException) { }

        public OperationUniformException(OperationError errorCode, string additionalMessage)
            : base(errorCode, additionalMessage) { }

        public OperationUniformException(OperationError errorCode, string additionalMessage, Exception innerException)
            : base(errorCode, additionalMessage, innerException) { }
    }
}
