using Sharp.Redux.Visualizer.Core;
using Sharp.Redux.Visualizer.Services.Implementation;
using System.Collections.Generic;

namespace Sharp.Redux.Visualizer.Models
{
    public class ListObjectTreeItem : ObjectTreeItem, INodeObjectTreeItem
    {
        public ObjectTreeItem[] Children { get; }
        public ListObjectTreeItem(ObjectTreeItem parent, string propertyName, ListData source, int depth) : 
            base(parent, propertyName, source)
        {
            var builder = new List<ObjectTreeItem>(source.List.Length);
            foreach (var item in source.List)
            {
                builder.Add(StateFormatter.ToTreeHierarchy(this, item, depth + 1, null));
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
