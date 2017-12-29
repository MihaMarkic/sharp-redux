using System;

namespace Sharp.Redux
{
    /// <summary>
    /// Represents array extensions.
    /// </summary>
    public static class SpreadExtensions
    {
        /// <summary>
        /// Adds <paramref name="addition"/> to the endo of the array <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T">Type of the element.</typeparam>
        /// <param name="source">Source array.</param>
        /// <param name="addition">Element to append to the source array.</param>
        /// <returns>New array containing appended element.</returns>
        public static T[] Spread<T>(this T[] source, T addition)
        {
            var result = new T[source.Length + 1];
            Array.Copy(source, 0, result, 0, source.Length);
            result[result.Length - 1] = addition;
            return result;
        }
        /// <summary>
        /// Replaces all equal elements to <paramref name="originalItem"/> with <paramref name="newItem"/> in given array <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T">Type of element.</typeparam>
        /// <param name="source">Array with items to replace.</param>
        /// <param name="originalItem">Item to be replaced with <paramref name="newItem"/>.</param>
        /// <param name="newItem">Item that replaces the <paramref name="originalItem"/>.</param>
        /// <returns>New array with replaced items.</returns>
        /// <remarks>To match original item <see cref="Object.Equals(object)"/> is used.</remarks>
        public static T[] Replace<T>(this T[] source, T originalItem, T newItem)
        {
            var result = new T[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                result[i] = Equals(source[i], originalItem) ? newItem : source[i];
            }
            return result;
        }
    }
}
