using Sharp.Redux.Visualizer.Core;
using Sharp.Redux.Visualizer.Models;
using Sharp.Redux.Visualizer.States;
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
        public static ObjectTreeItem CreateDifferenceTree(NodeObjectTreeItem current, NodeObjectTreeItem next)
        {
            List<ObjectTreeItem> modifications = new List<ObjectTreeItem>();
            if (current is StateObjectTreeItem || current is DictionaryObjectTreeItem)
            {
                var nextChildren = new List<ObjectTreeItem>(next.Children);
                foreach (var item in current.Children)
                {
                    var nextItem = next.Children.SingleOrDefault(i => string.Equals(i.PropertyName, item.PropertyName));
                    if (nextItem == null)
                    {
                        modifications.Add(Mark(item, DiffType.Removed));
                    }
                    else
                    {
                        //if (false /* different */)
                        //{
                        //    nextChildren.Remove(nextItem);
                        //    modifications.Add()
                        //}
                    }
                }
            }
            if (modifications.Count == 0)
            {
                return null;
            }
            switch (current)
            {
                case StateObjectTreeItem state:
                    return state.Clone(modifications.ToArray());
                case ListObjectTreeItem list:
                    return list.Clone(modifications.ToArray());
                case DictionaryObjectTreeItem dictionary:
                    return dictionary.Clone(modifications.ToArray());
                default:
                    throw new System.Exception($"Invalid current: {current.GetType()} type");
            }
        }

        public static ObjectTreeItem Mark(ObjectTreeItem item, DiffType diffType)
        {
            switch (item)
            {
                //case StateObjectTreeItem state:
                //    return state.Clone(diffType: diffType);
                //case ListObjectTreeItem list:
                //    return list.Clone(diffType: diffType);
                //case DictionaryObjectTreeItem dictionary:
                //    return dictionary.Clone(diffType: diffType);
                //case PrimitiveObjectTreeItem primitive:
                //    return primitive.Clone(diffType: diffType);
                default:
                    throw new System.Exception($"Invalid item: {item.GetType()} type");
            }
        }
    }
}
