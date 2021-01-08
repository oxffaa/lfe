using System;
using System.Collections.Generic;
using System.Threading;

namespace Oxffaa.LFE
{
    public class OutputBox<T>
    {
        private BoxItem<T> _root;

        public OutputBox(IEnumerable<T> source)
        {
            BoxItem<T> current = null;
            
            foreach (var item in source)
                current = new BoxItem<T>(item, current);

            _root = current;
        }

        public bool HasItems => _root != null;

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