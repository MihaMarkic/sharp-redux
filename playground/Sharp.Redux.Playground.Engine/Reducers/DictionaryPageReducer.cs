using Sharp.Redux.Playground.Engine.Actions.DIctionaryPage;
using Sharp.Redux.Playground.Engine.States;

namespace Sharp.Redux.Playground.Engine.Reducers
{
    public static class DictionaryPageReducer
    {
        public static DictionaryPageState Reduce(DictionaryPageState state, ReduxAction action)
        {
            switch (action)
            {
                case AddValueAction addValue:
                    return state.Clone(dictionary: state.Dictionary.Add(addValue.Key, addValue.Value));
                case RemoveValueAction removeValue:
                    return state.Clone(dictionary: state.Dictionary.Remove(removeValue.Key));
                default:
                    return state;
            }
        }
    }
}
