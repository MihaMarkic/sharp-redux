using Sharp.Redux.Visualizer.Core;
using Sharp.Redux.Visualizer.Services.Implementation;
using System;
using System.Collections.Generic;

namespace Sharp.Redux.Visualizer.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Don't auto recreate immutable type.</remarks>
    public class StateObjectTreeItem : ObjectTreeItem, INodeObjectTreeItem
    {
        public ObjectTreeItem[] Children { get; }
        public StateObjectTreeItem(ObjectTreeItem parent, string propertyName, StateObjectData source, int depth) : 
            base(parent, propertyName, source)
        {
            var properties = new List<ObjectTreeItem>(source.Properties.Count);
            foreach (var item in source.Properties)
            {
                properties.Add(StateFormatter.ToTreeHierarchy(this, item.Value, depth + 1, item.Key));
            }
            Children = properties.ToArray();
        }
        
        public new StateObjectData Source => (StateObjectData)base.Source;
        public object Key => Source.Key;
        public bool HasKey => Source.HasKey;
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
                    result = Identifier;
                }
                return result;
            }
        }
        /// <summary>
        /// StateObjectTreeItem is the only type that can have a key as an identifier.
        /// However <see cref="IdentifierType.Index"/> has precedence over key.
        /// </summary>
        public override IdentifierType IdentifierType
        {
            get
            {
                var id = base.IdentifierType;
                if (HasKey)
                {
                    id |= IdentifierType.Key;
                }
                return id;
            }
        }
        public override string KeyIdentifier => Convert.ToString(Key);
    }
}
