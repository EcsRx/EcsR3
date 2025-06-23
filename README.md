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

If you haven't used an ECS system before there are 3 main parts:
- `Entity` - Has components applied to them and acts as a handle to access them
- `Components` - Stores actual state/data and is used in driving logic in `Systems`
- `Systems` - Runs logic against entities and their components

> Its also worth noting we have the notion of `Groups` which are a contractual object which describes what `Components` a `System` needs to operate, see more below or in docs.

Here are some quick examples of the above:

### Creating Entities via an Application

This is a super barebones example of creating an application and adding an `Entity` to the `EntityCollection`, check the docs for more information.

```csharp
public class HelloWorldExampleApplication : EcsR3Application // Sets up a basic entry point for your application
{
    public override IDependencyRegistry DependencyRegistry { get; } = new MicrosoftDependencyRegistry(); // Uses the default Microsoft dependency injection provider
    
    protected override void ApplicationStarted()
    {
        var entityId = EntityCollection.Create(); // The Entity Collection allows you to create/access entities

        var canTalkComponent = new CanTalkComponent { Message = "Hello world" }; // We make a new component
        EntityComponentAccessor.AddComponent(entityId, canTalkComponent); // We apply the component to the entity
    }
}
```

> As mentioned above and in the docs you can ignore the `Infrastructure` part of the framework if you do not want DI/Plugins etc, but in 99% of cases you probably do.

### Components

```csharp
public class CanTalkComponent : IComponent // Give it the interface so its treated like a component
{
    public string Message { get; set; } // Expose any data we want to access
}
```

Your components should contain state which the `Systems` access via the `Entities`, you can apply as many `Components` to an `Entity` as you like so sometimes its better to break things down into several components vs one large component.

> For example in an RPG you may have a `Character` model which represents `Health`, `Damage`, `Experience`, `Inventory` etc but in ECS world you would probably want each of those to be their own components so then anything with a `HealthComponent` can have its health reduced without knowing about any other facets of a `Character`, deconstructing larger models into their behavioural parts is a big part of ECS design.

```csharp
public class HealthComponent : IComponent
{
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }
}
```

You can optionally implement `IDisposable` if you want to dispose stuff like so:

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

> You can also make your components as `struct` rather than `class` which can provide some performance benefits, but you need to then explicitly request the 
> `ref` of a component if you want to alter its state, whereas the class components are inherently reference types which makes things a bit simpler.

### Systems

Systems are basically logic executors, but they only execute logic against `Entities` within a given `Group`, so if you want to check for an `Entity` with health dying, then you would only want `Entities` with the `HealthComponent` like this:

```csharp
public class CheckForDeathSystem : IReactToEntitySystem
{
    public IGroup TargetGroup => new Group(typeof(HealthComponent)); // Get any entities with health component

    public Observable<Entity> ReactToEntity(IEntityComponentAccessor entityComponentAccessor, Entity entity) // Explain when you want to execute
    {
        var healthComponent = entityComponentAccessor.GetComponent<HealthComponent>(entity);
        return healthComponent.CurrentHealth.Where(x => x <= 0).Select(x => entity);
    }
    
    public void Process(IEntityComponentAccessor entityComponentAccessor, IEntity entity) // Execute against entities whenever the above reaction occurs
    {
        // We know the health is <= 0 because of the predicate in the ReactToEntity contract
        entityComponentAccessor.RemoveComponent<HealthComponent>(entity);
        entityComponentAccessor.AddComponent<IsDeadComponent>(entity);
    }
}
``` 

There are MANY different types of systems provided out the box, each have their own scenarios they are catering for, in almost all scenarios you will probably want a `BatchedSystem` as they are the fastest and provide you the components you need directly, like so:

```csharp
// The system automatically sets up the group based on the component generics provided
public class BatchedExampleSystem : BatchedSystem<SomeComponentA, SomeComponentB>
{
    // Execute every update (see R3 docs around setting up Time/Frame providers)
    protected override Observable<Unit> ReactWhen()
    { return Observable.EveryUpdate(); }
    
    protected override void Process(Entity entity, SomeComponentA componentA, SomeComponentB componentB)
    {
        // Do something with the components or entity
    }    
}
```

This is the most *typical* ECS style system where it allows you to batch process all entities of the same type, it's also the most performant out the box system, but in some situations you may have some other scenarios, so here are some other system examples:

```csharp
// IBasicSystem just allows arbitrary execution every update, its agnostic of the ECS paradigm
public class SayHelloSystem : IBasicSystem
{
    // Triggered every time the IUpdateScheduler ticks (default 60 fps)
    public void Execute(ElapsedTime elapsedTime)
    {
        Console.WriteLine($"System says hello @ {elapsedTime.TotalTime.ToString()}");
    }
}

// If you wanted the same but with entities provided then you could use the IBasicEntitySystem
public class SayHelloEntitySystem : IBasicEntitySystem
{
    public IGroup Group { get; } = new Group(typeof(CanTalkComponent));

    public void Process(IEntityComponentAccessor entityComponentAccessor, Entity entity, ElapsedTime elapsedTime)
    {
        var canTalkComponent = entityComponentAccessor.GetComponent<CanTalkComponent>(entity);
        Console.WriteLine($"Entity [{entity.Id}] says [{canTalkComponent.Message}] @ {elapsedTime.TotalTime.ToString()}")
    }
}
```

```csharp
// You may want to react to events agnostic of the ECS paradigm
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
// Maybe you want to just do things when a system is started or stopped, or even make your own System types based off this (Like BatchedSystems)
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

> You can mix and match interface systems on a single class, there are also many other kinds of systems built in and available via plugins, so check docs for more information on that.

## Quick Architecture Overview

The architecture is layered so you can use the core parts without needing the additional layers if you want to keep things bare bones.

### SystemsR3

This creates a basic `ISystem` convention with an `ISystemExecutor` and `IConventionalSystemHandler` implementations to provide basic systems interfaces (As shown in quick start above).

While this can be used alone for basic systems you can build your own conventions on top of here, such as `EcsR3` which adds an ECS paradigm on top of SystemsRx.

### EcsR3

This is layered on top of **SystemsR3** and adds the ECS paradigm to the framework as well as adding a couple of systems specifically for entity handling. This also contains an **EcsR3.Infrastructure** layer which builds off the **SystemsR3.Infrastructure** layer to provide additional ECS related paradigms and modules.

## Docs / Help

There is a book available which covers the main parts which can be found here:

[![Documentation][gitbook-image]][gitbook-url]

> This is basically just the [docs folder](docs) in a fancy viewer

We also have a discord server where you can ask for help or discuss the framework further:

[![Join Discord Chat][discord-image]][discord-url]

## Community Plugins/Extensions

This can all be found within the [docs here](./docs/others/third-party-content.md)

## Thanks

Thanks to Jetbrains for providing free liceneses via their [Open Source Support program](https://jb.gg/OpenSourceSupport)

[![Jetbrains][jetbrains-image]][jetbrains-url]

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
[jetbrains-image]: https://resources.jetbrains.com/storage/products/company/brand/logos/jetbrains.svg
[jetbrains-url]: https://jb.gg/OpenSourceSupport
