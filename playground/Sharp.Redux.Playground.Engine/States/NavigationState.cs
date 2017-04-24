using Righthand.Immutable;
using Sharp.Redux.Playground.Engine.Core;

namespace Sharp.Redux.Playground.Engine.States
{
    [ReduxState]
    public class NavigationState
    {
        public NavigationPage Page { get; }
        public object Data { get; }
        public bool IsNavigating { get; }

        public NavigationState(NavigationPage page, object data, bool isNavigating)
        {
            Page = page;
            Data = data;
            IsNavigating = isNavigating;
        }

        public NavigationState Clone(Param<NavigationPage>? page = null, Param<object>? data = null, Param<bool>? isNavigating = null)
        {
            return new NavigationState(page.HasValue ? page.Value.Value : Page,
data.HasValue ? data.Value.Value : Data,
isNavigating.HasValue ? isNavigating.Value.Value : IsNavigating);
        }
    }
}
