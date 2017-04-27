using Righthand.Immutable;
using System;

namespace Sharp.Redux.Visualizer.Models
{
    public class PrimitiveObjectTreeItem : ObjectTreeItem
    {
        public object Value { get; }

        public PrimitiveObjectTreeItem(object value, string propertyName, string typeName, bool isRoot) : base(propertyName, typeName, isRoot)
        {
            Value = value;
        }

        public PrimitiveObjectTreeItem Clone(Param<object>? value = null, Param<string>? propertyName = null, Param<string>? typeName = null, Param<bool>? isRoot = null)
        {
            return new PrimitiveObjectTreeItem(value.HasValue ? value.Value.Value : Value,
propertyName.HasValue ? propertyName.Value.Value : PropertyName,
typeName.HasValue ? typeName.Value.Value : TypeName,
isRoot.HasValue ? isRoot.Value.Value : IsRoot);
        }

        public ObjectTreeItem[] Children => null;
        public override string ValueHeader => Convert.ToString(Value);
        public override string DescriptionHeader
        {
            get
            {
                string result;
                if (!string.IsNullOrEmpty(PropertyName))
                {
                    result = PropertyName + ": ";
                }
                else
                {
                    result = null;
                }
                return result;
            }
        }
    }
}
