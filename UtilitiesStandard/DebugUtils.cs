using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;

namespace Utilities
{
    public static class DebugUtils
    {
        public static void Break()
        {
            try
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
            }
            catch { }
        }

        public static void BreakIf(bool condition)
        {
            if (condition)
            {
                Break();
            }
        }
    }
}
