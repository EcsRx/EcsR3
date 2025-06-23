# Blueprints

Blueprints provide a re-useale way to setup entities with pre-defined components and values, it is ultimately up to the developer 
how they wish to implement blueprints, as you can add as much configurable properties to it as you wish but they all implement an 
`Apply` method which takes the entity and sets it up.

Here is an example of a blueprint:

```csharp
public class PlayerBlueprint : IBlueprint
{
	public string Name {get;set;}
	public string Class {get;set;}
	public int Health {get;set;}
	
	public void Apply(IEntityComponentAccessor entityComponentAccessor, IEntity entity)
	{
		entityComponentAccessor.AddComponent(entity, new HasNameComponent{ Name = Name });
		entityComponentAccessor.AddComponent(entity, new HasClassComponent{ Class = Class });
		entityComponentAccessor.AddComponent(entity, new HasHealthComponent{ MaxHealth = Health, CurrentHealth = Health });
	}
}
```

## Creating via blueprints

You can create entities via blueprints using extension methods on the `IEntityCollection`, this will allow you to do things like:

```csharp
var hanSoloEntity = entityCollection.Create(new PlayerBlueprint{ 
	Name = "Han Solo", 
	Class = "Smuggler", 
	Health = 100});
```

## Applying blueprints to existing entities

You have 2 options of applying blueprints to entities, one would be to just new up the desired blueprint and call the 
`Apply` method passing in the entity, or you could use the available extension methods to apply directly from the entity, 
this is also chainable so you are able to apply multiple blueprints to the same entity if you wanted, like so:

```csharp
entity.ApplyBlueprint(new DefaultActorBlueprint())
	.ApplyBlueprint(new DefaultEquipmentBlueprint())
	.ApplyBlueprint(new SetupNetworkingBlueprint());
```

## How much should a blueprint do?

This is ultimately up to you, but it was conceived as a way to do bulk component assignments to entities with a small 
amount of config or logic setup. Anything more than that should probably be handled by `SetupSystems` or some other object.

For example a blueprint ideally shouldnt need to have anything injected into it, so if you need to setup complex data 
like setting up a sprite or texture, or some other data payload from a 3rd party source, its recommended you let the blueprint
add the component to the entity, then have some system that catches that entity once its got the component added and then
have the system provide the 3rd party dependency data and set it up (i.e a `SetupSystem`).

## Batching

You can also use `IBatchedBlueprint` if you want to create multiple entities at once with a single blueprint, this can be 
far quicker at setting up multiple entities at once, as it will attempt to batch as many of the creation/update operations
as possible.

This is also exposed on the `IEntityCollection` as extension methods so you can provide an amount of entities you wish to 
create for the batch, then it will return a collection of the entites that have been created rather than a single one.
