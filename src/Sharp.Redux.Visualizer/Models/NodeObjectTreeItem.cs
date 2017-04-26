namespace Sharp.Redux.Visualizer.Models
{
    public abstract class NodeObjectTreeItem : ObjectTreeItem
    {
        public ObjectTreeItem[] Children { get; }

        public NodeObjectTreeItem(ObjectTreeItem[] children, string propertyName, string typeName, bool isRoot, Sharp.Redux.Visualizer.Core.DiffType diffType) : base(propertyName, typeName, isRoot, diffType)
        {
            Children = children;
        }

        public override string ValueHeader => "";
        public override string DescriptionHeader
        {
            get
            {
                string result;
                if (!string.IsNullOrEmpty(PropertyName))
                {
                    result = PropertyName;
                }
                else
                {
                    result = null;
                }
                return result;
            }
        }
    }
}
