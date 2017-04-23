using Righthand.Immutable;
using Sharp.Redux.Playground.Engine.States;

namespace Sharp.Redux.Playground.Engine.States
{
    public class RootState
    {
        public NavigationState Navigation { get; }
        public FirstPageState FirstPage { get; }

        public RootState(NavigationState navigation, FirstPageState firstPage)
        {
            Navigation = navigation;
            FirstPage = firstPage;
        }

        public RootState Clone(Param<NavigationState>? navigation = null, Param<FirstPageState>? firstPage = null)
        {
            return new RootState(navigation.HasValue ? navigation.Value.Value : Navigation,
firstPage.HasValue ? firstPage.Value.Value : FirstPage);
        }
    }
}
