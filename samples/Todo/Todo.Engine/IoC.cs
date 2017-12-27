using Autofac;
using Sharp.Redux;
using System.Collections.Immutable;
using Todo.Engine.Core;
using Todo.Engine.Models;
using Todo.Engine.Reducers;
using Todo.Engine.States;
using Todo.Engine.ViewModels;

namespace Todo.Engine
{
    public static class IoCRegistrar
    {
        public static IContainer Container { get; private set; }
        public static void Build(ContainerBuilder builder)
        {
            builder.RegisterType<RootReducer>().As<IReduxReducer<RootState>>().SingleInstance();
            // register root dispatcher and initialize state
            builder.Register<ITodoReduxDispatcher>(ctx => new TodoReduxDispatcher(
                initialState: new RootState(
                    allChecked: false,
                    newItemText: "",
                    filter: ItemsFilter.All,
                    items: ImmutableList<TodoItem>.Empty,
                    filteredItems: new TodoItem[0],
                    editedItemId: null,
                    editText: ""
                ), 
                reducer: ctx.Resolve<IReduxReducer<RootState>>())).SingleInstance();
            // register view models
            builder.RegisterType<MainViewModel>();
            // ExternallyOwned ->prevent Autofac from keeping it in memory
            builder.RegisterType<TodoItemViewModel>().ExternallyOwned();
            Container = builder.Build();
        }
    }
}
