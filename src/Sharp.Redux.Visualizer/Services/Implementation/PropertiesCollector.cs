using Sharp.Redux.Visualizer.Core;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux.Visualizer.Services.Implementation
{
    public static class PropertiesCollector
    {
        private static readonly ConcurrentDictionary<Type, TypeMetadata> metadata = new ConcurrentDictionary<Type, TypeMetadata>();
        /// <summary>
        /// Contains references to all ObjectData instances created.
        /// </summary>
        private static readonly ConcurrentDictionary<object, ObjectData> cache = new ConcurrentDictionary<object, ObjectData>();
        public async static Task<ObjectData> CollectAsync(object source, CancellationToken ct)
        {
            if (ReferenceEquals(source, null))
            {
                return new PrimitiveData("", null);
            }
            var typeMetadata = GetTypeMetadata(source);
            string typeName = source.GetType().FullName;
            if (typeMetadata.IsState)
            {
                return await CreateStateObjectAsync(source, typeMetadata, typeName, ct);
            }
            else if (source is IDictionary dictionary)
            {
                return await CreateDictionaryAsync(typeName, dictionary, ct);
            }
            else if (source is IEnumerable enumerable && !(source is string))
            {
                return await CreateListDataAsync(typeName, enumerable, ct);
            }
            else if (typeMetadata.IsPrimitive)
            {
                return CreatePrimitiveData(source, typeName);
            }
            else
            {
                return await CreateStateObjectAsync(source, typeMetadata, typeName, ct);
            }
        }

        private static PrimitiveData CreatePrimitiveData(object source, string typeName)
        {
            return CreateObjectData(source,
                        src =>
                            new PrimitiveData(
                                typeName,
                                source
                        ));
        }

        private static Task<ListData> CreateListDataAsync(string typeName, IEnumerable enumerable, CancellationToken ct)
        {
            return CreateObjectDataAsync(enumerable,
                                async (src, ctoken) =>
                                {
                                    return new ListData(
                                        typeName,
                                        list: await CollectListValuesAsync(src, ctoken)
                                    );
                                }, ct);
        }
        public static async ValueTask<ObjectData[]> CollectListValuesAsync(IEnumerable enumerable, CancellationToken ct)
        {
            var list = new List<ObjectData>();
            foreach (var item in enumerable)
            {
                list.Add(await CollectAsync(item, ct));
            }
            return list.ToArray();
        }

        private static Task<DictionaryData> CreateDictionaryAsync(string typeName, IDictionary dictionary, CancellationToken ct)
        {
            return CreateObjectDataAsync(dictionary,
                                async (src, ctoken) =>
                                {
                                    return new DictionaryData(
                                        typeName,
                                        dictionary: await CollectDictionaryValuesAsync(src, ctoken)
                                    );
                                }, ct
                            );
        }
        public static async ValueTask<ImmutableDictionary<object, ObjectData>> CollectDictionaryValuesAsync(IDictionary dictionary, CancellationToken ct)
        {
            var data = ImmutableDictionary.CreateBuilder<object, ObjectData>();
            foreach (DictionaryEntry pair in dictionary)
            {
                data.Add(pair.Key, await CollectAsync(pair.Value, ct).ConfigureAwait(false));
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
        public static Task<StateObjectData> CreateStateObjectAsync(object source, TypeMetadata typeMetadata, string typeName, CancellationToken ct)
        {
            return CreateObjectDataAsync(source,
                        async (src, ctoken) =>
                        {
                            return new StateObjectData(
                                typeName,
                                properties: await CollectPropertiesAsync(typeMetadata.Properties, source, ctoken).ConfigureAwait(false)
                            );
                        }, ct
                    );
        }

        public static async Task<TResult> CreateObjectDataAsync<TSource, TResult>(TSource source, Func<TSource, CancellationToken, Task<TResult>> factoryAsync, CancellationToken ct)
           where TResult: ObjectData
        {
            if (cache.TryGetValue(source, out ObjectData cached))
            {
                return (TResult)cached;
            }
            return await factoryAsync(source, ct);
        }
        public static  TResult CreateObjectData<TSource, TResult>(TSource source, Func<TSource, TResult> factory)
           where TResult : ObjectData
        {
            if (cache.TryGetValue(source, out ObjectData cached))
            {
                return (TResult)cached;
            }
            return factory(source);
        }

        public static Task<ImmutableDictionary<string, ObjectData>> CollectPropertiesAsync(PropertyInfo[] properties, object source, CancellationToken ct)
        {
            return Task.Run(async () =>
            {
                var builder = ImmutableDictionary.CreateBuilder<string, ObjectData>();
                var query = from p in properties
                            let propertyValue = p.GetValue(source)
                            select new
                            {
                                PropertyName = p.Name,
                                ValueTask = CollectAsync(propertyValue, ct)
                            };
                await Task.WhenAll(query.Select(q => q.ValueTask));
                foreach (var pair in query)
                {
                    builder.Add(pair.PropertyName, pair.ValueTask.Result);
                }
                return builder.ToImmutableDictionary();
            });
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
