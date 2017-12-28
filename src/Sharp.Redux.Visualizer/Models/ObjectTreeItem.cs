using Sharp.Redux.Visualizer.Core;
using System;
using System.Diagnostics;

namespace Sharp.Redux.Visualizer.Models
{
    [DebuggerDisplay("{PropertyName,nq}")]
    public abstract class ObjectTreeItem
    {
        public ObjectTreeItem Parent { get; }
        public string PropertyName { get; }
        public ObjectData Source { get; }

        public ObjectTreeItem(ObjectTreeItem parent, string propertyName, ObjectData source)
        {
            Parent = parent;
            PropertyName = propertyName;
            Source = source;
        }
        public string TypeName => Source?.TypeName;
        public abstract string ValueHeader { get; }
        public abstract string DescriptionHeader { get; }
        /// <summary>
        /// Returns a descriptive identifier if it has one (when member of a list).
        /// Identifier consist of either a key or list index.
        /// </summary>
        public virtual string Identifier
        {
            get
            {
                var idType = IdentifierType;
                string result = null;
                if (idType.HasFlag(IdentifierType.Index))
                {
                    result = $"[{IndexIdentifier}]";
                }
                if (idType.HasFlag(IdentifierType.Key))
                {
                    if (result != null)
                    {
                        result += " ";
                    }
                    result += KeyIdentifier;
                }
                return result;
            }
        }
        public virtual string KeyIdentifier => null;
        public int? IndexIdentifier
        {
            get
            {
                if (Parent is ListObjectTreeItem list)
                {
                    return Array.IndexOf(list.Children, this);
                }
                return null;
            }
        }
        public virtual IdentifierType IdentifierType
        {
            get
            {
                if (Parent is ListObjectTreeItem)
                {
                    return IdentifierType.Index;
                }
                return IdentifierType.None;
            }
        }
    }
}
