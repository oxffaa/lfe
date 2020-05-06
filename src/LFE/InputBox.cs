using System.Threading;

namespace System.Collections.Concurrent
{   
    namespace System.Collections.Concurrent
    {
        public class InputBox<T>
        {
            private BoxItem<T> _current;
    
            public InputBox()
            {
                _current = null;
            }
    
            public void Add(T value)
            {
                var nextItem = new BoxItem<T>(value);
                BoxItem<T> initialItem = null;
                do
                {
                    initialItem = _current;
                    nextItem.Next = initialItem;
                } while (initialItem != Interlocked.CompareExchange(ref _current,
                                                                    nextItem,
                                                                    initialItem));
            }
    
            public bool TryAdd(T value)
            {
                var initialItem = _current;
                return initialItem ==
                       Interlocked.CompareExchange(ref _current, 
                                                   new BoxItem<T>(value, _current), 
                                                   initialItem);
            }
    
            public ResultIterator<T> TakeAll()
            {
                BoxItem<T> initialItem = null;
                do
                {
                    initialItem = _current;
                } while (initialItem !=
                         Interlocked.CompareExchange(ref _current, null, initialItem));
    
                return new ResultIterator<T>(initialItem);
            }
        }
    }
}