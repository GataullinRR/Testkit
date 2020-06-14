using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities.Types
{
    public enum ArgumentError
    {
        /// <summary>
        /// Взаимные значения аргументов некорректны.
        /// ПР: Ожидалось, что оба аргумента будут отрицательными.
        /// </summary>
        INCORRECT_CONSTARAINTS,

        /// <summary>
        /// Числовой аргумент имел недопустимое значение.
        /// </summary>
        NUMBER_OUT_OF_RANGE,
        /// <summary>
        /// Аргумент имел недопустимое значение.
        /// </summary>
        OUT_OF_RANGE,
        /// <summary>
        /// </summary>
        NULL_ARGUMENT,
        /// <summary>
        /// Ожидалась структура, имеющая значение отличное от значения по умолчанию.
        /// </summary>
        DEFAULT_STRUCT,
        /// <summary>
        /// Аргумент типа имеет недопустимый тип.
        /// </summary>
        PROHIBITED_TYPE_ARGUMENT,
        /// <summary>
        /// Не возможно выполнить кастинг аргумента к более типу интерфейсу/классу.
        /// ПР: В метод your_type.Equals(object) передали объект не являющийся типом/подтипом your_type
        /// </summary>
        DOWNCAST_NOT_POSSIBLE,

        /// <summary>
        /// Не NULL значение аргумента не поддерживается текущей реализацией. Только для аргументов поддерживающих NULL значения.
        /// </summary>
        NOT_SUPPORTED_BY_CURRENT_IMPL
    }

    /// <summary>
    /// Для случаев, когда по какой-то причине метод, индекасатор или конструктор не могут корректно выполниться с данным набором аргументов.
    /// </summary>
    public class ArgumentUniformException : UniformException<ArgumentError>
    {
        public ArgumentUniformException(ArgumentError errorCode) 
            : base(errorCode) { }

        public ArgumentUniformException(ArgumentError errorCode, Exception innerException) 
            : base(errorCode, innerException) { }

        public ArgumentUniformException(ArgumentError errorCode, string additionalMessage) 
            : base(errorCode, additionalMessage) { }

        public ArgumentUniformException(ArgumentError errorCode, string additionalMessage, Exception innerException) 
            : base(errorCode, additionalMessage, innerException) { }
    }
}
