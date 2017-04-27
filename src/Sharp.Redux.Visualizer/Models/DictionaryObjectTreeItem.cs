using Sharp.Redux.Visualizer.Core;
using Righthand.Immutable;

namespace Sharp.Redux.Visualizer.Models
{
    public class DictionaryObjectTreeItem : NodeObjectTreeItem
    {
        public DictionaryObjectTreeItem(Sharp.Redux.Visualizer.Models.ObjectTreeItem[] children, string propertyName, string typeName, bool isRoot) : base(children, propertyName, typeName, isRoot)
        {
        }

        public DictionaryObjectTreeItem Clone(Param<Sharp.Redux.Visualizer.Models.ObjectTreeItem[]>? children = null, Param<string>? propertyName = null, Param<string>? typeName = null, Param<bool>? isRoot = null)
        {
            return new DictionaryObjectTreeItem(children.HasValue ? children.Value.Value : Children,
propertyName.HasValue ? propertyName.Value.Value : PropertyName,
typeName.HasValue ? typeName.Value.Value : TypeName,
isRoot.HasValue ? isRoot.Value.Value : IsRoot);
        }
    }
}
