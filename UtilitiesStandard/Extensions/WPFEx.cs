using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities.Extensions
{
    public static class WPFEx
    {
        public static void InvokeImplicitly(this PropertyChangedEventHandler eventHandler, object sender, 
            [CallerMemberName]string propName = "")
        {
            eventHandler.Invoke(sender, new PropertyChangedEventArgs(propName));
        }

        public static void Invoke(this PropertyChangedEventHandler eventHandler, object sender,
    string propName)
        {
            eventHandler.Invoke(sender, new PropertyChangedEventArgs(propName));
        }
    }
}
