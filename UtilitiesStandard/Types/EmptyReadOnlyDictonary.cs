using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;
using System.Collections;

namespace Utilities.Types
{
    public class EmptyReadOnlyDictonary : IDictionary
    {
        public object this[object key]
        {
            get => throw new Exception();
            set => throw new Exception();
        }

        public ICollection Keys => new object[0];

        public ICollection Values => new object[0];

        public bool IsReadOnly => true;

        public bool IsFixedSize => true;

        public int Count => 0;

        public object SyncRoot => new object();

        public bool IsSynchronized => true;

        public void Add(object key, object value)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(object key)
        {
            return false;
        }

        public void CopyTo(Array array, int index)
        {
            
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return (new Dictionary<object, object>()).GetEnumerator();
        }

        public void Remove(object key)
        {
            
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Enumerable.Empty<object>().GetEnumerator();
        }
    }
}
