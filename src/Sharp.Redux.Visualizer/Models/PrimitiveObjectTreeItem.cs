using Sharp.Redux.Visualizer.Core;
using System;

namespace Sharp.Redux.Visualizer.Models
{
    public class PrimitiveObjectTreeItem : ObjectTreeItem
    {
        public object Value { get; }

        public PrimitiveObjectTreeItem(object value, ObjectTreeItem parent, string propertyName, ObjectData source) : base(parent, propertyName, source)
        {
            Value = value;
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
