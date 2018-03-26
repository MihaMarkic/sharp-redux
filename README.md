# sharp-redux

|                SharpRedux                |                Visualizer                |              Visualizer.Wpf              |
| :--------------------------------------: | :--------------------------------------: | :--------------------------------------: |
| [![NuGet](https://img.shields.io/nuget/v/Righthand.SharpRedux.svg)](https://www.nuget.org/packages/Righthand.SharpRedux) | [![NuGet](https://img.shields.io/nuget/v/Righthand.SharpRedux.Visualizer.svg)](https://www.nuget.org/packages/Righthand.SharpRedux.Visualizer) | [![NuGet](https://img.shields.io/nuget/v/Righthand.SharpRedux.Visualizer.Wpf.svg)](https://www.nuget.org/packages/Righthand.SharpRedux.Visualizer.Wpf) |

Experimental .net implementation of Redux philosophy. Initial effort is concentrated on MVVM/WPF scenario.

## Requirements

| Package        | Min. .NET Standard | Min. .NET |
| -------------- | :----------------: | :-------: |
| SharpRedux     |        1.2         |    4.5    |
| Visualizer     |        1.5         |    4.5    |
| Visualizer.WPF |         -          |    4.6    |

## Features

* Parallel reducer at root level.
* Build around immutable state

## Qucikly about redux

This project is inspired by [Redux](https://github.com/reactjs/redux), if you don't know what Redux is, then I warmly recommend visiting [website](https://github.com/reactjs/redux) and read about it, it is much better described than I can.

That said, Redux is all about having a centralized **state**, **actions** dispatched through **dispatcher** and reducers that calculate new state. The state and actions should be immutable and preferably searilizable (in case you need archive, storing state, etc.).

### State

This is a centralized and immutable state where all information is stored and main point of truth. State is usually replaced with new state after every dispatched action. After the state has 'changed' (created a new immutable instance), dispatcher raises *StateChanged* event which holds the action that was used to calculate new state and the new state itself.

### Dispatcher

Dispatcher is responsible for dispatching actions (to reducer), holding current state and raising useful events. It also provides a way to replay certain set of actions, a feature that is provided through visualizers or manually triggered by client. Application can have more than one dispatcher, but only one state and one core reducer[^1] per dispatcher. Dispatcher runs in a background task but will raise events on a thread set by *TaskScheduler* passed as argument during instance creation. Usually the UI TaskScheduler is used when having UI, so all events run in the UI thread.

[^1]: A core reducer can and usually will dispatch action to sub reducers

### Reducer

Reducer is responsible for calculating new state based on dispatched action and current state. Reducers must be **pure**, meaning (definition from [reactjs/redux/docs](https://redux.js.org/docs/basics/Reducers.html) slightly adjusted for .NET):

* Mutate its arguments;
* Perform side effects like API calls and routing transitions;
* Call non-pure functions, e.g. DateTime.Now or Random.Next().
  Basically the next state should depend only on previous state and action.

## The benefits

There are quite some benefits using SharpRedux although it is another layer.

* All state is in one place,
* State is always visible through a visualizer (or other visualization tool). Visualizer shows also differences between the two states,
* History of changes/actions can be persisted,
* Actions can be replayed,
* Powerfull unit testing on reducers,
* Clear flow,
* Easy undo/redo implementation,
* and, I'm sure, plenty of others.

## Samples

There are currently two samples: **GettingStarted** and **Todo** which is a reimplementation of TodoMvc website. GettingStarted is a simple as it gets to start with SharpRedux. Todo is a WPF/MVVM and has basic funcationallity, plus it shows visualizer in action. Both are found in *samples* folder.
There is also **Playground** project but is not a sample, it's rather used to test more advanced scenarios and won't be stable.

Check also [experimentation](https://github.com/MihaMarkic/BlazorWithSharpReduxSample) with [Blazor](https://github.com/aspnet/Blazor).

### Getting started

This is a minimal .NET Core console application showing the core of SharpRedux in action. I'll guide you to create it from scratch.

1. Create new .NET Core console application.
2. Add NuGet reference to Righthand.SharpRedux. At the time of the writting the version 1.0.2 is actual.
3. Create new class RootState.

```csharp
class RootState
{
    public int Counter { get; }
    public RootState(int counter)
    {
        Counter = counter;
    }
}
```

Counter holds the number that the application increases or decreases. RootState is immutable.

4. Create the two actions app will use to increase or decrease the counter.

```csharp
class IncrementAction: ReduxAction
{}
class DecrementAction : ReduxAction
{ }
```

Actions don't have any properties since it is implied that the step is 1. i.e. they could have a 

```csharp
public int Step { get; }
```

property.

5. Create reducer

```csharp
class Reducer : IReduxReducer<RootState>
{
    public Task<RootState> ReduceAsync(RootState state, ReduxAction action, CancellationToken ct)
    {
        switch (action)
        {
            case IncrementAction _:
                return Task.FromResult(new RootState(state.Counter + 1));
            case DecrementAction _:
                return Task.FromResult(new RootState(state.Counter - 1));
            default:
                return Task.FromResult(state);
        }
    }
}
```

Reducer will act based on current state and action that is dispatched. It will calculate new state and return it. If action is unknown then it simply returns the current state.

Since the Reduce method is async, Task.FromResult is used. As an alternative *asycn/await* could be used, but it would create an unnecessary overhead.

6. With all types prepared (actions, state, reducer) we can put it in action. Create a dispatcher instance in Main method:

```csharp
var dispatcher = new ReduxDispatcher<RootState, Reducer>(
    initialState: new RootState(0),
    reducer: new Reducer(),
    notificationScheduler: TaskScheduler.Current
);
```

Dispatcher needs state and reducer type, initial state, reducer instance and the TaskScheduler that runs events on proper thread. Since this is a console applictiona, the later isn't important.

7. Before starting the dispatcher, it's *StateChanged* event has to be implemented, so the app can respond to all state changes.

```csharp
dispatcher.StateChanged += (s, e) => 
    Console.WriteLine($"\nCounter is {e.State.Counter}");
```

Application will merely output the current *RootState.Counter* value to the console. In real application here the UI update happens.

8. Once dispatcher is created and *StateChanged* event is implemented, it can be started.

```csharp
dispatcher.Start();
```

This will put dispatcher in the running mode and will immediately raise *StateChanged* event containing initial state and (special) InitAction. This action is special because it is one of the actions that are implicit to the dispatcher, not dispatched by client.

9. At this point application is ready to do some work. For every state change it has to dispatch an action. It will do this in an infinite loop (until use presses something else than *i* or *d* keys).

```csharp
bool loop = true;
Console.WriteLine("Type i to increment, d to decrement or any other key to exit.");
do
{
    char c = (char)Console.ReadKey().KeyChar;
    switch (c)
    {
        case 'i':
            dispatcher.Dispatch(new IncrementAction());
            break;
        case 'd':
            dispatcher.Dispatch(new DecrementAction());
            break;
        default:
            loop = false;
            break;
    }
} while (loop);
```

When user presses either *i* or *d* the matching action will be dispatched and reducer will calculate next state.

10. Before exiting the application dispatcher has to be disposed.

```csharp
 dispatcher.Dispose();
```

That's it. We have a simple but functional SharpRedux application that shows the basic redux principles.