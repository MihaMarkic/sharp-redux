namespace System.Collections.Generic
{
    public static class ListExtensions
    {
        public static void AddIfNotNull<T>(this List<T> list, T item)
            where T: class
        {
            if (!(item is default))
            {
                list.Add(item);
            }
        }
    }
}
