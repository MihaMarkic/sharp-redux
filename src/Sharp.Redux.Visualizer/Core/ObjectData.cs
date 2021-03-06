﻿using System.Collections.Immutable;

namespace Sharp.Redux.Visualizer.Core
{
    /// <summary>
    /// Contains object type and property values in a dictionary
    /// </summary>
    public abstract class ObjectData
    {
        public string TypeName { get; }

        public ObjectData(string typeName)
        {
            TypeName = typeName;
        }
    }

    public class PrimitiveData: ObjectData
    {
        public object Value { get; }
        public readonly static PrimitiveData NullValue = new PrimitiveData("", null);
        public PrimitiveData(string typeName, object value): base(typeName)
        {
            Value = value;
        }
    }
    

    public class StateObjectData: ObjectData
    {
        public object Key { get; }
        public bool HasKey { get; }
        public ImmutableDictionary<string, ObjectData> Properties { get; }
        public StateObjectData(string typeName, ImmutableDictionary<string, ObjectData> properties, bool hasKey, object key) : base(typeName)
        {
            HasKey = hasKey;
            Key = key;
            Properties = properties;
        }
    }

    public class ListData: ObjectData
    {
        public ObjectData[] List { get; }
        public ListData(string typeName, ObjectData[] list) : base(typeName)
        {
            List = list;
        }
    }

    public class DictionaryData: ObjectData
    {
        public IImmutableDictionary<object, ObjectData> Dictionary { get;}
        public DictionaryData(string typeName, IImmutableDictionary<object, ObjectData> dictionary) : base(typeName)
        {
            Dictionary = dictionary;
        }
    }
}
