using System;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components;
using EcsR3.Computeds.Components.Conventions;
using EcsR3.Examples.Custom.ComputedComponents.Components;

namespace EcsR3.Examples.Custom.ComputedComponents.Computeds;

public class ComputedStructComponentProcessor : ComputedDataFromComponentGroup<int, StructNumberComponent, StructNumber2Component>
{
    public ComputedStructComponentProcessor(IComponentDatabase componentDatabase, IComputedComponentGroup<StructNumberComponent, StructNumber2Component> dataSource) : base(componentDatabase, dataSource)
    {
    }

    protected override void UpdateComputedData(Memory<(int, StructNumberComponent, StructNumber2Component)> componentData)
    {
        ComputedData = 0;
        var data = componentData.Span;
        for(var i=0;i<data.Length;i++)
        { ComputedData += data[i].Item2.Value + data[i].Item3.Value; }
    }
}