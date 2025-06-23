# Entities

Conceptually entities have `Components` added/removed from them, so you can think of them as little databases with each `Component` being a table inside of it.

Under the hood though it's a lot simpler, they are basically `Id` handles into component data, which is stored somewhere else in huge contiguous memory arrays for each component type, but we still have an `Entity` struct that wraps and represents this notion for usage throughout the framework.

> Historically entities used to be reference types and would internally store their component allocation data etc, but it led to fragmented allocation data as well as made it problematic to do batch actions on many entities at once.
> 
> To solve this problem EcsR3 changed EcsRx's `IEntity` from a class implementation to be a simple `Entity` struct that can be passed around easily and defer operations on the `Entity` to another class.

## Creating/Accessing entities

Entities are created via the `Entity Collection` so you don't need to do much here other than get the collection and call the `Create` method which will return you an `Entity` to play with.

> You are often given the required `Entity` instances within `System` execution/processing methods so you don't need to track entities yourself, your job is to ensure entities have the right components on them to fit into the `Groups` that you need them to be in for `Systems` to pick up.

## Adding/Removing components on entities

Once you have your `Entity` you can start adding/removing `Components` to/from it via the `Entity Component Accessor` (injectable as `IEntityComponentAccessor` anywhere you need it).

For the most part you will find `ISystem` implementations, `EcsR3Applications` and most other classes where you are expected to interact with `Entities` will provide you an instance of `IEntityComponentAccessor` either by local argument passing or a local property on the instance you can access.

Most interactions would look like so:

```csharp
entityComponentAccessor.CreateComponent<MyComponent>(myEntity);
var component = entityComponentAccessor.GetComponent<MyComponent>(myEntity);
entityComponentAccessor.RemoveComponent<MyComponent>(myEntity);
```

> You can also do batch operations by providing multiple `Components` or `Entities` as arguments, although most of these are extension methods.