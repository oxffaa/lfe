using System;

namespace System.Collections.Concurrent
{
    internal sealed class BoxItem<T>
    {
        private readonly T _value;

        internal BoxItem(T value, BoxItem<T> next = null)
        {
            _value = value;
            Next = next;
        }

        internal T Value
        {
            get { return _value; }
        }

        internal BoxItem<T> Next { get; set; }
    }
}