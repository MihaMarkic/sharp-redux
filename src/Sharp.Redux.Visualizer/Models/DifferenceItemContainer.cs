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
    }
}
