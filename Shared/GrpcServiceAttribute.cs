using System;

namespace SharedT
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GrpcServiceAttribute : Attribute
    {

    }
}
