using Sharp.Redux.Visualizer.Core;
using Righthand.Immutable;

namespace Sharp.Redux.Visualizer.Models
{
    public abstract class ObjectTreeItem
    {
        public string PropertyName { get; }
        public ObjectData Source { get; }
        public bool IsRoot { get; }

        public  ObjectTreeItem(string propertyName, ObjectData source, bool isRoot)
        {
            PropertyName = propertyName;
            Source = source;
            IsRoot = isRoot;
        }
        public string TypeName => Source?.TypeName;
        public abstract string ValueHeader { get; }
        public abstract string DescriptionHeader { get; }
    }
}
