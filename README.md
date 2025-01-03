# EcsR3

EcsR3 is a reactive take on the common ECS pattern with a well separated design using R3 and adhering to IoC and other sensible design patterns.

[![Build Status][build-status-image]][build-status-url]
[![Code Quality Status][codacy-image]][codacy-url]
[![License][license-image]][license-url]
[![Nuget Version][nuget-image]][nuget-url]
[![Join Discord Chat][discord-image]][discord-url]
[![Documentation][gitbook-image]][gitbook-url]

> This is basically EcsRx but natively using R3 rather than Rx

## Features

- Simple ECS interfaces and implementations to use/extend
- Fully reactive architecture
- Favours composition over inheritance
- Adheres to inversion of control
- Lightweight codebase
- Built in support for events (raise your own and react to them)
- Built in Dependency Injection abstraction layer
- Built in support for plugins (wrap up your own components/systems/events and share them with others)

## Quick Start

It is advised to look at the [setup docs](./docs/ecs-r3/introduction/setup.md), this covers the 2 avenues to setup the application using it without the helper libraries, or with the helper libraries which offer you dependency injection and other benefits.

However here are some quick code examples:
### Simple components

```csharp
public class HealthComponent : IComponent
{
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }
}
```

You implement the `IComponent` interface which marks the class as a component, and you can optionally implement `IDisposable` if you want to dispose stuff like so:

```csharp
public class HealthComponent : IComponent, IDisposable
{
    public ReactiveProperty<int> CurrentHealth { get; set; }
    public int MaxHealth { get; set; }
    
    public HealthComponent() 
    { CurrentHealth = new ReactiveProperty<int>(); }
    
    public void Dispose() 
    { CurrentHealth.Dispose; }
}
```

Any component which is marked with `IDisposable` will be auto disposed of by entities.

### System examples

```csharp
public class CheckForDeathSystem : IReactToEntitySystem
{
    public IGroup TargetGroup => new Group(typeof(HealthComponent)); // Get any entities with health component

    public Observable<IEntity> ReactToEntity(IEntity entity) // Explain when you want to execute
    {
        var healthComponent = entity.GetComponent<HealthComponent>();
        return healthComponent.CurrentHealth.Where(x => x <= 0).Select(x => entity);
    }
    
    public void Process(IEntity entity) // Logic run whenever the above reaction occurs
    {
        entity.RemoveComponent<HealthComponent>();
        entity.AddComponent<IsDeadComponent>();
    }
}
``` 

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
> There are many other kinds of systems built in and available via plugins, so check docs for more information on that.

## Architecture

The architecture is layered so you can use the core parts without needing the additional layers if you want to keep things bare bones.

### SystemsR3

This creates a basic `ISystem` convention with an `ISystemExecutor` and `IConventionalSystemHandler` implementations to provide basic systems interfaces (As shown in quick start above).

While this can be used alone for basic systems you can build your own conventions on top of here, such as `EcsR3` which adds an ECS paradigm on top of SystemsRx.

### EcsR3

This is layered on top of **SystemsR3** and adds the ECS paradigm to the framework as well as adding a couple of systems specifically for entity handling. This also contains an **EcsR3.Infrastructure** layer which builds off the **SystemsR3.Infrastructure** layer to provide additional ECS related infrastructure.

## Docs

There is a book available which covers the main parts which can be found here:

[![Documentation][gitbook-image]][gitbook-url]

> This is basically just the [docs folder](docs) in a fancy viewer

## Community Plugins/Extensions

This can all be found within the [docs here](./docs/others/third-party-content.md)

[build-status-image]: https://github.com/EcsRx/EcsR3/actions/workflows/build-and-test.yml/badge.svg?branch=main
[build-status-url]: https://github.com/EcsRx/EcsR3/actions/workflows/build-and-test.yml
[nuget-image]: https://img.shields.io/nuget/v/EcsR3.svg
[nuget-url]: https://www.nuget.org/packages/EcsR3/
[discord-image]: https://img.shields.io/discord/488609938399297536.svg
[discord-url]: https://discord.gg/bS2rnGz
[license-image]: https://img.shields.io/github/license/ecsrx/ecsr3.svg
[license-url]: https://github.com/EcsRx/ecsr3/blob/master/LICENSE
[codacy-image]: https://app.codacy.com/project/badge/Grade/eb08368251df43c98aa55a8cbb8d5577
[codacy-url]: https://www.codacy.com/gh/EcsRx/SystemsRx/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=EcsRx/SystemsRx&amp;utm_campaign=Badge_Grade
[gitbook-image]: https://img.shields.io/static/v1.svg?label=Documentation&message=Read%20Now&color=Green&style=flat
[gitbook-url]: https://ecsrx.gitbook.io/ecsr3/v/main/


[![Build And Test](https://github.com/EcsRx/EcsR3/actions/workflows/build-and-test.yml/badge.svg?branch=main)](https://github.com/EcsRx/EcsR3/actions/workflows/build-and-test.yml)