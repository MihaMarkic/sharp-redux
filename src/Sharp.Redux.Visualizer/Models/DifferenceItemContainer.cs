using Righthand.Immutable;
using Sharp.Redux.Visualizer.Core;

namespace Sharp.Redux.Visualizer.Models
{
    public class DifferenceItemContainer : DifferenceItem
    {
        public DifferenceItem[] Children { get; }

        public DifferenceItemContainer(DifferenceItem[] children, ObjectTreeItem current, ObjectTreeItem next, DiffType diffType) : base(current, next, diffType)
        {
            Children = children;
        }

        public DifferenceItemContainer Clone(Param<DifferenceItem[]>? children = null, Param<ObjectTreeItem>? current = null, Param<ObjectTreeItem>? next = null, Param<DiffType>? diffType = null)
        {
            return new DifferenceItemContainer(children.HasValue ? children.Value.Value : Children,
current.HasValue ? current.Value.Value : Current,
next.HasValue ? next.Value.Value : Next,
diffType.HasValue ? diffType.Value.Value : DiffType);
        }
    }
}
