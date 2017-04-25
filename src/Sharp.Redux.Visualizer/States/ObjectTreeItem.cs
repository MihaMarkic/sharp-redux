namespace Sharp.Redux.Visualizer.States
{
    public class ObjectTreeItem
    {
        public bool IsRoot { get; }
        public string PropertyName { get; }
        public string TypeName { get; }
        public string Value { get; }
        public ObjectTreeItem[] Children { get; }

        public  ObjectTreeItem(string propertyName, string typeName, string value, ObjectTreeItem[] children, bool isRoot = false)
        {
            PropertyName = propertyName;
            TypeName = typeName;
            Value = value;
            Children = children;
            IsRoot = isRoot;
        }

        public string Header
        {
            get
            {
                string result;
                if (!string.IsNullOrEmpty(PropertyName))
                {
                    result = PropertyName;
                    if (Children == null)
                    {
                        result += ": " + Value;
                    }
                    return result;
                }
                else
                {
                    return Value;
                }
            }
        }

        public string DescriptionHeader
        {
            get
            {
                string result;
                if (!string.IsNullOrEmpty(PropertyName))
                {
                    result = PropertyName;
                    if (Children == null)
                    {
                        result += ": ";
                    }
                }
                else
                {
                    result = null;
                }
                return result;
            }
        }

        public string ValueHeader => Value;
    }
}
