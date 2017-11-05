using Sharp.Redux.Visualizer.Core;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Sharp.Redux.Visualizer.Services.Implementation
{
    public static class PropertiesCollector
    {
        public struct CollectContext
        {
            public int Depth { get; }
            public ConcurrentDictionary<object, ObjectData> Cache { get; }
            public static CollectContext CreateEmpty() =>
                new CollectContext(0, 
                    new ConcurrentDictionary<object, ObjectData>());
            private CollectContext(int depth, ConcurrentDictionary<object, ObjectData> cache)
            {
                Depth = depth;
                Cache = cache;
            }
            public CollectContext IncreaseDepth()
            {
                return new CollectContext(Depth + 1, Cache);
            }
        }
        public const int MaxDepth = 1000;
        private static readonly ConcurrentDictionary<Type, TypeMetadata> metadata = new ConcurrentDictionary<Type, TypeMetadata>();
        public static ObjectData Collect(object source)
        {
            return Collect(source, CollectContext.CreateEmpty());
        }
        public static ObjectData Collect(object source, CollectContext context)
        {
            if (context.Depth > MaxDepth)
            {
                throw new Exception("Object graph is too deep");
            }
            if (ReferenceEquals(source, null))
            {
                return PrimitiveData.NullValue;
            }
            // prevents recursion
            if (context.Cache.TryGetValue(source, out var data))
            {
                return data;
            }
            var typeMetadata = GetTypeMetadata(source);
            string typeName = source.GetType().FullName;
            if (typeMetadata.IsState)
            {
                return CreateStateObject(source, typeMetadata, typeName, context);
            }
            else if (source is IDictionary dictionary)
            {
                return CreateDictionary(dictionary, typeName, context);
            }
            else if (source is IEnumerable enumerable && !(source is string))
            {
                return CreateListData(enumerable, typeName, context);
            }
            else if (typeMetadata.IsPrimitive)
            {
                return CreatePrimitiveData(source, typeName, context);
            }
            else
            {
                return CreateStateObject(source, typeMetadata, typeName, context.IncreaseDepth());
            }
        }

        private static PrimitiveData CreatePrimitiveData(object source, string typeName, CollectContext context)
        {
            return CreateObjectData(source, context, null, typeName,
                (src, ctx, tm, tn) => new PrimitiveData(tn, source)
            );
        }

        private static ListData CreateListData(IEnumerable source, string typeName, CollectContext context)
        {
            return CreateObjectData(source, context, null, typeName,
                (src, ctx, tm, tn) => new ListData(
                    tn, 
                    list: CollectListValues(src, ctx)
                )
            );
        }
        public static ObjectData[] CollectListValues(IEnumerable enumerable, CollectContext context)
        {
            var list = new List<ObjectData>();
            foreach (var item in enumerable)
            {
                list.Add(Collect(item, context.IncreaseDepth()));
            }
            return list.ToArray();
        }

        private static DictionaryData CreateDictionary(IDictionary source, string typeName, CollectContext context)
        {
            return CreateObjectData(source, context, null, typeName,
                (src, ctx, tm, tn) => new DictionaryData(
                    tn, 
                    dictionary: CollectDictionaryValues(src, ctx)
                )
            );
        }
        public static ImmutableDictionary<object, ObjectData> CollectDictionaryValues(IDictionary dictionary, CollectContext context)
        {
            var data = ImmutableDictionary.CreateBuilder<object, ObjectData>();
            foreach (DictionaryEntry pair in dictionary)
            {
                data.Add(pair.Key, Collect(pair.Value, context.IncreaseDepth()));
            }
            return data.ToImmutableDictionary();
        }

        /// <summary>
        /// Creates a <see cref="StateObjectData"/> object.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="typeMetadata"></param>
        /// <param name="typeName"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        // TODO can parallelize
        public static StateObjectData CreateStateObject(object source, TypeMetadata typeMetadata, string typeName, CollectContext context)
        {
            return CreateObjectData(source, context, typeMetadata, typeName,
                (src, ctx, tm, tn) => new StateObjectData(
                    tn, 
                    properties: CollectProperties(tm.Properties, src, ctx.IncreaseDepth())
                )
            );
        }

        public static TResult CreateObjectData<TSource, TResult>(TSource source, CollectContext context, TypeMetadata typeMetadata, string typeName,
            Func<TSource, CollectContext, TypeMetadata, string, TResult> factory)
           where TResult: ObjectData
        {
            if (context.Cache.TryGetValue(source, out ObjectData cached))
            {
                return (TResult)cached;
            }
            var value = factory(source, context, typeMetadata, typeName);
            context.Cache.TryAdd(source, value);
            return value;
        }
        public static ImmutableDictionary<string, ObjectData> CollectProperties(PropertyInfo[] properties, object source, CollectContext context)
        {
            var builder = ImmutableDictionary.CreateBuilder<string, ObjectData>();
            var query = from p in properties
                        let propertyValue = p.GetValue(source)
                        select new
                        {
                            PropertyName = p.Name,
                            Value = Collect(propertyValue, context.IncreaseDepth())
                        };
            foreach (var pair in query)
            {
                builder.Add(pair.PropertyName, pair.Value);
            }
            return builder.ToImmutableDictionary();
        }

        /// <summary>
        /// Return an array of PropertyInfo[] for the given <paramref name="source"/>.
        /// It sets two additional flags: IsState when source is either <see cref="ReduxStateAttribute"/> decorated or a descendant or <see cref="ReduxAction"/>.
        /// It also stores it in the cache.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <remarks>It might happen that metadata is filled from another Task during the process. That's OK.</remarks>
        public static TypeMetadata GetTypeMetadata(object source)
        {
            TypeMetadata result;
            Type type = source.GetType();
            if (metadata.TryGetValue(type, out result))
            {
                return result;
            }
            TypeInfo typeInfo = type.GetTypeInfo();
            bool isState = source is ReduxAction || typeInfo.CustomAttributes.Any(cad => cad.AttributeType == typeof(ReduxStateAttribute));
            if (isState)
            {
                var properties = typeInfo.GetProperties();
                result = new TypeMetadata(true, false, properties);
            }
            else
            {
                bool isPrimitive = IsPrimitiveType(typeInfo, type);
                PropertyInfo[] properties;
                if (isPrimitive)
                {
                    properties = null;
                }
                else
                {
                    properties = typeInfo.GetProperties();
                }
                result = new TypeMetadata(false, isPrimitive, properties);
            }
            metadata.TryAdd(type, result);
            return result;
        }

        public static bool IsPrimitiveType(TypeInfo typeInfo, Type type) =>
            typeInfo.IsEnum || type == typeof(int) || type == typeof(uint) || type == typeof(byte) || type == typeof(sbyte)
            || type == typeof(short) || type == typeof(ushort) || type == typeof(long) || type == typeof(ulong) || type == typeof(float)
            || type == typeof(double) || type == typeof(char) || type == typeof(bool) || type == typeof(string) || type == typeof(decimal);

        public static string DataToString(ObjectData source)
        {
            return source.TypeName;
        }
    }
}
