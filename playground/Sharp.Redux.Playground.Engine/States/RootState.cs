using Righthand.Immutable;

namespace Sharp.Redux.Playground.Engine.States
{
    [ReduxState]
    public class RootState
    {
        public NavigationState Navigation { get; }
        public FirstPageState FirstPage { get; }
        public DictionaryPageState DictionaryPage { get; }

        public RootState(NavigationState navigation, FirstPageState firstPage, DictionaryPageState dictionaryPage)
        {
            Navigation = navigation;
            FirstPage = firstPage;
            DictionaryPage = dictionaryPage;
        }

        public RootState Clone(Param<NavigationState>? navigation = null, Param<FirstPageState>? firstPage = null, Param<DictionaryPageState>? dictionaryPage = null)
        {
            return new RootState(navigation.HasValue ? navigation.Value.Value : Navigation,
firstPage.HasValue ? firstPage.Value.Value : FirstPage,
dictionaryPage.HasValue ? dictionaryPage.Value.Value : DictionaryPage);
        }
    }
}
