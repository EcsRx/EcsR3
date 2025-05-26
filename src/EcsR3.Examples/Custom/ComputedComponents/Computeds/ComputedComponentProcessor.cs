using System;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components;
using EcsR3.Computeds.Components.Conventions;
using EcsR3.Examples.Custom.ComputedComponents.Components;

namespace EcsR3.Examples.Custom.ComputedComponents.Computeds;

public class ComputedComponentProcessor : ComputedDataFromComponentGroup<int, NumberComponent, Number2Component>
{
    public ComputedComponentProcessor(IComponentDatabase componentDatabase, 
        IComputedComponentGroup<NumberComponent, Number2Component> dataSource) 
        : base(componentDatabase, dataSource)
    {
    }

    protected override void UpdateComputedData(ReadOnlyMemory<(int, NumberComponent, Number2Component)> componentData)
    {
        ComputedData = 0;
        var span = componentData.Span;
        for(var i=0;i<componentData.Length;i++)
        { ComputedData += span[i].Item2.Value + span[i].Item3.Value; }
    }
}