using Sharp.Redux.Visualizer.Core;
using Sharp.Redux.Visualizer.Models;
using System;
using System.Collections.Generic;

namespace Sharp.Redux.Visualizer.Services.Implementation
{
    /// <summary>
    /// Transforms state expressed as <see cref="ObjectData"/> and descending types to <see cref="ObjectTreeItem"/> suitable
    /// for display in a tree structure.
    /// </summary>
    public static class StateFormatter
    {
        public const string DefaultPropertyName = "State";
        public const int MaxDepth = 1000;
        public static ObjectTreeItem ToTreeHierarchy(ObjectData source, string propertyName = DefaultPropertyName)
        {
            return ToTreeHierarchy(source, 0, propertyName);
        }
        public static ObjectTreeItem ToTreeHierarchy(ObjectData source, int depth, string propertyName)
        {
            if (depth > MaxDepth)
            {
                throw new Exception("Object graph is too deep");
            }
            switch (source)
            {
                case StateObjectData state:
                    return FormatState(depth, propertyName, state);
                case ListData list:
                    return FormatList(depth, propertyName, list);
                case DictionaryData dictionary:
                    return FormatDictionary(depth, propertyName, dictionary);
                case PrimitiveData primitive:
                    return FormatPrimitive(depth, propertyName, primitive);
                case RecursiveObjectData recursive:
                    return new RecursiveObjectTreeItem(null, propertyName, recursive, isRoot: depth == 0);
                default:
                    throw new Exception($"Unknown ObjectData {source.GetType()}");
            }
        }
        public static PrimitiveObjectTreeItem FormatPrimitive(int depth, string propertyName, PrimitiveData source)
        {
           return new PrimitiveObjectTreeItem(source.Value, propertyName, source, isRoot: depth==0);
        }
        public static ListObjectTreeItem FormatList(int depth, string propertyName, ListData source)
        {
            var builder = new List<ObjectTreeItem>(source.List.Length);
            foreach (var item in source.List)
            {
                builder.Add(ToTreeHierarchy(item, depth+1, null));
            }
            return new ListObjectTreeItem(builder.ToArray(), propertyName, source, isRoot: depth == 0);
        }
        public static DictionaryObjectTreeItem FormatDictionary(int depth, string propertyName, DictionaryData source)
        {
            var builder = new List<ObjectTreeItem>(source.Dictionary.Count);
            foreach (var item in source.Dictionary)
            {
                builder.Add(ToTreeHierarchy(item.Value, depth+1, Convert.ToString(item.Key)));
            }
            return new DictionaryObjectTreeItem(builder.ToArray(), propertyName, source, isRoot: depth == 0);
        }
        /// <summary>
        /// Creates <see cref=" StateObjectTreeItem"/>. Besides properties it could implement either a <see cref="IDictionary"/> or <see cref="IEnumerable"/>
        /// but not both.
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="propertyName"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static StateObjectTreeItem FormatState(int depth, string propertyName, StateObjectData source)
        {
            var properties = new List<ObjectTreeItem>(source.Properties.Count);
            foreach (var item in source.Properties)
            {
                properties.Add(ToTreeHierarchy(item.Value, depth+1, item.Key));
            }
            return new StateObjectTreeItem(properties.ToArray(), propertyName, source, isRoot: depth == 0);
        }

        public static string GetActionName(ReduxAction action)
        {
            const string Suffix = "Action";
            var name = action.GetType().FullName;
            foreach (string prefix in ReduxVisualizer.IgnoredNamespacePrefixes)
            {
                if (name.StartsWith(prefix, StringComparison.Ordinal))
                {
                    name = name.Substring(prefix.Length + 1);
                }
            }
            if (name.EndsWith(Suffix))
            {
                name = name.Substring(0, name.Length - Suffix.Length);
            }
            return name;
        }
    }
}
