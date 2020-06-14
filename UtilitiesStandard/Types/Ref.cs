using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;

namespace Utilities.Types
{
    public class Ref<T> 
    {
        readonly Func<T> _getter;
        readonly Action<T> _setter;

        public T Value
        {
            get { return _getter(); }
            set { _setter(value); }
        }

        public Ref(Func<T> getter)
        {
            _getter = getter;
        }

        public Ref(Func<T> getter, Action<T> setter)
        {
            _getter = getter;
            _setter = setter;
        }
    }
}
