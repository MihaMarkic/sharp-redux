using Sharp.Redux.Visualizer.Core;
using Sharp.Redux.Visualizer.Services.Implementation;
using System;
using System.Collections.Generic;

namespace Sharp.Redux.Visualizer.Models
{
    public class DictionaryObjectTreeItem : ObjectTreeItem, INodeObjectTreeItem
    {
        public ObjectTreeItem[] Children { get; }
        public DictionaryObjectTreeItem(ObjectTreeItem parent, string propertyName, DictionaryData source, int depth) : base(parent, propertyName, source)
        {
            var builder = new List<ObjectTreeItem>(source.Dictionary.Count);
            foreach (var item in source.Dictionary)
            {
                builder.Add(StateFormatter.ToTreeHierarchy(this, item.Value, depth + 1, Convert.ToString(item.Key)));
            }
            Children = builder.ToArray();
        }
        public override string ValueHeader => "";
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
