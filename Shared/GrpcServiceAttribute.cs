using System;

namespace Shared
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GrpcServiceAttribute : Attribute
    {

    }
}
