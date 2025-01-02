# `SystemsR3` Computeds

Computed values are basically read only values which are updated on changes, much like `IObservable` instances which notify you on data changing, computed objects also let you see what the value of the object is as well.

## Computed Types

There are 2 computed types available within the system:

### `IComputed<T>` (For computed single values)

Simplest computed and provides a current value and allows subscription to when the value changes, this can be very useful for precomputing things based off other data, i.e calculating MaxHp once all buffs have been taken into account.

### `IComputedCollection<T>` (For computed collections of data)

A reactive collection which provides an up to date collection of values and allows you to subscribe to when it changes, this could be useful for tracking all beneficial buffs on a player where the source data is just ALL buffs/debuffs on the entity.

> EcsR3 adds on top of this and provides `IComputedGroup` and other related functionality

## How do I use them

So there are a few different ways to use them and most of that is based upon your use cases, and you can make your own implementations if you want to wrap up your own scenarios.

All of these classes are provided as `abstract` classes so you should inherit from them if you wish to build off them.

### `ComputedFromData`

```csharp
var firstPlaceRacer = new ComputedFirstPlace(collectionOfRacers); // inherits from ComputedFromData<Racer, IEnumerable<Racer>>
RacerHud.CurrentWinner.Text = firstPlaceRacer.Value.Name;
```

This is a versatile computed generator where you can basically create a pre computed variable based upon anything. So you pass in any object you require which represents the state, then you calculate what the output value should be internally.

## Sounds good, but why?

You may never need this functionality, but in some cases you may want to share pre-computed data around your application without having to constantly re-compute it everywhere. This can make your code more simplistic and easier to maintain while also providing performance benefits were you do not need to keep doing live queries for data.

So to look at some real world scenarios, lets pretend we have a game with the following components:

- `CanAttack`
- `HasHealth`
- `HasLevel`
- `HasGroup`

We now have a few requirements:

- We need to show on the HUD all the people within OUR group
- We need to show an effect on someone in the group when their HP < 20%
- We need to show a value as to how hard the current area is

Now I appreciate this is all a bit whimsical but stay with me, now we can easily constrain on groups based on the components, so we can find all entities which are in a group and can attack etc, but thats where our current observations stop.

### Showing all party members with low HP (`IComputedCollection` scenario)

We now have a computed group of party members, but now we want to be able to know who in that group has low health, so we can create an `IComputedCollection` which is already constrained to the party (so we dont need to worry about working out that bit again), then we can check if their health is < 20% and if so put them in the list with their HP value, this way we can just bind our whimscial `PartyMembersWithLowHealthComputedCollection` which would implement `IComputedCollection<PartyMemberWithLowHealth>` (verbose I know) in the DI config then inject it into a system and boom you now have a system which can just look at this one object to find out whos got low health and be notified when anything changes.

### Show a value as to how hard the current area is (`IComputed` scenario)

So with the other bits out the way we basically want a way to quickly identify how hard the current area is, lets just assume this is based on what level all the enemies within a 30 unit radius is.

So we know how to make computed groups, so we can make one of them to wrap up all entities which are not within our group and are within 30 units of the player. Once we have that we can then create a computed variable (which just exposes a singular value) to loop through all the enemies within 30 units (which we now have from the computed group) and get all the enemies levels and average then, returning that as the result.

This way you can inject this into various other places, so if you need to show some colour indicator on current difficulty or warn the player you can use this value.