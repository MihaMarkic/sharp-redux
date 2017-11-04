using Righthand.Immutable;
using Sharp.Redux.Visualizer.Core;
using System;

namespace Sharp.Redux.Visualizer.Models
{
    public class PrimitiveObjectTreeItem : ObjectTreeItem
    {
        public object Value { get; }

        public PrimitiveObjectTreeItem(object value, string propertyName, ObjectData source, bool isRoot) : base(propertyName, source, isRoot)
        {
            Value = value;
        }

        public PrimitiveObjectTreeItem Clone(Param<object>? value = null, Param<string>? propertyName = null, Param<ObjectData>? source = null, Param<bool>? isRoot = null)
        {
            return new PrimitiveObjectTreeItem(value.HasValue ? value.Value.Value : Value,
propertyName.HasValue ? propertyName.Value.Value : PropertyName,
source.HasValue ? source.Value.Value : Source,
isRoot.HasValue ? isRoot.Value.Value : IsRoot);
        }
        /// <summary>
        /// Not used, but exists to make Wpf binding happy.
        /// </summary>
        public ObjectTreeItem[] Children => null;
        public override string ValueHeader => Convert.ToString(Value);
        public override string DescriptionHeader
        {
            get
            {
                string result;
                if (!string.IsNullOrEmpty(PropertyName))
                {
                    result = PropertyName;
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
