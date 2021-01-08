using System.Collections;
using System.Collections.Generic;

namespace Oxffaa.LFE
{
    public class ResultIterator<T> : IEnumerable<T>
    {
        private readonly BoxItem<T> _root;

        internal ResultIterator(BoxItem<T> root) => _root = root;

        public IEnumerator<T> GetEnumerator()
        {
            var current = _root;

            while (current != null)
            {
                yield return current.Value;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool IsEmpty => _root == null;

        public IEnumerable<T> ToEnumerable() => this;
    }
}