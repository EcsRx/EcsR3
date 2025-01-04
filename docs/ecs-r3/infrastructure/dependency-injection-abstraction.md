# Dependency Injection Abstraction

See the `SystemsR3` DI documentation for more information on the general DI abstractions available here.

### EcsR3 helpers

There is a helper which allows you to get an `IObservableGroup` directly from the container, this internally gets the instance of the `IEntityCollectionManager` and requests an observable group of a given type like so:

```csharp
// By group
var observableGroup = container.ResolveObservableGroup(new MyGroup());
// By required components
var observableGroup = container.ResolveObservableGroup(typeof(PlayerComponent));
```

This can be handy when you want to setup `Computed` objects which computed from a group, an example of this can be seen in the [Roguelike example](https://github.com/EcsRx/ecsrx.roguelike2d/blob/master/Assets/Game/Modules/ComputedModule.cs).