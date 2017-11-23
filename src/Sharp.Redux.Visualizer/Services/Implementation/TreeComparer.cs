using Sharp.Redux.Visualizer.Core;
using Sharp.Redux.Visualizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sharp.Redux.Visualizer.Services.Implementation
{
    public static class TreeComparer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <remarks>
        /// Root has to be the same type, no property.
        /// Also it can't be a primitive
        /// </remarks>
        public static DifferenceItem CreateDifferenceTree(ObjectTreeItem current, ObjectTreeItem next)
        {
            if (ReferenceEquals(current, null))
            {
                if (!ReferenceEquals(next, null))
                {
                    return FromBranchAdded(next);
                }
                else
                {
                    // when both are null
                    return null;
                }
            }
            if (!ReferenceEquals(current, null))
            {
                if (ReferenceEquals(next, null))
                {
                    return new DifferenceItem(current, null, DiffType.Removed);
                }
            }
            if (ReferenceEquals(current.Source, next.Source))
            {
                return null;
            }
            if (current.GetType() != next.GetType())
            {
                var removed = FromBranchRemoved(current);
                var added = FromBranchAdded(next);
                return new DifferenceItemContainer(new[] { removed, added }, current, next, DiffType.Modified);
            }
            // from this point both states are same type
            else if (current is StateObjectTreeItem || current is DictionaryObjectTreeItem)
            {
                return FromNamedProperties((NodeObjectTreeItem)current, (NodeObjectTreeItem)next);
            }
            else if (current is ListObjectTreeItem)
            {
                return FromList((ListObjectTreeItem)current, (ListObjectTreeItem)next);
            }
            else if (current is PrimitiveObjectTreeItem)
            {
                return FromPrimitive((PrimitiveObjectTreeItem)current, (PrimitiveObjectTreeItem)next);
            }
            throw new Exception($"Unknown source type {current.GetType().Name}");
        }

        /// <summary>
        /// Compares two primitives.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <remarks>Cases when at least one is null, or they are different types are handled in <see cref="CreateDifferenceTree"/>.</remarks>
        public static DifferenceItem FromPrimitive(PrimitiveObjectTreeItem current, PrimitiveObjectTreeItem next)
        {
            // first check is currently redundant, but it might come handy if StateFormatter cached objects
            if (!ReferenceEquals(current, next) &&
                !ReferenceEquals(current.Source, next.Source) && !Equals(current.Value, next.Value))
            {
                return new DifferenceItem(current, next, DiffType.Modified);
            }
            return null;
        }
        public static bool AreKeysEqual(ObjectTreeItem left, ObjectTreeItem right)
        {
            var leftObjectSource = left.Source;
            var rightObjectSource = right.Source;
            if (leftObjectSource is null || rightObjectSource is null || leftObjectSource.GetType() != rightObjectSource.GetType())
            {
                return false;
            }
            return Equals(left, right);
        }

        public static DifferenceItemContainer FromList(ListObjectTreeItem current, ListObjectTreeItem next)
        {
            List<DifferenceItem> modifications = new List<DifferenceItem>();
            var nextChildren = new List<ObjectTreeItem>(next.Children);
            foreach (var item in current.Children)
            {
                ObjectTreeItem nextItem;
                if (item != null)
                {
                    nextItem = next.Children.SingleOrDefault(i => AreKeysEqual(item, i));
                }
                else
                {
                    nextItem = next.Children.SingleOrDefault(i => Equals(i, item));
                }
                if (nextItem == null)
                {
                    modifications.Add(new DifferenceItem(item, null, DiffType.Removed));
                }
                else
                {
                    if (!ReferenceEquals(item, nextItem))
                    {
                        modifications.Add(CreateDifferenceTree(item, nextItem));
                    }
                    nextChildren.Remove(nextItem);
                }
            }
            foreach (var nextItem in nextChildren)
            {
                modifications.Add(FromBranchAdded(nextItem));
            }
            if (modifications.Count > 0)
            {
                return new DifferenceItemContainer(modifications.ToArray(), current, next, DiffType.Modified);
            }
            return null;
        }

        public static DifferenceItemContainer FromNamedProperties(NodeObjectTreeItem current, NodeObjectTreeItem next)
        {
            List<DifferenceItem> modifications = new List<DifferenceItem>();
            var nextChildren = next.Children.ToDictionary(c => c.PropertyName, c => c);
            foreach (var item in current.Children)
            {
                if (nextChildren.TryGetValue(item.PropertyName, out var nextItem))
                {
                    if (!ReferenceEquals(item, nextItem))
                    {
                        modifications.AddIfNotNull(CreateDifferenceTree(item, nextItem));
                    }
                    nextChildren.Remove(item.PropertyName);
                }
                else
                {
                    modifications.Add(FromBranchRemoved(item));
                }
            }
            foreach (var nextItem in nextChildren.Values)
            {
                modifications.AddIfNotNull(FromBranchAdded(nextItem));
            }
            if (modifications.Count > 0)
            {
                return new DifferenceItemContainer(modifications.ToArray(), current, next, DiffType.None);
            }
            return null;
        }

        public static DifferenceItem FromBranchModified(ObjectTreeItem current, ObjectTreeItem next)
        {
            if (next is NodeObjectTreeItem node)
            {
                var children = node.Children.Select(n => FromBranchAdded(n)).ToArray();
                return new DifferenceItemContainer(children, current, next, DiffType.None);
                //return 
            }
            else
            {
                return new DifferenceItem(current, next, DiffType.None);
            }
        }
        public static DifferenceItem FromBranchAdded(ObjectTreeItem source)
        {
            if (source is NodeObjectTreeItem node)
            {
                var children = node.Children.Select(n => FromBranchAdded(n)).ToArray();
                return new DifferenceItemContainer(children, null, source, DiffType.Added);
            }
            else
            {
                return new DifferenceItem(null, source, DiffType.Added);
            }
        }
        public static DifferenceItem FromBranchRemoved(ObjectTreeItem source)
        {
            if (source is NodeObjectTreeItem node)
            {
                var children = node.Children.Select(n => FromBranchRemoved(n)).ToArray();
                return new DifferenceItemContainer(children, source, null, DiffType.Removed);
            }
            else
            {
                return new DifferenceItem(source, null, DiffType.Removed);
            }
        }
    }
}
