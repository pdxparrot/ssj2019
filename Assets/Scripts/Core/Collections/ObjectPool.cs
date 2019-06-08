using System.Collections.Generic;

using JetBrains.Annotations;

namespace pdxpartyparrot.Core.Collections
{
    public interface IPooledItem
    {
        void Reset();
    }

    public sealed class ObjectPool<T> where T: class, IPooledItem, new()
    {
        private class ObjectPoolElement
        {
            public bool InUse { get; set; }

            public T Item { get; set; }
        }

        public int Size { get; }

        public int Used { get; private set; }

        public int Free => Size - Used;

        private readonly ObjectPoolElement[] _elements;

        public ObjectPool(int size)
        {
            Size = size;

            _elements = new ObjectPoolElement[Size];
            foreach(ObjectPoolElement e in _elements) {
                e.Item = new T();
            }
        }

        [CanBeNull]
        public T Acquire()
        {
            foreach(ObjectPoolElement e in _elements) {
                if(e.InUse) {
                    continue;
                }

                Used++;

                T item = e.Item;
                item.Reset();
                return item;
            }
            return null;
        }

        public void Release(T item)
        {
            foreach(ObjectPoolElement e in _elements) {
                if(e.Item != item) {
                    continue;
                }

                Used--;

                e.InUse = false;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach(ObjectPoolElement e in _elements) {
                if(e.InUse) {
                    yield return e.Item;
                }
            }
        }
    }
}
