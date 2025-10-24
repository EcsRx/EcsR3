# Systems

Systems are where all the logic lives, it takes `Entities` from the collection and executes logic on each one. The way `Systems` are designed there is an orchestration layer which wraps all systems and handles the communication between the pools and the execution/reaction/setup methods known as `ISystemExecutor` (Which can be read about on other pages).

This means your systems don't need to worry about the logistics of getting entities and dealing with them, you just express how you want to interact with entities and let the `SystemExecutor` handle the heavy lifting and pass you the entities for processing. This can easily be seen when you look at all the available system interfaces which all process individual entities not groups of them.

> If you havent already its recommended you look at the [SystemsR3 Systems Docs](../../systems-r3/framework/systems.md), which cover some system paradigms we build on top of here or can still be used in conjunction with the ECS ones.

## System Types
All **ECS** *(Not SystemsR3)* systems have the notion of a `Group` which describes what entities to target out of the pool, so you don't need to do much other than setup the right group contracts and implement the methods for the interfaces.

### `BatchedSystem`

This is by far the quickest system type and should probably be your default system to use for all entity execution, it simply aligns all the entities and their related component allocations up in memory then provide them directly for you to process.

This means you do not need to do any `entityComponentAccessor.GetComponent<SomeComponent>()` for each thing you want in the `Group`, you just get given all the required components via the method call.

```csharp
public class BatchedExampleSystem : BatchedSystem<SomeComponentA, SomeComponentB>
{
    // We dont need to provide an explicit Group as it can infer them from the generics provided, but you can override it for ExcludedComponent scenarios
    
    protected override Observable<Unit> ReactWhen()
    { return Observable.EveryUpdate(); }
    
    protected override void Process(Entity entity, SomeComponentA componentA, SomeComponentB componentB)
    {
        // Do something with the components and/or entity
    }    
}
```

> You can use both `class` and `struct` components with this system, but the structs will be passed by value so are read only (there is a performance benefit to not having to deal with `refs`)

### `BatchedRefSystem`

This is the same as the `BatchedSystem` for all intents and purposes but it is meant for `struct` based `Components` and provides them as `refs` so they can be updated within the system. 

```csharp
public class BatchedExampleSystem : BatchedRefSystem<SomeStructComponentA, SomeStructComponentB>
{
    // We dont need to provide an explicit Group as it can infer them from the generics provided, but you can override it for ExcludedComponent scenarios
    
    protected override Observable<Unit> ReactWhen()
    { return Observable.EveryUpdate(); }
    
    protected override void Process(Entity entity, ref SomeStructComponentA componentA, ref SomeStructComponentB componentB)
    {
        // Do something with the components refs and/or entity
    }    
}
```

> This `System` is fine if all your `struct` components are going to need to be written to, but if you only need to write to some of them and the others are just for reading from you should look at the `BatchedMixedSystem` below.

### `BatchedMixedSystem`

This is a hybrid of both previous systems, it allows you to mix `ref` and non ref components.

> This can be handy if you have a mix of `class` and `struct` components but only a couple of the `struct` components need to be updated, the rest of read only, or the others are classes.

```csharp
public class BatchedExampleSystem : BatchedMixedSystem<SomeStructComponentA, SomeStructComponentB>
{
    // We dont need to provide an explicit Group as it can infer them from the generics provided, but you can override it for ExcludedComponent scenarios
    
    protected override Observable<Unit> ReactWhen()
    { return Observable.EveryUpdate(); }
    
    // First one is mutable, latter is immutable
    protected override void Process(Entity entity, ref SomeStructComponentA componentA, SomeStructComponentB componentB)
    {
        // Do something with the components refs and/or entity
    }    
}
```

> Due to the MANY permutations of this that you can have its recommended that if you have very specific scenarios you just copy the code for the current `BatchedMixedSystem` and alter the signatures however you need.

### `MultiplexingSystem` + `IMultiplexedJob`

This is very much like a batched system, but it acts as a sort of multiplexer to allow you to run multiple `jobs` within one update, a `job` is basically the same as a batched systems `Process` method but standalone.

