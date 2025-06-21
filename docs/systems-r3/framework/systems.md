# Systems

Systems are where all the logic lives, but these dont *NEED* to be an ECS only notion, and in `SystemsR3` we dont have the ECS paradigms, so the systems here are just for general execution and implement the ECS agnostic `ISystem` interface.

The way systems are designed there is an orchestration layer which wraps all systems and handles the communication between the pools and the execution/reaction/setup methods known as `ISystemExecutor` (Which can be read about on other pages).

You just express how you want to trigger your systems and let the `SystemExecutor` handle the heavy lifting and trigger the relevant method in the system when its time. This can easily be seen when you look at all the available system interfaces which all process individual entities not groups of them.

> This only documents the SystemsR3 available systems but EcsR3 builds on top of this and provides many other system types and an ECS paradigm.

## System Types

This is where it gets interesting, so we have multiple flavours of systems depending on how you want to trigger them, by default there is `IManualSystem` which acts as a simple setup/teardown style system. You can also mix them up so you could have a single system implement `IManualSystem`, `IBasicSystem` and `IReactToEventSystem` which would trigger all the required methods when system sets up/tears down, when an update happens and when an event comes in, but ultimately you can mix and match the interfaces however you want.

### `IManualSystem`

This is a niche/base system for when you want to carry out some logic outside the scope of entities/reactions, or want to have 
more fine-grained control over how you deal with the entities matched.

Rather than the `SystemExecutor` doing most of the work for you and managing the subscriptions it leaves it up to you
to manage everything how you want once the system has been started.

The `StartSystem` method will be triggered when the system has been added to the executor, and the `StopSystem` 
will be triggered when the system is removed.

```csharp
public class SomeManualSystem : IManualSystem
{
    // Triggered when the system is first registered
    public void StartSystem()
    {
        Console.WriteLine("System has started");
    }
        
    // Triggered when the system is removed/stopped
    public void StopSystem()
    {
        Console.WriteLine("System has ended");
    }
}
```

> These can often be used as sort of self contained system convention base classes, where you don't want/need a whole `IConventionalSystemHandler` and just want to do some setup/stopping logic, this is what the `BatchedSystems` use in the ECS layer. 

### `IBasicSystem`

This is a basic system that is triggered every update (based on scheduler update frequency) and lets you do anything you want per update.

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

### `IReactiveSystem`

This allows you to react to any observable and be passed the data for execution.

```csharp
public class RandomNumberTriggeringSystem : IReactiveSystem<int>
{
    private Random _random = new Random();
    
    // Every second we will trigger
    public Observable<T> ReactTo() => Observable.Interval(TimeSpan.FromSeconds(1)).Select(_random.Next(0,100));
    
    // Every second we are given a random number to do something with
    public void Execute(int randomNumber)
    {
        Console.WriteLine($"System generated {randomNumber}");
    }
}
```

### `IReactToEventSystem`

This allows you to react to any event of that type which is published over the `IEventSystem`.

```csharp
public class TakeDamageSystem : IReactToEventSystem<EntityDamagedEvent>
{
    // This allows us to alter how we observe the event, in most cases you can pass through, but you may want to `ObserveOnMainThread` or `ThrottleLast`
    public Observable<EntityDamagedEvent> ObserveOn(Observable<EntityDamagedEvent> observable)
    { return observable; }

    // Take the event data and do something with it
    public void Process(EntityDamagedEvent eventData)
    { eventData.HealthComponent.Health.Value -= eventData.DamageApplied; }
}
```

## System Loading Order

So by default (with the default implementation of `ISystemExecutor`) systems will load in the order you add them, however you can add a `[Priority]` attribute to indicate an explicit order for running.

## Composite Systems

You can mix and match interfaces on a single class, so for example you may want a system which reacts to 2 events and does basic execution as well, such as:

```csharp
public class DamageSystem : IReactToEventSystem<OnHealed>, IReactToEventSystem<OnDamaged>, IBasicSystem
{
    public GameState GameState {get;}
    
    public DamageSystem(GameState gameState) { GameState = gameState; }
    
    public Observable<OnHealed> ObserveOn(Observable<OnHealed> observable) => observable;
    public Observable<OnDamaged> ObserveOn(Observable<OnDamaged> observable) => observable;
       
    public void Process(OnHealed args) => GameState.PlayerHealth += args.Amount;
    public void Process(OnDamaged args) => GameState.PlayerHealth -= args.Amount;
    
    public void Execute(ElapsedTime elapsedTime)
    {
        if(GameState.PlayerHealth > 0)
        { Console.WriteLine($"Player is alive with {GameState.PlayerHealth}/{GameState.PlayerMaxHealth} remaining"); }
        else
        { Console.WriteLine("Player has died"); }
    }
}
```
> This is one reason why a lot of systems are expressed as interfaces for composition rather than classes for inheritance, as this allows you to express your intent easier and not have to worry about any infrastructure, the `IConventionalSystemHandlers` for each interface will do all the heavy lifting and ensure execution occurs when its needed.