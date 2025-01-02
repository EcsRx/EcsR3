# Systems

Systems are where all the logic lives. 

The way systems are designed there is an orchestration layer which wraps all systems and handles the communication between the pools and the execution/reaction/setup methods known as `ISystemExecutor` (Which can be read about on other pages).

You just express how you want to trigger your systems and let the `SystemExecutor` handle the heavy lifting and trigger the relevant method in the system when its time. This can easily be seen when you look at all the available system interfaces which all process individual entities not groups of them.

> This only documents the SystemsR3 available systems but EcsR3 builds on top of this and provides many other system types and an ECS paradigm.

## System Types

This is where it gets interesting, so we have multiple flavours of systems depending on how you want to trigger them, by default there is `IManualSystem` which acts as a simple setup/teardown style system. You can also mix them up so you could have a single system implement `IManualSystem`, `IBasicSystem` and `IReactToEventSystem` which would trigger all the required methods when system sets up/tears down, when an update happens and when an event comes in, but ultimately you can mix and match the interfaces however you want.

### `IManualSystem`

This is a niche system for when you want to carry out some logic outside the scope of entities, or want to have 
more fine grained control over how you deal with the entities matched.

Rather than the `SystemExecutor` doing most of the work for you and managing the subscriptions it leaves it up to you
to manage everything how you want once the system has been started.

The `StartSystem` method will be triggered when the system has been added to the executor, and the `StopSystem` 
will be triggered when the system is removed.

### `IBasicSystem`

This is a basic system that is triggered every update (based on scheduler update frequency) and lets you do anything you want per update.

### `IReactiveSystem`

This allows you to react to any observable and be passed the data for execution.

### `IReactToEventSystem`

This allows you to react to any event of that type which is published over the `IEventSystem`.

## System Loading Order

So by default (with the default implementation of `ISystemExecutor`) systems will load in the order you add them, however you can add a `[Priority]` attribute to indicate an explicit order for running.