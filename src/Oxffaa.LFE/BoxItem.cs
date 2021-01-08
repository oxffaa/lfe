namespace Oxffaa.LFE
{
    internal sealed class BoxItem<T>
    {
        internal BoxItem(T value, BoxItem<T> next = null)
        {
            Value = value;
            Next = next;
        }

        internal T Value { get; }
        internal BoxItem<T> Next { get; set; }
    }
}