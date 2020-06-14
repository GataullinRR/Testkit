using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities.Extensions
{
    public static class ThrowEx
    {
        public static int ThrowIf_Zero(this int value, ArgumentError argumentError, string argumenName)
        {
            ((double)value).ThrowIf_Zero(argumentError, argumenName);

            return value;
        }
        public static double ThrowIf_Zero(this double value, ArgumentError argumentError, string argumenName)
        {
            if (value == 0)
            {
                throw new ArgumentUniformException(argumentError, $"ArgumentName = {argumenName}");
            }

            return value;
        }
        public static byte ThrowIf_Zero(this byte value, ArgumentError argumentError, string argumenName)
        {
            ((double)value).ThrowIf_Zero(argumentError, argumenName);

            return value;
        }

        public static int ThrowIf_Zero(this int value, OperationError operationError, string argumenName)
        {
            ((double)value).ThrowIf_Zero(operationError);

            return value;
        }
        public static double ThrowIf_Zero(this double value, OperationError operationError)
        {
            if (value == 0)
            {
                throw new OperationUniformException(operationError);
            }

            return value;
        }
        public static byte ThrowIf_Zero(this byte value, OperationError operationError)
        {
            ((double)value).ThrowIf_Zero(operationError);

            return value;
        }

        public static int ThrowIf_Negative(this int value, ArgumentError argumentError, string argumenName)
        {
            if (value.IsNegative())
            {
                throw new ArgumentUniformException(argumentError, $"ArgumentName = {argumenName}");
            }

            return value;
        }
        public static double ThrowIf_Negative(this double value, ArgumentError argumentError, string argumenName)
        {
            value.Sign().ThrowIf_Negative(argumentError, argumenName);

            return value;
        }
    }
}
