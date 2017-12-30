using Sharp.Redux.Actions;
using Sharp.Redux.Visualizer.Core;
using Sharp.Redux.Visualizer.Models;
using System;

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
            return ToTreeHierarchy(null, source, 0, propertyName);
        }
        public static ObjectTreeItem ToTreeHierarchy(ObjectTreeItem parent, ObjectData source, int depth, string propertyName)
        {
            if (depth > MaxDepth)
            {
                throw new Exception("Object graph is too deep");
            }
            switch (source)
            {
                case StateObjectData state:
                    return FormatState(parent, depth, propertyName, state);
                case ListData list:
                    return FormatList(parent, depth, propertyName, list);
                case DictionaryData dictionary:
                    return FormatDictionary(parent, depth, propertyName, dictionary);
                case PrimitiveData primitive:
                    return FormatPrimitive(parent, depth, propertyName, primitive);
                default:
                    throw new Exception($"Unknown ObjectData {source.GetType()}");
            }
        }
        public static PrimitiveObjectTreeItem FormatPrimitive(ObjectTreeItem parent, int depth, string propertyName, PrimitiveData source)
        {
           return new PrimitiveObjectTreeItem(source.Value, parent, propertyName, source);
        }
        public static ListObjectTreeItem FormatList(ObjectTreeItem parent, int depth, string propertyName, ListData source)
        {
            return new ListObjectTreeItem(parent, propertyName, source, depth);
        }
        public static DictionaryObjectTreeItem FormatDictionary(ObjectTreeItem parent, int depth, string propertyName, DictionaryData source)
        {
            return new DictionaryObjectTreeItem(parent, propertyName, source, depth);
        }
        /// <summary>
        /// Creates <see cref=" StateObjectTreeItem"/>. Besides properties it could implement either a <see cref="IDictionary"/> or <see cref="IEnumerable"/>
        /// but not both.
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="propertyName"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static StateObjectTreeItem FormatState(ObjectTreeItem parent, int depth, string propertyName, StateObjectData source)
        {
            return new StateObjectTreeItem(parent, propertyName, source, depth);
        }

        public static string GetActionName(ReduxAction action)
        {
            if (action is SpecialReduxAction)
            {
                var fullName = action.GetType().Name;
                return fullName.Substring(0, fullName.Length - "Action".Length);
            }
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
