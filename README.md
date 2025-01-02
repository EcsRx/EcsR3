# SystemsRx
A general system execution framework with additional layers for `Application` scenarios with DI abstraction and plugin support.

[![Build Status][build-status-image]][build-status-url]
[![Code Quality Status][codacy-image]][codacy-url]
[![License][license-image]][license-url]
[![Nuget Version][nuget-image]][nuget-url]
[![Join Discord Chat][discord-image]][discord-url]
[![Documentation][gitbook-image]][gitbook-url]

> This originally was a core part of [EcsRx](https://github.com/EcsRx/ecsrx) however now lives as it's own repo, but EcsRx is built on top of this.

## Features

- Simple system interfaces and implementations to use/extend
- Fully reactive architecture
- Favours composition over inheritance
- Adheres to inversion of control
- Lightweight codebase
- Built in support for events (raise your own and react to them)
- Built in Dependency Injection abstraction layer
- Built in support for plugins (wrap up your own components/systems/events and share them with others)

## Quick Start

You have 3 system conventions out of the box, `IBasicSystem`, `IManualSystem` and `IReactToEventSystem`. They can be mixed and matched and when added to the `ISystemExecutor` they will be triggered as expected.

> It is advised to look at the [setup docs](./docs/introduction/setup.md), this covers the 2 avenues to setup the application using it without the helper libraries, or with the helper libraries which offer you dependency injection and other benefits.

### `IBasicSystem`

```csharp
public class SayHelloSystem : IBasicSystem
{
    // Triggered every time the IUpdateScheduler ticks (default 60 fps)
    public void Execute(ElapsedTime elapsedTime)
    {
        Console.WriteLine($"System says hello @ {elapsedTime.TotalTime.ToString()}");
    }
}
```

### `IReactToEventSystem`
```csharp
public class ReactToPlayerDeadEventSystem : IReactToEventSystem<PlayerDeadEvent>
{
    // Triggered when the IEventSystem gets a PlayerDeadEvent
    public void Process(PlayerDeadEvent eventData)
    {
        Console.WriteLine("Oh no the player has died");
    }
}
```

### `IManualSystem`

```csharp
public class StartGameManualSystem : IManualSystem
{
    // Triggered when the system is first registered
    public void StartSystem()
    {
        Console.WriteLine("Game Has Started");
    }
        
    // Triggered when the system is removed/stopped
    public void StopSystem()
    {
        Console.WriteLine("Game Has Ended");
    }
}
```

## Architecture

The architecture is layered so you can use the core parts without needing the additional layers if you want to keep things bare bones.

### MicroRx

This is a bare bones rx implementation, it literally just contains some basic `Subject` and other related rx classes, this is so that we do not have a dependencies on rx.net or unirx in the core.

### SystemsRx

This takes the barebones rx implementation and creates a basic `ISystem` convention with an `ISystemExecutor` and `IConventionalSystemHandler` implementations to provide basic systems interfaces (As shown in quick start above).

While this can be used alone for basic systems you can build your own conventions on top of here, such as `EcsRx` which adds an ECS paradigm on top of SystemsRx.

### Infrastructure

This allows you to make use of the infrastructure layer which provides a DI abstraction layer, Plugin support and some best practice classes.

> The 2 main libraries here are **SystemsRx** and **SystemsRx.Infrastructure**, these only have a dependency on **MicroRx**

## Docs

There is a book available which covers the main parts which can be found here:

[![Documentation][gitbook-image]][gitbook-url]

> This is basically just the [docs folder](docs) in a fancy viewer

## Community Plugins/Extensions

This can all be found within the [docs here](./docs/others/third-party-content.md)

[build-status-image]: https://ci.appveyor.com/api/projects/status/6incybkqawq9qe7u?svg=true
[build-status-url]: https://ci.appveyor.com/project/grofit/systemsrx/branch/main
[nuget-image]: https://img.shields.io/nuget/v/systemsrx.svg
[nuget-url]: https://www.nuget.org/packages/SystemsRx/
[discord-image]: https://img.shields.io/discord/488609938399297536.svg
[discord-url]: https://discord.gg/bS2rnGz
[license-image]: https://img.shields.io/github/license/ecsrx/ecsrx.svg
[license-url]: https://github.com/EcsRx/systemsrx/blob/master/LICENSE
[codacy-image]: https://app.codacy.com/project/badge/Grade/eb08368251df43c98aa55a8cbb8d5577
[codacy-url]: https://www.codacy.com/gh/EcsRx/SystemsRx/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=EcsRx/SystemsRx&amp;utm_campaign=Badge_Grade
[gitbook-image]: https://img.shields.io/static/v1.svg?label=Documentation&message=Read%20Now&color=Green&style=flat
[gitbook-url]: https://ecsrx.gitbook.io/systemsrx/v/main/