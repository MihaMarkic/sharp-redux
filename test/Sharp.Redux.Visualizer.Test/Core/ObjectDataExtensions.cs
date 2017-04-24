using Sharp.Redux.Visualizer.Core;

namespace Sharp.Redux.Visualizer.Test.Core
{
    public static class ObjectDataExtensions
    {
        public static bool IsDataString(this ObjectData data, string text)
        {
            return data is PrimitiveData primitive && string.Equals((string)primitive.Value, text, System.StringComparison.Ordinal);
        }
        public static bool IsDataInt32(this ObjectData data, int value)
        {
            return data is PrimitiveData primitive && (int)primitive.Value == value;
        }
    }
}
