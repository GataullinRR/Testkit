using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Utilities.Extensions;

namespace Utilities.Types
{
    public interface IObservableCollection<T> : IList<T>, INotifyCollectionChanged
    {

    }

    /// <summary>
    /// This collection removes elements from the beginning to keep it's size constant
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DisplaceCollection<T> : ListBase<T, ObservableCollection<T>>, IObservableCollection<T>
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Max amount of items
        /// </summary>
        public int Capacity { get; }

        public DisplaceCollection(int capacity)
            : base(new ObservableCollection<T>(new List<T>(capacity)))
        {
            Capacity = capacity;
            _baseCollection.CollectionChanged += _baseCollection_CollectionChanged;
        }
        void _baseCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        public void AddRange(IList<T> array)
        {
            var position = Count;
            var freeSpace = Capacity - position;
            var shiftLength = (array.Count - freeSpace).NegativeToZero();
            _baseCollection.MoveLeftSelf(shiftLength);
            position -= shiftLength;
            for (int i = position; i < position + array.Count; i++)
            {
                if (Count <= i)
                {
                    _baseCollection.Add(default);
                }

                _baseCollection[i] = array[i - position];
            }
        }

        public override void Add(T item)
        {
            AddRange(new[] { item });
        }

        public override void Insert(int index, T item)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (index == Count)
            {
                Add(item);
            }
            else if (index < Count)
            {
                var tmp = this.GetRangeSafe(index, Count - index).ToArray();
                this[index] = item;
                for (int i = 0; i < tmp.Length - 1; i++)
                {
                    this[index + 1 + i] = tmp[i];
                }
                Add(tmp.LastItem());
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public DisplaceCollection<T> Fill(T value)
        {
            AddRange(Enumerable.Repeat(value, Capacity - Count).ToArray());

            return this;
        }
    }
}