> This is really useful if you have multiple systems which all require same components and execute at same time, this ensures that it only needs to lookup the batches once and then runs all jobs back to back with the batch data, this can allow for better utilisation of CPU/Memory.

```csharp
// Make as many jobs as you want, you can also use ISystemPreProcessor/PostProcessor with it
public class Job1 : IMultiplexedJob<ClassComponent, ClassComponent2, ClassComponent3>
{
    // Notice we dont give it a schedule, it is handled by the systems schedule
    public void Process(Entity entity, ClassComponent component1, ClassComponent2 component2, ClassComponent3 component3)
    {
        component1.Position += Vector3.One;
        component1.Something += 10;
        component2.IsTrue = true;
        component2.Value += 10;
    }
}

// Notice that even though we don't use Component2 here, and the previous didnt use Component1 it doesnt matter too much as we 
// still get a performance bonus due to it scheduling both things in same block
public class Job2 : IMultiplexedJob<ClassComponent, ClassComponent2, ClassComponent3>
{
    public void Process(Entity entity, ClassComponent component1, ClassComponent2 component2, ClassComponent3 component3)
    {
        component1.Position += Vector3.One;
        component1.Something += 10;
        component3.IsTrue = true;
        component3.Value += 10;
    }
}
    

// This acts like a normal batched system, but it just expects you to provide it the jobs you want to run
public class ExampleMultiplexedSystem : MultiplexingBatchedSystem<ClassComponent, ClassComponent2, ClassComponent3>
{
    public ExampleMultiplexedSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
    {}

    // This is scheduling when all jobs should be run
    protected override Observable<Unit> ReactWhen() => Observable.EveryUpdate();
    
    // This is a simple example, but you can always DI in the jobs and pass them into here for more complex use cases
    protected override IEnumerable<IMultiplexedJob<ClassComponent, ClassComponent2, ClassComponent3>> ResolveJobs()
    { return [new Job1(), new Job2()]; }
}
```
On one hand this may seem slightly more complex but in a way it also makes things a bit simpler as your Jobs are lightweight objects that can just be scheduled together and you have a smaller logic footprint.

> Remember you dont need to have 100% overlap on required components etc, there will be a tipping point but if you have several systems which all use 75% of the same components you can possibly get a decent performance bonus making them into jobs and giving them all the same components, even if a few of the jobs ignore a component or two it may still end up being more efficient than running them as fully fledged systems.

### `MultiplexingBatchedRefSystem` + `IMultiplexedRefJob`
Same as above but it lets you pass the components to jobs with `ref` keyword, mainly meant for struct scenarios.

> There is currently no mixed one but it may be added in the future, there is so many varieties of approaches to mix `ref` it is currently left to you to implement your own variants if you need them.

### `IBasicEntitySystem`

This system is like a `IBasicSystem` allowing you to process each entity within the group on every update cycle.

```csharp
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
### `ISetupSystem`

This interface implies that you want to setup entities, so it will match all entities via the group and will run a `Setup` method once for each of the entities. 

> This is primarily there for doing one off setup methods on entities, such as setting up view related data (i.e `ViewResolvers`) or other one time things.

```csharp
public class SetupAsteroidSystem : ISetupSystem
{
    public IGroup Group { get; } = new Group(typeof(AsteroidComponent), typeof(ViewComponent));
    
    // When an entity initially joins the group it will have this Setup method execute once
    public void Setup(IEntityComponentAccessor entityComponentAccessor, Entity entity)
    {
        var viewComponent = entityComponentAccessor.GetComponent<ViewComponent>(entity);
        viewComponent.View = new SomeAsteroidSpriteThingy();
    }
}
```

### `ITeardownSystem`

This is similar to `ISetupSystem`, but is used when a matched entity's group is *ABOUT* to be removed.

> This distinction is important because it means that all components should still be on the entity when this system gets triggered, if we triggered it after the component was removed you wouldnt be able to access it on the entity.

```csharp
public class TeardownAsteroidSystem : ITeardownSystem
{
    public IGroup Group { get; } = new Group(typeof(AsteroidComponent), typeof(ViewComponent));
    
