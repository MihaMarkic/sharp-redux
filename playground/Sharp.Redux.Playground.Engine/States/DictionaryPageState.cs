using System.Collections.Immutable;
using Righthand.Immutable;

namespace Sharp.Redux.Playground.Engine.States
{
    [ReduxState]
    public class DictionaryPageState
    {
        public ImmutableDictionary<int, string> Dictionary { get; }

        public DictionaryPageState(ImmutableDictionary<int, string> dictionary)
        {
            Dictionary = dictionary;
        }

        public DictionaryPageState Clone(Param<ImmutableDictionary<int, string>>? dictionary = null)
        {
            return new DictionaryPageState(dictionary.HasValue ? dictionary.Value.Value : Dictionary);
        }
    }
}
