using Righthand.Immutable;

namespace Sharp.Redux.Visualizer.Models
{
    public class StateObjectTreeItem : NodeObjectTreeItem
    {
        public StateObjectTreeItem(Sharp.Redux.Visualizer.Models.ObjectTreeItem[] children, string propertyName, Sharp.Redux.Visualizer.Core.ObjectData source, bool isRoot) : base(children, propertyName, source, isRoot)
        {
        }

        public StateObjectTreeItem Clone(Param<Sharp.Redux.Visualizer.Models.ObjectTreeItem[]>? children = null, Param<string>? propertyName = null, Param<Sharp.Redux.Visualizer.Core.ObjectData>? source = null, Param<bool>? isRoot = null)
        {
            return new StateObjectTreeItem(children.HasValue ? children.Value.Value : Children,
propertyName.HasValue ? propertyName.Value.Value : PropertyName,
source.HasValue ? source.Value.Value : Source,
isRoot.HasValue ? isRoot.Value.Value : IsRoot);
        }
    }
}
