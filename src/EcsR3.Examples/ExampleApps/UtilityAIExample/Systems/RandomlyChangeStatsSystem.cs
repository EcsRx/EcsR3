using System;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.ExampleApps.UtilityAIExample.Components;
using EcsR3.Systems.Batching.Convention;
using OpenRpg.Core.Utils;
using R3;
using SystemsR3.Threading;

namespace EcsR3.Examples.ExampleApps.UtilityAIExample.Systems;

public class RandomlyChangeStatsSystem : BatchedSystem<CharacterDataComponent>
{
    public IRandomizer Randomizer { get; }
    
    public RandomlyChangeStatsSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler, IRandomizer randomizer) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
    {
        Randomizer = randomizer;
    }

    protected override Observable<Unit> ReactWhen()
    { return Observable.Interval(TimeSpan.FromSeconds(3)); }

    protected override void Process(Entity entity, CharacterDataComponent characterDataComponent)
    {
        characterDataComponent.Health = Randomizer.Random(0, characterDataComponent.MaxHealth);
    }
}