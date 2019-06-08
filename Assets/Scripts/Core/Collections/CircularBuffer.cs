using System.Collections;
using System.Collections.Generic;

namespace pdxpartyparrot.Core.Collections
{
    public class CircularBuffer<T> : ICollection<T>, IReadOnlyCollection<T>
    {
        public int Size { get; }

        private int _head;

        public T Head => Count > 0 ? _elements[_head] : default;

        private int _tail;

        public T Tail => Count > 0 ? _elements[PreviousIndex(_tail)] : default;

        private readonly T[] _elements;

        public CircularBuffer(int size)
        {
            Size = size;
            _elements = new T[Size];

            _head = 0;
            _tail = 0;
        }

        public void RemoveOldest()
        {
            if(Count < 1) {
                return;
            }
            AdvanceIndex(ref _head);
        }

        private void AdvanceIndex(ref int i)
        {
            i = (i + 1) % Size;
        }

        private int PreviousIndex(int i)
        {
            int idx = i - 1;
            return idx < 0 ? Size - 1 : idx;
        }

#region ICollection
        // TODO: this might be off by 1
        public int Count => _head == _tail
            ? 0
            : _tail > _head
                ? _tail - _head
                : Size - _head + _tail;

        public bool IsReadOnly => false;

        public IEnumerator<T> GetEnumerator()
        {
            int idx = _head;
            while(idx != _tail) {
                yield return _elements[idx];
                AdvanceIndex(ref idx);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            int idx = _tail;

            AdvanceIndex(ref _tail);
            if(_tail == _head) {
                AdvanceIndex(ref _head);
            }

            _elements[idx] = item;
        }

        public void Clear()
        {
            _head = 0;
            _tail = 0;
        }

        public bool Contains(T item)
        {
            var comparer = EqualityComparer<T>.Default;

            int idx = _head;
            while(idx != _tail) {
                if(comparer.Equals(_elements[idx], item)) {
                    return true;
                }
                AdvanceIndex(ref idx);
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            int idx = _head;
            while(idx != _tail && arrayIndex < array.Length) {
                array[arrayIndex] = _elements[idx];
                AdvanceIndex(ref idx);
            }
        }

        public bool Remove(T item)
        {
            throw new System.NotImplementedException();
        }
#endregion
    }
}
