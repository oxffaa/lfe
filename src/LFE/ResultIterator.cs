using System.Collections.Generic;

namespace System.Collections.Concurrent
{
    public class ResultIterator<T> : IEnumerable<T>
    {
        private readonly BoxItem<T> _root;

        internal ResultIterator(BoxItem<T> root)
        {
            _root = root;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var current = _root;

            while (current != null)
            {
                yield return current.Value;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool IsEmpty
        {
            get { return _root == null; }
        }

        public IEnumerable<T> ToEnumerable()
        {
            return this;
        }
    }
}