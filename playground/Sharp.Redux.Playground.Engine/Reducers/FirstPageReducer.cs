using Sharp.Redux.Playground.Engine.Actions.FirstPage;
using Sharp.Redux.Playground.Engine.States;

namespace Sharp.Redux.Playground.Engine.Reducers
{
    public static class FirstPageReducer
    {
        public static FirstPageState Reduce(FirstPageState state, ReduxAction action)
        {
            switch (action)
            {
                case InputChangedAction inputChanged:
                    return state.Clone(input: inputChanged.Value);
                case ClickMeAction clickMe:
                    return state.Clone(output:$"Yolo: {state.Input}");
                default:
                    return state;
            }
        }
    }
}
