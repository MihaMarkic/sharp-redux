using Sharp.Redux.Visualizer.Core;
using System;
using System.Collections;
using System.Collections.Concurrent;
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
            else
            {
                return CreatePrimitiveData(source, typeName);
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
                                    var list = ImmutableArray.CreateBuilder<ObjectData>();
                                    foreach (var item in src)
                                    {
                                        list.Add(await CollectAsync(item, ctoken));
                                    }
                                    return new ListData(
                            typeName,
                            list.ToArray()
                        );
                                }, ct);
        }

        private static Task<DictionaryData> CreateDictionaryAsync(string typeName, IDictionary dictionary, CancellationToken ct)
        {
            return CreateObjectDataAsync(dictionary,
                                async (src, ctoken) =>
                                {
                                    var data = ImmutableDictionary.CreateBuilder<object, ObjectData>();
                                    foreach (DictionaryEntry pair in dictionary)
                                    {
                                        data.Add(pair.Key, await CollectAsync(pair.Value, ctoken).ConfigureAwait(false));
                                    }
                                    return new DictionaryData(
                                        typeName,
                                        data.ToImmutableDictionary()
                                    );
                                }, ct
                            );
        }

        public static Task<StateObjectData> CreateStateObjectAsync(object source, TypeMetadata typeMetadata, string typeName, CancellationToken ct)
        {
            return CreateObjectDataAsync(source,
                        async (src, ctoken) =>
                            new StateObjectData(
                                typeName,
                                await CollectPropertiesAsync(typeMetadata.Properties, source, ctoken).ConfigureAwait(false)
                        ), ct
                    );
        }

        public static async Task<TResult> CreateObjectDataAsync<TSource, TResult>(TSource source, Func<TSource, CancellationToken, Task<TResult>> factoryAsync, CancellationToken ct)
           where TResult: ObjectData
        {
            ObjectData cached;
            if (cache.TryGetValue(source, out cached))
            {
                return (TResult)cached;
            }
            return await factoryAsync(source, ct);
        }
        public static  TResult CreateObjectData<TSource, TResult>(TSource source, Func<TSource, TResult> factory)
           where TResult : ObjectData
        {
            ObjectData cached;
            if (cache.TryGetValue(source, out cached))
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
            bool isState = typeInfo.CustomAttributes.Any(cad => cad.AttributeType == typeof(ReduxStateAttribute)) || source is ReduxAction;
            if (!isState)
            {
                result = new TypeMetadata(false, null);
            }
            else
            {
                var properties = typeInfo.DeclaredProperties.ToArray();
                result = new TypeMetadata(true, properties);
            }
            metadata.TryAdd(type, result);
            return result;
        }

        public static string DataToString(ObjectData source)
        {
            return source.TypeName;
        }
    }
}
