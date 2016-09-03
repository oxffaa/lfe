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

using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace System.Collections.Concurrent
{
    public class OutputBox<T>
    {
        private BoxItem<T> _root; 

        public OutputBox(IEnumerable<T> source)
        {
            _root = source.Aggregate(_root, (current, item) => new BoxItem<T>(item, current));
        }

        public bool HasItems
        {
            get { return _root != null; }
        }

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

        public bool TryTake(out T item)
        {
            var currentRoot = _root;

            if (currentRoot == null)
                throw new InvalidOperationException("Box is empty.");

            var result = currentRoot ==
                         Interlocked.CompareExchange(ref _root,
                                                     currentRoot.Next,
                                                     currentRoot);

            item = result ? currentRoot.Value : default(T);

            return result;
        }
    }
}