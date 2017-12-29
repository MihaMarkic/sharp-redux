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
        public static MergeResult MergeList<TKey, TSource, TTarget>(IEnumerable<TSource> source, IList<TTarget> target, Func<TSource, TTarget> creator)
            where TSource: IKeyedItem<TKey>
            where TTarget : IBoundViewModel<TSource>
        {
            MergeResult result = new MergeResult();
            List<TSource> current = new List<TSource>(source);
            for (int i = target.Count - 1; i >= 0; i--)
            {
                TKey targetKey = target[i].State.Key;
                if (!Find(targetKey, source, out var match))
                {
                    target.RemoveAt(i);
                    result.Removed++;
                }
                else
                {
                    current.Remove(match);
                    if (!ReferenceEquals(match, target[i].State))
                    {
                        target[i].Update(match);
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

        public static bool Find<TKey, TSource>(TKey key, IEnumerable<TSource> items, out TSource match)
            where TSource: IKeyedItem<TKey>
        {
            foreach (var item in items)
            {
                if (Equals(item.Key, key))
                {
                    match = item;
                    return true;
                }
            }
            match = default(TSource);
            return false;
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
