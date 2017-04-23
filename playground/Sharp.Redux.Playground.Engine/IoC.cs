using Autofac;
using Sharp.Redux.Playground.Engine.Core;
using Sharp.Redux.Playground.Engine.Reducers;
using Sharp.Redux.Playground.Engine.States;
using Sharp.Redux.Playground.Engine.ViewModels;
using System.Reflection;

namespace Sharp.Redux.Playground.Engine
{
    public static class IoCRegistrar
    {
        public static IContainer Container { get; private set; }
        public static void Build(ContainerBuilder builder)
        {
            builder.RegisterType<RootReducer>().As<IReduxReducer<RootState>>().SingleInstance();
            // register root dispatcher and initialize statef
            builder.Register<IPlaygroundReduxDispatcher>(ctx => new PlaygroundReduxDispatcher(
                initialState: new RootState(
                    navigation: new NavigationState(NavigationPage.FirstPage, data: null, isNavigating: true), 
                    firstPage: new FirstPageState(input: null, output: null)), 
                reducer: ctx.Resolve<IReduxReducer<RootState>>())).SingleInstance();
            // register view models
            builder.RegisterAssemblyTypes(typeof(IoCRegistrar).GetTypeInfo().Assembly)
                .Where(t => t.IsAssignableTo<BaseViewModel>());
            Container = builder.Build();
        }
    }
}
