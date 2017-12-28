using Righthand.Immutable;
using System.Reflection;

namespace Sharp.Redux.Visualizer.Core
{
    public class TypeMetadata
    {
        public bool IsState { get; }
        public bool IsPrimitive { get; }
        public PropertyInfo[] Properties { get; }
        public PropertyInfo KeyProperty { get; }
        public bool HasKey => KeyProperty != null;

        public TypeMetadata(bool isState, bool isPrimitive, PropertyInfo[] properties, PropertyInfo keyProperty)
        {
            IsState = isState;
            IsPrimitive = isPrimitive;
            Properties = properties;
            KeyProperty = keyProperty;
        }

        public TypeMetadata Clone(Param<bool>? isState = null, Param<bool>? isPrimitive = null, Param<PropertyInfo[]>? properties = null, Param<PropertyInfo>? keyProperty = null)
        {
            return new TypeMetadata(isState.HasValue ? isState.Value.Value : IsState,
isPrimitive.HasValue ? isPrimitive.Value.Value : IsPrimitive,
properties.HasValue ? properties.Value.Value : Properties,
keyProperty.HasValue ? keyProperty.Value.Value : KeyProperty);
        }
    }
}
