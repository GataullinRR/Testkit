using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities.Types
{
    public class GCPinner : IDisposable
    {
        GCHandle _pinnedArray;

        public IntPtr Address => _pinnedArray.AddrOfPinnedObject();

        public GCPinner(object objectForPin)
        {
            _pinnedArray = GCHandle.Alloc(objectForPin, GCHandleType.Pinned);
        }

        public void Dispose()
        {
            _pinnedArray.Free();
        }
    }
}
