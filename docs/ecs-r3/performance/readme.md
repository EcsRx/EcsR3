# Performance

If you want to maximize performance then here is quick set of advisories which can greatly increase performance and reduce allocations:

## 1. Use BatchedSystems Wherever Possible - *Low Effort* / *High Performance Gains*
The `BatchedSystem` (and related `BatchedRefSystem`, `BatchedMixedSystem`) is the most performant system for scheduled execution on entities, this is because it caches all the lookup data ahead of time and pre-accesses all the components at once, so the cpu doesnt need to do any memory hopping every time you want a component.

## 2. Try To Reuse Same Groups - *Low Effort* / *Reduces Background Processing*
In some instances you may have multiple systems which all use the same core data but with one or two differences, i.e:

```csharp
public class MyBatchedSystem1 : BatchedSystem<SomeComponentA, SomeComponentB, SomeComponentC> { /* ... */ }
public class MyBatchedSystem2 : BatchedSystem<SomeComponentA, SomeComponentB, SomeComponentC, SomeComponentD> { /* ... */ }
public class MyBatchedSystem3 : BatchedSystem<SomeComponentA, SomeComponentB, SomeComponentD> { /* ... */ }
public class MyBatchedSystem4 : BatchedSystem<SomeComponentA, SomeComponentC, SomeComponentD> { /* ... */ }
```
This is fine to do, but it means you are maintaining 4 different observable groups and having to build up 4 different batches for processing, if you were instead to make them all:

```csharp
public class MyBatchedSystem1 : BatchedSystem<SomeComponentA, SomeComponentB, SomeComponentC, SomeComponentD> { /* ... */ }
public class MyBatchedSystem2 : BatchedSystem<SomeComponentA, SomeComponentB, SomeComponentC, SomeComponentD> { /* ... */ }
public class MyBatchedSystem3 : BatchedSystem<SomeComponentA, SomeComponentB, SomeComponentC, SomeComponentD> { /* ... */ }
public class MyBatchedSystem4 : BatchedSystem<SomeComponentA, SomeComponentB, SomeComponentC, SomeComponentD> { /* ... */ }
```

Then just ignore the components you do not need for processing you will end up with less `Computed Entity Group` instances being refreshed in memory.

> This is not always doable as it may mean certain entities will be processed when you do not want them to be, but if you are able to it can help reduce background workloads.

## 3. Use Multitheading Where Safe - *Low Effort* / *Medium Performance Gains*
There is a `Multithreaded` attribute that you can put on most systems and it will tell the related executor for the system to process each entity in parallel rather than sequentially.

This can improve processing speeds on higher volume systems (especially `BatchedSystems`) where you are processing lots of entities and doing lots of logic against them, the more logic being done the bigger the performance gain from spreading it out, if there is not much processing the performance gains may not be as much.

> You may not always be able to do this, as depending you your platform/engine being used, you may be unable to do certain logic in another thread, i.e Unity will not let you update transforms/GOs etc unless you are on the main thread

## 4. Use `Computeds` For Non Entity State - *Medium Effort* / *Varying Performance Gains*
The mileage for this one varies based on your scenarios, but let's make up a whimsical scenario where you have multiple systems which need to know how far an entity is from the player.

Assuming a single player, you could just recalculate the distance (i.e `Vector3.Distance(playerPos, entityPos)`) every time you need it, but its adding extra processing to a system.

There are a few other options like caching it in the Systems, but then you would need each system to cache its own version of it, and add a pre-processing step or something.

A far simpler way is to just make a `ComputedFromComponentGroup` (or similar) implementation and have that constantly recalculate the distance from the player and inject that into each system.

Here is a rough example of how that could look:

