using System;
using System.Collections.Generic;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Utilities.Types
{
    public class ReadWriteIterator<T> : IEnumerable<T>
    {
        IList<T> _baseCollection;
        public int CurrentIndex { get; private set; }
        public T Current { get; set; }

        public ReadWriteIterator(IList<T> baseColletion)
        {
            _baseCollection = baseColletion;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (CurrentIndex = 0; CurrentIndex < _baseCollection.Count; CurrentIndex++)
            {
                Current = _baseCollection[CurrentIndex];
                yield return Current;
                _baseCollection[CurrentIndex] = Current;
                CurrentIndex++;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
    }
}
