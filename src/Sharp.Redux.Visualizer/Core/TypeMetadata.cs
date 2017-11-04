using Righthand.Immutable;
using System.Reflection;

namespace Sharp.Redux.Visualizer.Core
{
    public class TypeMetadata
    {
        public bool IsState { get; }
        public bool IsPrimitive { get; }
        public PropertyInfo[] Properties { get; }

        public TypeMetadata(bool isState, bool isPrimitive, PropertyInfo[] properties)
        {
            IsState = isState;
            IsPrimitive = isPrimitive;
            Properties = properties;
        }

        public TypeMetadata Clone(Param<bool>? isState = null, Param<bool>? isPrimitive = null, Param<PropertyInfo[]>? properties = null)
        {
            return new TypeMetadata(isState.HasValue ? isState.Value.Value : IsState,
isPrimitive.HasValue ? isPrimitive.Value.Value : IsPrimitive,
properties.HasValue ? properties.Value.Value : Properties);
        }
    }
}