```csharp
// We compute a Dictionary<int,float> where the int is the Entity Id and the float is the distance
public class ComputedDistanceFromPlayer : ComputedFromComponentGroup<Dictionary<int, float>, ExampleEnemyComponent, Transform3DComponent>
{
    private Entity _playerEntity;
    private Transform3D _playerTransform;
    
    // In this example we pass in the entity collection to get the player entity and its transform component
    public ComputedRuntimeColliders(IEntityCollection entityCollection, IComponentDatabase componentDatabase, IComputedComponentGroup<ExampleEnemyComponent, Transform3DComponent> dataSource) : base(componentDatabase, dataSource)
    {
        ComputedData = new Dictionary<int, Rectangle>();
        _playerEntity = // Get player entity from entityCollection somehow
        _playerTransform = _playerEntity.GetComponent<Transform3DComponent>();
    }

    // We could set this to any interval, event etc or even Never and only manually refresh, but for now lets assume we want to recalculate it every udpate
    protected override Observable<Unit> RefreshWhen()
    { return Observable.EveryUpdate(); }

    // This Component type provides you a premade memory batch of the entities and components for you to process
    protected override void UpdateComputedData(ReadOnlyMemory<(Entity, ExampleEnemyComponent, Transform3DComponent)> componentData)
    {
        // Get a span of it so we can iterate
        var componentDataSpan = componentData.Span;
        for (var i = 0; i < componentDataSpan.Length; i++)
        {
            var (entity, _, transformComponent) = componentDataSpan[i];
            var distance = Vector3.Distance(_playerTransform.Position, transformComponent.Transform.Position);
            ComputedData[entity.Id] = distance;
        }
    }
}
```
Then all we need to do is inject this in and call `computedDistanceFromPlayer.ComputedData[entity.Id]` in each system that needs to get the distance.

> This sort of approach can be used in so many ways, and also gives you the benefit of being able to alter how often its updated to reduce background processing, i.e in some instances you may only need to update the distances every second or so if your entities dont move quickly or it's used for simple AI lookups etc.

## 5. Avoid Using `IReactToEntitySystem` where possible - *Varying Effort / *High Performance Gain*

This isn't to say "NEVER USE THIS TYPE OF SYSTEM" but its mainly only meant to be used for more complex entity predicate execution scenarios, which in most cases can often be expressed in a `BatchedSystem` where the first line of execution checks the predicate and `returns` straight away if not met, this will often be far faster than the react to entity system, but you have less control over the reactivity triggers of it.

> If used well you can actually gain performance from these system types if you are doing throttled inputs or some other scenario which reduces computation to entities only when needed, but in reality this is rarely the case.

## 6. Explicitly setup your Component Type Ids and use the ids instead of types - *Varying Effort* / *Low Performance Gain*

Out the box most of your component interactions will be done via generics like `entity.GetComponent<SomeComponent>();` and this is fine for most cases.

Behind the scenes when EcsR3 starts it builds an index/id list based off the available components and will under the hood use these indexes to interact with the component data, so when you provide it a generic at the entity layer, its actually being looked up and converted into an index to be used in the underlying data stores.

So if you are happy to provide a type index/id with your call it will bypass the lookup step for the generic, meaning less overhead in entity/component interactions, for example the call above would now be `entity.GetComponent<SomeComponent>(ComponentLookupTypes.SomeComponentId)` (or if you dont care about the strong type) `entity.GetComponent(ComponentLookupTypes.SomeComponentId)`.

So this change means **YOU** have to tell EcsR3 ahead of time what indexes/ids to use for your components and provide that data to the rest of your codebase, its easiest to do this by making a class with static int properties like so:

```csharp
public static class ComponentLookupTypes
{
    public static int NameComponentId = 0;
    public static int PositionComponentId = 1;
    public static int MovementSpeedComponentId = 2;
}
```
> You could also just put public readonly static Ids on each `IComponent` implementation, there are many ways you can represent the ids of each component.

Then you can just reference these types anywhere which satisfies the telling of the entity the index, and you then just need to make sure when the application is created it uses these explicit lookups rather than auto generating them, generally done by making your own module and loading it like so:

```csharp
public class CustomComponentLookupsModule : IDependencyModule
{
    public void Setup(IDependencyContainer container)
    {
        container.Unbind<IComponentTypeLookup>();
        var explicitTypeLookups = new Dictionary<Type, int>
        {
            {typeof(NameComponent), ComponentLookupTypes.NameComponentId},
            {typeof(PositionComponent), ComponentLookupTypes.PositionComponentId},
            {typeof(MovementSpeedComponent), ComponentLookupTypes.MovementSpeedComponentId}
        };
        var explicitComponentLookup = new ComponentTypeLookup(explicitTypeLookups);
        container.Bind<IComponentTypeLookup>(new BindingConfiguration{ToInstance = explicitComponentLookup});
    }
}
```

This will unbind the default implementation which auto generates, and replaces it with explicit config.

It's not a MASSIVE performance boost, but it will reduce lookups into dictionaries on entity interactions, so this can reduce memory consumption in a few scenarios as there is no need for the system to build up `Type` objects etc.

To make use of this stuff you can now do calls like `entityComponentAccessor.GetComponent(entity, ComponentLookupTypes.NameComponentId);`