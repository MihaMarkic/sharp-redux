using Righthand.Immutable;
using Sharp.Redux.Visualizer.Core;

namespace Sharp.Redux.Visualizer.Models
{
    public class DictionaryObjectTreeItem : NodeObjectTreeItem
    {
        public DictionaryObjectTreeItem(ObjectTreeItem[] children, string propertyName, ObjectData source, bool isRoot) : base(children, propertyName, source, isRoot)
        {
        }

        public DictionaryObjectTreeItem Clone(Param<ObjectTreeItem[]>? children = null, Param<string>? propertyName = null, Param<ObjectData>? source = null, Param<bool>? isRoot = null)
        {
            return new DictionaryObjectTreeItem(children.HasValue ? children.Value.Value : Children,
propertyName.HasValue ? propertyName.Value.Value : PropertyName,
source.HasValue ? source.Value.Value : Source,
isRoot.HasValue ? isRoot.Value.Value : IsRoot);
        }
    }
}
