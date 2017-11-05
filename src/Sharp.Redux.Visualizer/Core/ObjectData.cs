using System.Collections.Immutable;

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
        public object Value { get; set;  }
        public readonly static PrimitiveData NullValue = new PrimitiveData("");
        public PrimitiveData(string typeName, object value = default): base(typeName)
        {
            Value = value;
        }
    }
    

    public class StateObjectData: ObjectData
    {
        public ImmutableDictionary<string, ObjectData> Properties { get; set; }
        public StateObjectData(string typeName, ImmutableDictionary<string, ObjectData> properties = default) : base(typeName)
        {
            Properties = properties;
        }
    }

    public class ListData: ObjectData
    {
        public ObjectData[] List { get; set; }
        public ListData(string typeName, ObjectData[] list = default) : base(typeName)
        {
            List = list;
        }
    }

    public class DictionaryData: ObjectData
    {
        public IImmutableDictionary<object, ObjectData> Dictionary { get; set; }
        public DictionaryData(string typeName, IImmutableDictionary<object, ObjectData> dictionary = default) : base(typeName)
        {
            Dictionary = dictionary;
        }
    }
    public class RecursiveObjectData : ObjectData
    {
        public object Source { get; }
        public RecursiveObjectData(object source) : base("Recursive")
        {
            Source = source;
        }
    }
}
