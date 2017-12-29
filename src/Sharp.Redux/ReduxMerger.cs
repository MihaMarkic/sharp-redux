using System;
using System.Collections.Generic;

namespace Sharp.Redux
{
    /// <summary>
    /// Represents helper methods to synchronize target types with redux state. Common target types are view models.
    /// </summary>
    public static class ReduxMerger
    {
        /// <summary>
        /// Merges a dictionary.
        /// </summary>
        /// <typeparam name="TKey">Type of key.</typeparam>
        /// <typeparam name="TValue">Type of dictionary value.</typeparam>
        /// <typeparam name="TTargetValue">Type of target value.</typeparam>
        /// <param name="source">Redux state type.</param>
        /// <param name="target">Target type linked to <paramref name="source"/>.</param>
        /// <param name="creator">Creator function that creates new instances of target types.</param>
        /// <returns>Merge result.</returns>
        public static MergeResult MergeDictionary<TKey, TValue, TTargetValue>(
            IDictionary<TKey, TValue> source, 
            IList<KeyValuePair<TKey, TTargetValue>> target, 
            Func<TValue, TTargetValue> creator)
            where TTargetValue: IBoundViewModel<TValue>
        {
            MergeResult result = new MergeResult();
            var current = new Dictionary<TKey, TValue>(source);
            for (int i = target.Count - 1; i >= 0; i--)
            {
                TKey targetKey = target[i].Key;
                if (!source.TryGetValue(targetKey, out var match))
                {
                    target.RemoveAt(i);
                    result.Removed++;
                }
                else
                {
                    current.Remove(targetKey);
                    if (!ReferenceEquals(match, target[i].Value.State))
                    {
                        target[i].Value.Update(match);
                        result.Updated++;
                    }
                }
            }
            foreach (var pair in current)
            {
                var value = creator(pair.Value);
                target.Add(new KeyValuePair<TKey, TTargetValue>(pair.Key, value));
                result.Added++;
            }
            return result;
        }
        /// <summary>
        /// Merges a list.
        /// </summary>
        /// <typeparam name="TKey">Type of key.</typeparam>
        /// <typeparam name="TSource">State type.</typeparam>
        /// <typeparam name="TTarget">Target type.</typeparam>
        /// <param name="source">Redux state type.</param>
        /// <param name="target">Target type linked to <paramref name="source"/>.</param>
        /// <param name="creator">Creator function that creates new instances of target types.</param>
        /// <returns>Merge result.</returns>
        public static MergeResult MergeList<TKey, TSource, TTarget>(IEnumerable<TSource> source, IList<TTarget> target, Func<TSource, TTarget> creator)
            where TSource: IKeyedItem<TKey>
            where TTarget : IBoundViewModel<TSource>
        {
            MergeResult result = new MergeResult();
            List<TSource> current = new List<TSource>(source);
            for (int i = target.Count - 1; i >= 0; i--)
            {
                TKey targetKey = target[i].State.Key;
                var findResult = Find(targetKey, source);
                if (!findResult.Found)
                {
                    target.RemoveAt(i);
                    result.Removed++;
                }
                else
                {
                    current.Remove(findResult.Match);
                    if (!ReferenceEquals(findResult.Match, target[i].State))
                    {
                        target[i].Update(findResult.Match);
                        result.Updated++;
                    }
                }
            }
            foreach (TSource item in current)
            {
                target.Add(creator(item));
                result.Added++;
            }
            return result;
        }
        /// <summary>
        /// Finds target instance with the given key.
        /// </summary>
        /// <typeparam name="TKey">Type of key.</typeparam>
        /// <typeparam name="TSource">State type.</typeparam>
        /// <param name="key">Key to search for.</param>
        /// <param name="items">Items to search for given key.</param>
        /// <returns>An instance of <see cref="FindResult{T}"/>.</returns>
        public static FindResult<TSource> Find<TKey, TSource>(TKey key, IEnumerable<TSource> items)
            where TSource: IKeyedItem<TKey>
        {
            foreach (var item in items)
            {
                if (Equals(item.Key, key))
                {
                    return new FindResult<TSource>(true, item);
                }
            }
            return new FindResult<TSource>(false, default(TSource));
        }
        /// <summary>
        /// Represents Find results.
        /// </summary>
        /// <typeparam name="T">Type of matched item.</typeparam>
        public struct FindResult<T>
        {
            /// <summary>
            /// Gets whether the find was successful.
            /// </summary>
            public  bool Found { get; }
            /// <summary>
            /// Gets the found item when result was successful. Otherwise the default value for the type.
            /// </summary>
            public T Match { get; }
            public FindResult(bool found, T match)
            {
                Found = found;
                Match = match;
            }
        }
    }
    /// <summary>
    /// Represents merge result.
    /// </summary>
    public struct MergeResult
    {
        /// <summary>
        /// Number of target instances added.
        /// </summary>
        public int Added { get; set; }
        /// <summary>
        /// Number of target instances removed.
        /// </summary>
        public int Removed { get; set; }
        /// <summary>
        /// Number of target instances modified.
        /// </summary>
        public int Updated { get; set; }
    }
}
