using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities.Types
{
    public class DPerformer
    {
        readonly Dictionary<string, Action> _performedActions = new Dictionary<string, Action>();

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public void PerformOnce(Action action)
        {
            var stackTrace = new StackTrace();
            var stackFrame = stackTrace.GetFrame(1);
            var name = stackFrame.GetMethod().Name;
            var line = stackFrame.GetFileLineNumber();
            var file = stackFrame.GetFileName();

            var id = $"{name}:{line} in {file}";
            PerformOnce(id, action);
        }

        public void PerformOnce(string actionToken, Action action)
        {
            if (!_performedActions.ContainsKey(actionToken))
            {
                action();
                _performedActions.Add(actionToken, action);
            }
        }

        public override string ToString()
        {
            return _performedActions.AsMultilineString();
        }

        public void TraceMessage(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Trace.WriteLine("message: " + message);
            Trace.WriteLine("member name: " + memberName);
            Trace.WriteLine("source file path: " + sourceFilePath);
            Trace.WriteLine("source line number: " + sourceLineNumber);
        }
    }
}
