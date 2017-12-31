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

## Shortly about redux

This project is inspired by [Redux](https://github.com/reactjs/redux), if you don't know what Redux is, then I warmly recommend visiting that website and read about it.

That said, Redux is all about having a centralized **state**, **actions** dispatched through **dispatcher** and reducers that mutate the state based on actions. The state itself should be immutable (when I mention *state mutation* I really mean creating a new instance of immutable state with given modifications).

### State

This is a centralized and immutable state where all information is stored and main point of truth. State might be mutated after every dispatched action. After the state has been 'mutated' (created a new immutable instance), dispatcher raises *StateChanged* event which holds the action that mutated it and the new state.

### Dispatcher

Dispatcher is responsible for dispatching actions (to reducer), holding current state and raising useful events. It also provides a way to replay certain set of actions, a feature that is provided through visualizers or manually triggered by client. Application can have more than one dispatcher, but only one state and one core reducer[^1] per dispatcher. Dispatcher runs in a background task but will raise events on a thread set by *TaskScheduler* passed as argument during instance creation.

[^1]: A core reducer can and usually will dispatch action to sub reducers

### Reducer

Reducer is responsible for mutating state 

## Samples

There are currently two samples: **GettingStarted** and **Todo** which is a reimplementation of TodoMvc website. GettingStarted is a simple as it gets to start with SharpRedux. Todo is a WPF/MVVM and has basic funcationallity, plus it shows visualizer in action. Both are found in *samples* folder.
There is also **Playground** project but is not a sample, it's rather used to test more advanced scenarios and won't be stable.

### Getting started

This is a .net core console application.

