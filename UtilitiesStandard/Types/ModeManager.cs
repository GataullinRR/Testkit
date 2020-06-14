using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;
using System.Diagnostics;

namespace Utilities.Types
{
    public class ModeManager
    {
        public event Action Activated;
        public event Action Deactivated;
        
        public int ActiveHoldersCount { get; private set; }
        public bool IsActive { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IDisposable Holder
        {
            get
            {
                var isActivated = IsActive == false;
                IsActive = true;
                ActiveHoldersCount++;
                var restorer = new DisposingAction(restore);
                try
                {
                    Activated?.Invoke();

                    return restorer;
                }
                catch
                {
                    restorer.To<IDisposable>().Dispose();

                    throw;
                }

                void restore()
                {
                    ActiveHoldersCount--;
                    if (ActiveHoldersCount <= 0)
                    {
                        IsActive = false;
                        Deactivated?.Invoke();
                    }
                }
            }
        }
    }
}