    // When an entity is about to leave the group this will get triggered once
    public void Teardown(IEntityComponentAccessor entityComponentAccessor, Entity entity);
    {
        var viewComponent = entityComponentAccessor.GetComponent<ViewComponent>(entity);
        viewComponent.View.Dispose();
        viewComponent.View = null;
    }
}
```

### `IReactToEntitySystem`

This interface implies that you want to react to individual changes in an entity. It will pass each entity to the `ReactToEntity` method to setup the observable you want, such as Health changing, input occurring, random intervals etc. This only happens once per matched entity, here is an example of the sort of thing you would do here:

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

Once you have setup your reactions the `Process` method is triggered every time the subscription from the reaction phase is triggered, so this way your system reacts to data rather than polling for changes each frame, this makes system logic for succinct and direct, it also can make quite complex scenarios quite simple as you can use the power of **R3** to daisy chain together your observables to trigger whatever you want.

### `IReactToGroupSystem`

This is like the `IReactToEntitySystem` but rather than reacting to each entity matched, it instead just schedules at the at the group level. The `ReactToGroup` is generally used as a way to process all entities every frame using `Observable.EveryUpdate()` and selecting the group, however you can do many other things such as reacting to events at a group level or some other observable notion, here is a simple example:

```csharp
public class ExampleReactToGroupSystem : IReactToGroupSystem
{
    public IGroup Group { get; } = new Group(typeof(SimpleReadComponent), typeof(SimpleWriteComponent));
    
    public Observable<IComputedEntityGroup> ReactToGroup(IComputedEntityGroup observableGroup)
    { return Observable.Interval(TimeSpan.FromSeconds(1)).Select(x => observableGroup); }

    public void Process(IEntityComponentAccessor entityComponentAccessor, Entity entity)
    {
        var readComponent = entityComponentAccessor.GetComponent<SimpleReadComponent>(entity);
        var writeComponent = entityComponentAccessor.GetComponent<SimpleWriteComponent>(entity);
        writeComponent.WrittenValue = readComponent.StartingValue;
    }
}
```

> While this system lets you do whatever you want on entities, in almost every scenario you would use this system, you would be better off using a `BatchedSystem`, i.e `BatchedSystem<SimpleReadComponent, SimpleWriteComponent>` which would function the same but perform FAR faster.

### `IReactToDataSystem<T>`

So this is the more complicated and lesser used flavour of system. It is basically the same as the `IReactToEntitySystem` however it reacts to data rather than an entity. 

> For example lets say you wanted to react to a collision event and your system wanted to know about the entity as normal, but also the collision event that occurred. This system is the way you would do that, as its subscription passes back some data rather than an entity, here is an example:

```csharp
public class ReactiveCollisionSystem : IReactToDataSystem<CollisionData>
{
    public IGroup Group => new Group().WithComponent<TestComponentOne>();

    public Observable<float> ReactToData(IEntityComponentAccessor entityComponentAccessor, Entity entity)
    { return MessageBroker.Receive<EntityCollisionEvent>().Single(x => x.collidee == entity); }

    public void Process(IEntityComponentAccessor entityComponentAccessor, Entity entity, CollisionData reactionData)
    {
        // do something with entity and data
    }
}
```

So this offers a bit more power as the `Process` method takes both the entity in the pool and the returned data from the subscription allowing you to work with external data when processing.

## System Loading Order

So by default (with the default implementation of `ISystemExecutor`) systems will load in the order of:

1. Implementations of `ISetupSystem`
2. Implementations of `IReactToEntitySystem`
3. Other Systems 

However within those groupings it will load the systems in whatever order Zenject/Extenject (assuming you are using it) provides them, however there is a way to enforce some level of priority by applying the `[Priority(1)]` attribute, this allows you to specify the priority of how systems should be loaded. The ordering will be from lowest to highest so if you have a priority of 1 it will load before a system with a priority of 10.

## Performance Implications

While there are a myriad of out the box conventional systems to use, its worth saying that in almost all scenarios your first go to system type should be `BatchedSystem`, its by far faster than the others and has enough flexibility to cope with most scenarios.

The `IReactToEntity` system is generally the slowest, so should only really be used when you *NEED* to react to specific per entity situations rather than reacting to the group as a whole.