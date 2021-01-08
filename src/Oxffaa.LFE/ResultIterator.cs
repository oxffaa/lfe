using System.Collections;
using System.Collections.Generic;

namespace Oxffaa.LFE
{
    /// <summary>
    /// Iterator over returned items
    /// </summary>
    /// <typeparam name="T">
    /// Any required type
    /// </typeparam>
    public class ResultIterator<T> : IEnumerable<T>
    {
        private readonly BoxItem<T> _root;

        internal ResultIterator(BoxItem<T> root) => _root = root;

        /// <summary>
        /// Return the enumerator for iterating over the result set
        /// </summary>
        /// <returns>
        /// The iterator
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            var current = _root;

            while (current != null)
            {
                yield return current.Value;
                current = current.Next;
            }
        }

        /// <summary>
        /// Return the enumerator for iterating over the result set
        /// </summary>
        /// <returns>
        /// The iterator
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// True if iterator isn't empty
        /// </summary>
        public bool IsEmpty => _root == null;

        /// <summary>
        /// Cast to IEnumerable
        /// </summary>
        /// <returns>
        /// The iterator as IEnumerable
        /// </returns>
        public IEnumerable<T> ToEnumerable() => this;
    }
}