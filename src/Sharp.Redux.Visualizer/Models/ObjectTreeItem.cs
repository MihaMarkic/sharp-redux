using Sharp.Redux.Visualizer.Core;

namespace Sharp.Redux.Visualizer.Models
{
    public abstract class ObjectTreeItem
    {
        public string PropertyName { get; }
        public string TypeName { get; }
        public bool IsRoot { get; }
        public DiffType DiffType { get; }

        public  ObjectTreeItem(string propertyName, string typeName, bool isRoot, DiffType diffType)
        {
            PropertyName = propertyName;
            TypeName = typeName;
            IsRoot = isRoot;
            DiffType = diffType;
        }

        public abstract string ValueHeader { get; }
        public abstract string DescriptionHeader { get; }
    }
}
