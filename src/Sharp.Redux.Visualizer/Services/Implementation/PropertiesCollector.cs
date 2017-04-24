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
                return new StateObjectData(
                    typeName,
                    await CollectPropertiesAsync(typeMetadata.Properties, source, ct).ConfigureAwait(false)
                );
            }
            else if (source is IDictionary dictionary)
            {
                var data = ImmutableDictionary.CreateBuilder<object, ObjectData>();
                foreach (DictionaryEntry pair in dictionary)
                {
                    data.Add(pair.Key, await CollectAsync(pair.Value, ct));
                }
                return new DictionaryData(
                    typeName,
                    data.ToImmutableDictionary()
                );
            }
            else if (source is IEnumerable enumerable && !(source is string))
            {
                var list = ImmutableArray.CreateBuilder<ObjectData>();
                foreach (var item in enumerable)
                {
                    list.Add(await CollectAsync(item, ct));
                }
                return new ListData(
                    typeName,
                    list.ToArray()
                );
            }
            else
            {
                return new PrimitiveData(
                    typeName,
                    source
                );
            }
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
