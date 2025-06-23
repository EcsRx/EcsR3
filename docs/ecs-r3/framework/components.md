# Components

Components are the data containers in the ECS world and should only contain some properties to contain data, there should be no logic in components, if there is logic then you are probably a bad man and need to step away from the computer.

## Using components

You will need to make your own implementations of `IComponent` which encapsulate the data your components need to expose to the systems. It is fairly simple really just implement `IComponent` and it just does its stuff.

> You can implement `IComponent` as a `class` or a `struct` and both will work fine, however if you use `struct` types you will need to remember that unless you use the `*ByRef` methods you will not be able to update any of the data within the component.

## `IDisposable` support
If you implement `IDisposable` on your `IComponent` implementation then the framework will ensure when the `Component` is removed from an `Entity` it also gets disposed.

## Some Advice

If you are not new to ECS in general then you can probably ignore most of this, it is mainly for people newer to the paradigm.

### Try to keep your components small and concise
The more you attempt to represent within a component the more expensive it is to pass around and the harder it is to break apart associated behaviours that utilise it.

Quite often you may want to break up larger components into multiple smaller ones (where it makes sense), it also makes it easier to re-use those components on other things without them having redundant fields.

> For example in an RPG you may have a `Character` model which represents `Health`, `Damage`, `Experience`, `Inventory` etc but in ECS world you would probably want each of those to be their own components so then anything with a `HealthComponent` can have its health reduced without knowing about any other facets of a `Character`, deconstructing larger models into their behavioural parts is a big part of ECS design.

### Dont be afraid to make POCOs to encapsulate related data

You do not need to have all fields exposed directly on the `Component` you can wrap related fields into your own objects, for example if you have a `Character` with `Stats` you could do something like:

```csharp
public class StatsComponent : IComponent
{
    public int Health {get;set;}
    public int MaxHealth {get;set;}
    public int Magic {get;set;}
    public int MaxMagic {get;set;}
}
```

However what if you want to store that data or use it outside of the ECS world, like in a UI somewhere, you need to start bleeding concerns, whereas if you did something like:

```csharp
public class Stats
{
    public int Health {get;set;}
    public int MaxHealth {get;set;}
    public int Magic {get;set;}
    public int MaxMagic {get;set;}
}

public class StatsComponent : IComponent
{
    public Stats Stats {get;set;} = new Stats();
}
```

You can pass the `Stats` object around to anything that needs to without worrying about it having to know about the ECS paradigm etc.

> This can also be a REALLY useful approach for when want to deal with saving data, you should ideally save POCOs not ECS objects, so if you make all your important game data their own objects its far easier to just extract them all from their component housings and do whatever you want with them.

### Not EVERYTHING needs to be a component/ecs

In purist ECS frameworks everything very much needs to be a component, for example if you complete a level an `Entity` should be given a `LevelCompleted` component which a system would pick up and do something with, then the `Entity` would be destroyed.

Here though we have `Events` as first class citizens, you can just raise an event to represent a change without faffing with components/entities, you can also take this further and have logic run outside of ECS entirely be it within or outside of `Components`.