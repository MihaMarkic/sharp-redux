using System;
using System.Collections.Generic;

namespace Sharp.Redux
{
    public static class ReduxMerger
    {
        public static MergeResult Merge<TKey, TSource, TTarget>(IEnumerable<TSource> source, IList<TTarget> target, Func<TSource, TTarget> creator)
            where TSource: IKeyedItem<TKey>
            where TTarget : IKeyedItem<TKey>
        {
            MergeResult result = new MergeResult();
            List<TSource> curent = new List<TSource>(source);
            TSource match;
            for (int i = target.Count - 1; i >= 0; i--)
            {
                TKey targetKey = target[i].Key;
                if (!Find(targetKey, source, out match))
                {
                    target.RemoveAt(i);
                    result.Removed++;
                }
                else
                {
                    curent.Remove(match);
                }
            }
            foreach (TSource item in curent)
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

    public struct MergeResult
    {
        public int Added { get; set; }
        public int Removed { get; set; }
    }
}
