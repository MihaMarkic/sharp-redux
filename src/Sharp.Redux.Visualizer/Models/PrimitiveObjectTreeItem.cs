using Righthand.Immutable;
using System;

namespace Sharp.Redux.Visualizer.Models
{
    public class PrimitiveObjectTreeItem : ObjectTreeItem
    {
        public object Value { get; }

        public PrimitiveObjectTreeItem(object value, string propertyName, string typeName, bool isRoot, Sharp.Redux.Visualizer.Core.DiffType diffType) : base(propertyName, typeName, isRoot, diffType)
        {
            Value = value;
        }

        public PrimitiveObjectTreeItem Clone(Param<object>? value = null, Param<string>? propertyName = null, Param<string>? typeName = null, Param<bool>? isRoot = null, Param<Sharp.Redux.Visualizer.Core.DiffType>? diffType = null)
        {
            return new PrimitiveObjectTreeItem(value.HasValue ? value.Value.Value : Value,
propertyName.HasValue ? propertyName.Value.Value : PropertyName,
typeName.HasValue ? typeName.Value.Value : TypeName,
isRoot.HasValue ? isRoot.Value.Value : IsRoot,
diffType.HasValue ? diffType.Value.Value : DiffType);
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
