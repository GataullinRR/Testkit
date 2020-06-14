using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Types;

namespace Utilities.Extensions
{
    public static class DisposingEx
    {
        public static IDisposable ThenDispose(this IDisposable disposingAction, IDisposable disposable)
        {
            return new DisposingActions() { disposingAction, disposable };
        }
    }
}
