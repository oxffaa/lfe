using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Oxffaa.LFE
{
    /// <summary>
    /// Generic thread-safe and lock-free collection for parallel handling a set of values.
    /// Collection uses custom linked list under hood and works similar to a fifo queue.
    /// </summary>
    /// <typeparam name="T">
    /// Any required type
    /// </typeparam>
    public class OutputBox<T>
    {
        private BoxItem<T> _root;

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        /// <param name="source">
        /// The set of values for consumers
        /// </param>
        public OutputBox(IEnumerable<T> source) => _root = source
            .Aggregate<T, BoxItem<T>>(
                null, 
                (cur, val) => new BoxItem<T>(val, cur)
            );

        /// <summary>
        /// Indicates that the collection isn't empty
        /// </summary>
        public bool HasItems => _root != null;

        /// <summary>
        /// Take and remove the value from the coolection
        /// </summary>
        /// <returns>
        /// The value
        /// </returns>
        public T Take()
        {
            var result = default(T);
            bool success = false;

            do
            {
                success = TryTake(out result);
            } while (!success);

            return result;
        }
        
        /// <summary>
        /// Try take and remove the value from the collection
        /// </summary>
        /// <param name="item">
        /// The value
        /// </param>
        /// <returns>
        /// true if the value was returned successfully; otherwise false
        /// </returns>
        public bool TryTake(out T item)
        {
            var currentRoot = _root;

            if (currentRoot == null)
            {
                item = default;
                return false;
            }

            var result = currentRoot == Interlocked.CompareExchange(
                ref _root,
                currentRoot.Next,
                currentRoot
            );

            item = result ? currentRoot.Value : default;

            return result;
        }
    }
}