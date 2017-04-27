using Sharp.Redux.Visualizer.Core;
using Righthand.Immutable;

namespace Sharp.Redux.Visualizer.Models
{
    public abstract class ObjectTreeItem
    {
        public string PropertyName { get; }
        public string TypeName { get; }
        public bool IsRoot { get; }

        public  ObjectTreeItem(string propertyName, string typeName, bool isRoot)
        {
            PropertyName = propertyName;
            TypeName = typeName;
            IsRoot = isRoot;
        }

        public abstract string ValueHeader { get; }
        public abstract string DescriptionHeader { get; }
    }
}
