using System;

namespace Sharp.Redux
{
    public static class SpreadExtensions
    {
        public static T[] Spread<T>(this T[] source, T addition)
        {
            var result = new T[source.Length + 1];
            Array.Copy(source, 0, result, 0, source.Length);
            result[result.Length - 1] = addition;
            return result;
        }
    }
}
