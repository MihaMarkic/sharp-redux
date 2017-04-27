﻿using Sharp.Redux.Visualizer.Core;
using Righthand.Immutable;

namespace Sharp.Redux.Visualizer.Models
{
    public class ListObjectTreeItem : NodeObjectTreeItem
    {
        public ListObjectTreeItem(Sharp.Redux.Visualizer.Models.ObjectTreeItem[] children, string propertyName, string typeName, bool isRoot) : base(children, propertyName, typeName, isRoot)
        {
        }

        public ListObjectTreeItem Clone(Param<Sharp.Redux.Visualizer.Models.ObjectTreeItem[]>? children = null, Param<string>? propertyName = null, Param<string>? typeName = null, Param<bool>? isRoot = null)
        {
            return new ListObjectTreeItem(children.HasValue ? children.Value.Value : Children,
propertyName.HasValue ? propertyName.Value.Value : PropertyName,
typeName.HasValue ? typeName.Value.Value : TypeName,
isRoot.HasValue ? isRoot.Value.Value : IsRoot);
        }
    }
}
