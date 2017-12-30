# sharp-redux

| SharpRedux                                                                                                               | Visualizer                                                                                                                                     | Visualizer.Wpf                                                                                                                                         |
| :----------------------------------------------------------------------------------------------------------------------: | :--------------------------------------------------------------------------------------------------------------------------------------------: | :----------------------------------------------------------------------------------------------------------------------------------------------------: |
| [![NuGet](https://img.shields.io/nuget/v/Righthand.SharpRedux.svg)](https://www.nuget.org/packages/Righthand.SharpRedux) | [![NuGet](https://img.shields.io/nuget/v/Righthand.SharpRedux.Visualizer.svg)](https://www.nuget.org/packages/Righthand.SharpRedux.Visualizer) | [![NuGet](https://img.shields.io/nuget/v/Righthand.SharpRedux.Visualizer.Wpf.svg)](https://www.nuget.org/packages/Righthand.SharpRedux.Visualizer.Wpf) |

Experimental .net implementation of Redux philosophy. Initial effort is concentrated on MVVM/WPF scenario.

## Requirements

| Package        | Min. .NET Standard | Min. .NET |
| -------------- | :----------------: | :-------: |
| SharpRedux     | 1.2                | 4.5       |
| Visualizer     | 1.5                | 4.5       |
| Visualizer.WPF | -                  | 4.6       |

## Features

* Parallel reducer at root level.
* Build around immutable state

## Samples

There are currently two samples: **GettingStarted** and **Todo** which is a reimplementation of TodoMvc website. GettingStarted is a simple as it gets to start with SharpRedux. Todo is a WPF/MVVM and has basic funcationallity, plus it shows visualizer in action. Both are found in *samples* folder.
There is also **Playground** project but is not a sample, it's rather used to test more advanced scenarios and won't be stable.

### Getting started

This is a .net core console application.

