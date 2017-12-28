using Sharp.Redux.Visualizer.Core;
using System.Diagnostics;

namespace Sharp.Redux.Visualizer.Models
{
    [DebuggerDisplay("{DescriptionHeader,nq}")]
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

        public string Id
        {
            get
            {
                switch (DiffType)
                {
                    case DiffType.Removed:
                        return Current.ValueHeader;
                    case DiffType.Modified:
                        return $"{Current?.ValueHeader} -> {Next?.ValueHeader}";
                    default:
                        return Next.ValueHeader;
                }
            }
        }

        public string ValueHeader
        {
            get
            {
                switch (DiffType)
                {
                    case DiffType.Removed:
                        return Current.ValueHeader;
                    case DiffType.Modified:
                        if (Current is PrimitiveObjectTreeItem || Next is PrimitiveObjectTreeItem)
                        {
                            return $"{Current?.ValueHeader} -> {Next?.ValueHeader}";
                        }
                        else
                        {
                            return "";
                        }
                    default:
                        return Next.ValueHeader;
                }
            }
        }
        public string DescriptionHeader
        {
            get
            {
                switch (DiffType)
                {
                    case DiffType.Removed:
                        return Current.DescriptionHeader;
                    default:
                        return Next.DescriptionHeader;
                }
            }
        }
    }
}
