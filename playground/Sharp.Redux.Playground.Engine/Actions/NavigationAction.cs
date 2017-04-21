using Sharp.Redux.Playground.Engine.Core;

namespace Sharp.Redux.Playground.Engine.Actions
{
    public class NavigationAction : ReduxAction
    {
        public NavigationPage Page { get; set; }
        public object Data { get; set; }
    }
}
