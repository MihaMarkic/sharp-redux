using Sharp.Redux.Visualizer.Core;
using Sharp.Redux.Visualizer.States;
using System;
using System.Collections.Generic;

namespace Sharp.Redux.Visualizer.Services.Implementation
{
    public static class StateFormatter
    {
        public static ObjectTreeItem ToTreeHierarchy(ObjectData source, string propertyName = "")
        {
            switch (source)
            {
                case StateObjectData state:
                    return FormatState(propertyName, state);
                case ListData list:
                    return FormatList(propertyName, list);
                case DictionaryData dictionary:
                    return FormatDictionary(propertyName, dictionary);
                case PrimitiveData primitive:
                    return FormatPrimitive(propertyName, primitive);
                default:
                    throw new Exception($"Unknown ObjectData {source.GetType()}");
            }
        }
        public static ObjectTreeItem FormatPrimitive(string propertyName, PrimitiveData source)
        {
           return new ObjectTreeItem(propertyName, source.TypeName, Convert.ToString(source.Value), null);
        }
        public static ObjectTreeItem FormatList(string propertyName, ListData source)
        {
            var builder = new List<ObjectTreeItem>(source.List.Length);
            foreach (var item in source.List)
            {
                builder.Add(ToTreeHierarchy(item));
            }
            return new ObjectTreeItem(propertyName, source.TypeName, null, builder.ToArray());
        }
        public static ObjectTreeItem FormatDictionary(string propertyName, DictionaryData source)
        {
            var builder = new List<ObjectTreeItem>(source.Dictionary.Count);
            foreach (var item in source.Dictionary)
            {
                builder.Add(ToTreeHierarchy(item.Value, Convert.ToString(item.Key)));
            }
            return new ObjectTreeItem(propertyName, source.TypeName, null, builder.ToArray());
        }
        public static ObjectTreeItem FormatState(string propertyName, StateObjectData source)
        {
            var builder = new List<ObjectTreeItem>(source.Properties.Count);
            foreach (var item in source.Properties)
            {
                builder.Add(ToTreeHierarchy(item.Value, item.Key));
            }
            return new ObjectTreeItem(propertyName, source.TypeName, null, builder.ToArray());
        }

        //public const string Level = "  ";
        //public static void Format(StringBuilder sb, ObjectData source, string prefix = "")
        //{
        //    switch (source)
        //    {
        //        case StateObjectData state:
        //            FormatState(sb, state, prefix);
        //            break;
        //        case ListData list:
        //            FormatList(sb, list, prefix);
        //            break;
        //        case DictionaryData dictionary:
        //            FormatDictionary(sb, dictionary, prefix);
        //            break;
        //        case PrimitiveData primitive:
        //            FormatPrimitive(sb, primitive);
        //            break;
        //    }
        //}

        //public static void FormatState(StringBuilder sb, StateObjectData source, string prefix)
        //{
        //    sb.AppendLine("{");
        //    foreach (var property in source.Properties)
        //    {
        //        sb.Append($"{property.Key}: ");
        //        Format(sb, property.Value, prefix + Level);
        //    }
        //    sb.AppendLine("}");
        //}
        //public static void FormatList(StringBuilder sb, ListData source, string prefix)
        //{
        //    sb.Append("[");
        //    foreach (var item in source.List)
        //    {
        //        //Format(sb, property.Value, prefix + Level);
        //    }
        //    sb.Append("}");
        //}
        //public static void FormatDictionary(StringBuilder sb, DictionaryData source, string prefix)
        //{
        //}
        //public static void FormatPrimitive(StringBuilder sb, PrimitiveData source)
        //{
        //    sb.Append($"'{source.Value}'");
        //}
    }
}
