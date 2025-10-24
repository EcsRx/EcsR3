# `EcsR3` Computeds

The `Computeds` within EcsR3 build on top of the ones provided within SystemsR3 and provide higher level conventions that align with the ECS paradigms.

> If you haven't already, check out the [SystemsR3 Computeds Docs](../../systems-r3/framework/computeds.md) first, which covers the high level blurb about what `Computeds` are and the basic `Computed` types/conventions available.

## `IComputedGroup`

This is more a convention than an actual `Computed` type, it just indicates that the `Computed` implements an `IGroup` accessor so you know the `Computed` is constrained by a group.

## `IComputedEntityGroup`

The `IComputedEntityGroup` builds on top of `IComputedCollection` and computes all `Entities` within a given `Group`.

It also exposes observables to represent when an `Entity` has been added or removed from the underlying computed group.

> These are used heavily throughout the framework to maintain resolved groups, such as `Systems` which will use these to get access to all `Entities` they should operate on.

You can access these or request your own should you need them via the `IComputedEntityGroupRegistry`, it will provide you an existing `ComputedEntityGroup` should you request a `Group` which has already been setup.

### `ComputedEntityGroupFromEntityGroup`

Now we can all laugh at the name here, but this is basically the same as the previous `Computed Entity Group`, but it's a virtualized version which allows for further constraining, providing you the underlying `IComputedEntityGroup` as the `DataSource` and expecting 

> For example you may want to take a base `Computed Entity Group` and then restrict it further only returning entities which are for a given faction or low health.

### `ComputedFromEntityGroup<T>`

This provides you with an `IComputedEntityGroup` as the `DataSource` then you translate them into whatever you want `T` to be.

> This inherits from the lazy computed line, so it will only refresh its value when you read its `Value`(and it has awaiting changes) or you explicitly call `ForceRefresh`.

## `IComputedComponentGroup`

While this is listed as a high level `Computed` and not a convention, in reality it is a `ComputedFromEntityGroup<ReadOnlyMemory<ComponentBatch<...>>>` which is a bit of a mouthful, but it provides a really performant way to access components for `Entities`.

> For example if you have a group that requires `ComponentA`, `ComponentB` then this computed will provide `ComponentBatch<ComponentA, ComponentB>` for each entity, allowing quick lookup and processing, this is what `BatchedSystems` use under the hood to resolve components.

> This inherits from the lazy computed line, so it will only refresh its value when you read its `Value`(and it has awaiting changes) or you explicitly call `ForceRefresh`.

### `ComputedFromComponentGroup<T>`

This provides you a `IComputedComponentGroup` as the `DataSource` and lets you process it however you want into `T`.

> This inherits from the lazy computed line, so it will only refresh its value when you read its `Value`(and it has awaiting changes) or you explicitly call `ForceRefresh`.
