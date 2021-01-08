using System.Threading;

namespace Oxffaa.LFE
{
    /// <summary>
    /// Generic thread-safe and lock-free collection for
    /// implementing pattern `multiple producers - one consumer`.
    /// Collection uses custom linked list under hood and works similar to a fifo queue.
    /// </summary>
    /// <typeparam name="T">
    /// Any required type
    /// </typeparam>
    public class InputBox<T>
    {
        private BoxItem<T> _current;

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        public InputBox() => _current = null;

        /// <summary>
        /// Add a value to the end of the collection
        /// </summary>
        /// <param name="value">
        /// The value to be addded
        /// </param>
        public void Add(T value)
        {
            var nextItem = new BoxItem<T>(value);
            BoxItem<T> initialItem = null;
            
            do
            {
                initialItem = _current;
                nextItem.Next = initialItem;
            } while (
                initialItem != Interlocked.CompareExchange(
                    ref _current,
                    nextItem,
                    initialItem
                )
            );
        }

        /// <summary>
        /// Try add a value to the end of the collection
        /// </summary>
        /// <param name="value">
        /// The value to be added
        /// </param>
        /// <returns>
        /// true if the value was added successfully; false if the value wasn't added
        /// </returns>
        public bool TryAdd(T value)
        {
            var initialItem = _current;
            
            return initialItem == Interlocked.CompareExchange(
                ref _current,
                new BoxItem<T>(value, _current),
                initialItem
            );
        }

        /// <summary>
        /// Take all values from the collection and empty it
        /// </summary>
        /// <returns>
        /// The iterator over returned values
        /// </returns>
        public ResultIterator<T> TakeAll()
        {
            BoxItem<T> initialItem = null;
            
            do
            {
                initialItem = _current;
            } while (
                initialItem != Interlocked.CompareExchange(
                    ref _current, 
                    null, 
                    initialItem
                )
            );

            return new ResultIterator<T>(initialItem);
        }
    }
}