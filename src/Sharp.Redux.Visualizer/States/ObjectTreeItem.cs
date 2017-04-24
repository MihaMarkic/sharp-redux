namespace Sharp.Redux.Visualizer.States
{
    public class ObjectTreeItem
    {
        public string PropertyName { get; }
        public string TypeName { get; }
        public string Value { get; }
        public ObjectTreeItem[] Children { get; }

        public  ObjectTreeItem(string propertyName, string typeName, string value, ObjectTreeItem[] children)
        {
            PropertyName = propertyName;
            TypeName = typeName;
            Value = value;
            Children = children;
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
    }
}
