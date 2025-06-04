using System;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components;
using EcsR3.Computeds.Components.Conventions;
using EcsR3.Entities;
using EcsR3.Examples.Custom.ComputedComponents.Components;

namespace EcsR3.Examples.Custom.ComputedComponents.Computeds;

public class ComputedStructComponentProcessor : ComputedFromComponentGroup<int, StructNumberComponent, StructNumber2Component>
{
    public ComputedStructComponentProcessor(IComponentDatabase componentDatabase, IComputedComponentGroup<StructNumberComponent, StructNumber2Component> dataSource) : base(componentDatabase, dataSource)
    {
    }

    protected override void UpdateComputedData(ReadOnlyMemory<(Entity, StructNumberComponent, StructNumber2Component)> componentData)
    {
        ComputedData = 0;
        var span = componentData.Span;
        for(var i=0;i<componentData.Length;i++)
        { ComputedData += span[i].Item2.Value + span[i].Item3.Value; }
    }
}