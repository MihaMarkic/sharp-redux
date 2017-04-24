using Righthand.Immutable;
using System;
using System.Linq;
using System.Reflection;

namespace Sharp.Redux.Visualizer.Core
{
    public class TypeMetadata
    {
        public bool IsState { get; }
        public PropertyInfo[] Properties { get; }

        public TypeMetadata(bool isState, PropertyInfo[] properties)
        {
            IsState = isState;
            Properties = properties;
        }

        public TypeMetadata Clone(Param<bool>? isState = null, Param<PropertyInfo[]>? properties = null)
        {
            return new TypeMetadata(isState.HasValue ? isState.Value.Value : IsState,
properties.HasValue ? properties.Value.Value : Properties);
        }
    }
}
