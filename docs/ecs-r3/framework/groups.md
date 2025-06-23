# Groups

To access entities from entity collection we have the notion of groups (implementations of `IGroup`). As every *system* requires a group to access entities they are quite an important piece of the puzzle.

So for example lets say that I wanted the notion of a `Player`, that may actually be expressed as an entity with `IsPlayerControlled`, `IsSceneActor`, `HasStatistics` components. If we were to pretend that `IsPlayerControlled` means that the player controls this entity, the `IsSceneActor` implies that you have a `GameObject` in the scene which does some random stuff, and `HasStatistics` which contains information like Health, Mana, Strength etc. Now if we assume a `Player` group is expressed with those, you could express an `NPC` as an entity with just `IsSceneActor` and `HasStatistics`, so this way you can look at your entities in a high level way but making the best use of your more granular components.

## How groups work

So groups are pretty simple, they are just POCOs which describe the component types that you wish to match from within the collection of entities. So if you have hundreds of entities all with different components you can use a group as a way of expressing a high level intent for a system. 

> Under the hood component `Type` instances are converted into `int` component Ids, then there are multiple internal components which deal with tracking/resolving entities into groups, which are known as `Computed Entity Groups` which are what `Systems` use to know what `Entities` need to be processed.

## Creating Groups

There are a few different ways to create a group, here are some of the common ways.

### Instantiate a group

There is a `Group` class which implements `IGroup`, this can be instantiated and passed any of the components you want to target, like so:

```csharp
var group = new Group(typeof(SomeComponent));
```

There are also some helper methods here so you can add component types if needed via extension methods, like so:

```csharp
var group = new Group()
    .WithComponent<SomeComponent>()
    .WithoutComponent<SomeOtherComponent();
```

This is a halfway house between the builder approach and the instantiation approach, which is often handy if you want to quickly create a new group from an existing one. It also supports adding many components at once via params if needed.

### Use the GroupBuilder

So there is also a `GroupBuilder` class which can simplify making complex groups, it is easy to use and allows you to express complex group setups in a fluent manner, like so:

```csharp
var group = new GroupBuilder()
    .WithComponent<SomeComponent>()
    .WithComponent<SomeOtherComponent>()
    .Build();
```

### Implement your own IGroup

So if you are going to be using the same groupings a lot, it would probably make sense to make your own implementation of `IGroup`, this will mean less faffing with the above concepts to build a group.

It is quite simple to make your own group, you just need to implement the 2 getters:

```csharp
public class MyGroup : IGroup
{
    public IEnumerable<Type> RequiredComponents {get;} =  return new[] { typeof(SomeComponent), typeof(SomeOtherComponent) };
        
    public IEnumerable<Type> ExcludedComponents {get;} =  return new[] { typeof(SomeComponentIDontWant) };
}
```

As you can see, you can now just instantiate `new MyGroup();` and everyone is happy.

## Required vs Excluded components?

In most cases you probably only care about `RequiredComponents` but in some cases you may have multiple groups with the same required components but you don't want to process certain `Entities` and you can gate this by having `ExcludedComponents` to indicate entities with those components should be ignored from the group.

## What is a `LookupGroup`?

If you have looked in the lower level APIs you will probably see `LookupGroup` all over the place, and internally an `IGroup` is resolved into a `LookupGroup` which resolves the component `Type` instances into the component type id `ints`.

> You will find that a lot of this framework has a sort of higher level nicer user facing functionality, and lower level more performant functionality, so while its interesting to know some of this, it isn't needed knowledge to use the framework.