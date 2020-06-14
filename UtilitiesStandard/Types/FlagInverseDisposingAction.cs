using System;

namespace Utilities.Types
{
    public class FlagInverseAction : DisposingAction
    {
        public FlagInverseAction(bool initialValue, Action<bool> flagSetter)
            : base(() => flagSetter(!initialValue))
        {
            flagSetter(initialValue);
        }
    }
}
