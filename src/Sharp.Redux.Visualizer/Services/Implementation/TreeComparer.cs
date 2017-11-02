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
                return FromBranchModified(current, next);
            }
            if (current is StateObjectTreeItem || current is DictionaryObjectTreeItem)
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

        public static DifferenceItem FromList(ListObjectTreeItem current, ListObjectTreeItem next)
        {
            List<DifferenceItem> modifications = new List<DifferenceItem>();
            var nextChildren = new List<ObjectTreeItem>(next.Children);
            foreach (var item in current.Children)
            {
                var itemKeyed = item as IKeyedItem;

                ObjectTreeItem nextItem;
                if (item != null)
                {
                    nextItem = next.Children.SingleOrDefault(i => itemKeyed.IsKeyEqualTo((IKeyedItem)i));
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
                        modifications.Add(FromBranchModified(item, nextItem));
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

        public static DifferenceItem FromNamedProperties(NodeObjectTreeItem current, NodeObjectTreeItem next)
        {
            List<DifferenceItem> modifications = new List<DifferenceItem>();
            var nextChildren = new List<ObjectTreeItem>(next.Children);
            foreach (var item in current.Children)
            {
                var nextItem = next.Children.SingleOrDefault(i => string.Equals(i.PropertyName, item.PropertyName));
                if (nextItem == null)
                {
                    modifications.Add(new DifferenceItem(item, null, DiffType.Removed));
                }
                else
                {
                    if (!ReferenceEquals(item, nextItem))
                    {
                        modifications.Add(FromBranchModified(item, nextItem));
                    }
                    nextChildren.Remove(nextItem);
                }
            }
            foreach (var nextItem in nextChildren)
            {
                modifications.Add(FromBranchAdded(nextItem));
            }
            return new DifferenceItemContainer(modifications.ToArray(), current, next, DiffType.Modified);
        }

        public static DifferenceItem FromBranchModified(ObjectTreeItem current, ObjectTreeItem next)
        {
            if (next is NodeObjectTreeItem node)
            {
                var children = node.Children.Select(n => FromBranchAdded(n)).ToArray();
                return new DifferenceItemContainer(children, current, next, DiffType.Modified);
            }
            else
            {
                return new DifferenceItem(current, next, DiffType.Modified);
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
    }
}
