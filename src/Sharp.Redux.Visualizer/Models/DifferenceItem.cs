using Righthand.Immutable;
using Sharp.Redux.Visualizer.Core;

namespace Sharp.Redux.Visualizer.Models
{
    public class DifferenceItem
    {
        public ObjectTreeItem Current { get; }
        public ObjectTreeItem Next { get; }
        public DiffType DiffType { get; }

        public DifferenceItem(ObjectTreeItem current, ObjectTreeItem next, DiffType diffType)
        {
            Current = current;
            Next = next;
            DiffType = diffType;
        }

        public DifferenceItem Clone(Param<ObjectTreeItem>? current = null, Param<ObjectTreeItem>? next = null, Param<DiffType>? diffType = null)
        {
            return new DifferenceItem(current.HasValue ? current.Value.Value : Current,
next.HasValue ? next.Value.Value : Next,
diffType.HasValue ? diffType.Value.Value : DiffType);
        }
    }
}
