/*
Copyright (c) 2012 Artem Akulyakov <akulyakov.artem@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal 
in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is furnished 
to do so, subject to the following conditions:

   The above copyright notice and this permission notice shall be included in 
   all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
IN THE SOFTWARE.
*/

using System.Threading;

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
