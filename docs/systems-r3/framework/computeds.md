# `SystemsR3` Computeds

Computed values are basically read only values which are proxy a data source and update on changes, much like `Observable` instances which notify you on data changing, computed objects also let you see what the value of the object is as well.

> It is easiest to think of `computed` types as simple transformers for a datasource, data comes in/is update, it's processed in some way, the results are stored internally for accessing

## Computed Types

There are 4 default computed types available within the system:

### `IComputed` (For computed single values)
Simplest computed and provides a current value and allows subscription to when the value changes, this can be very useful for precomputing things based off other data, i.e calculating MaxHp once all buffs have been taken into account.

### `ILazyComputed` (Same as above)
Same as normal Computed but only updates when value is read or `ForceRefresh` is called, triggering `OnChange` exposes `OnHasChange` as well to indicate the dependent data has changed but not been refreshed.

### `IComputedCollection` (For computed collections of data)
A reactive collection which provides an up to date collection of values and allows you to subscribe to when it changes, this could be useful for tracking all beneficial buffs on a player where the source data is just ALL buffs/debuffs on the entity.

> EcsR3 adds on top of this and provides `IComputedGroup` and other related functionality

### `IComputedList<T>` (For computed collections with indexing support)
Same as reactive collection but provides `IReadOnlyList` style methods and conventions.

## How do I use them

So there are a few different ways to use them and most of that is based upon your use cases, and you can make your own implementations if you want to wrap up your own scenarios.

They all share a lot of commonality in how their internals work, for the most part the 2 main things to highlight are:
- `ComputedData` - This is the internal settable state of the computed, which `Value` proxies via a getter
- `DataSource` - This is the source of data you should process to update state for `ComputedData`

All of these classes are provided as `abstract` classes so you should inherit from them if you wish to build off them.

### `ComputedFromData<TOutput, TInput>`

The `ComputedFromData` class lets you pass in anything you want and output anything you want, it also lets you mix auto updating via the `RefreshWhen` or explicitly via `RefreshData` method.

Here is an example using the `ComputedFromData` to keep track of who is in first place at all times:

```csharp
// Create a computed from the racing collection data
public class ComputedFirstPlace : ComputedFromData<Racer, IReadOnlyCollection<Racer>>
{    
    // We pass in all the racers as the DataSource
    public ComputedFirstPlace(IReadOnlyCollection racers) : base(racers) {}

    // Lets automatically refresh every update
    protected override Observable<Unit> RefreshWhen()
    { return Observable.EveryUpdate(); }

    // When updating we use the Racers via the DataSource and return a true/false if the data has changed
    protected override bool UpdateComputedData()
    {
        // Calculate the first place racer
        var topRacer = DataSource.OrderBy(x => x.RacePosition).First();
        // Check if its different to the current computed state in ComputedData
        var hasChanged = topRacer == ComputedData;
        // If its changed we update the internal state
        if(hasChanged) { ComputedData = DataSource.Data; }
        // Return true/false based on if the internal state has changed (which will trigger observables)
        return hasChanged;
    }
}

var computedFirstPlaceRacer = new ComputedFirstPlace(collectionOfRacers); // inherits from ComputedFromData<Racer, IEnumerable<Racer>>
RacerHud.CurrentWinner.Text = computedFirstPlaceRacer.Value.Name;
```

### `LazyComputedFromData<TOutput, TInput>`
Same as previous `ComputedFromData` but a lazily evaluated variant.

### `ComputedFromObservable<TOutput, TInput>`

Much like the above `ComputedFromData` but the `DataSource` needs to be an `Observable<TInput>`, and will listen for changes on the observable and update its internal state accordingly, these are often known as **Pure Computeds** as they just proxy the underlying Observable.

> One major difference worth noting is that you cannot explicitly update with the observable as the state changes are driven by the observable change triggers, there is no "state" as such to access arbitrarily on an `Observable`

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