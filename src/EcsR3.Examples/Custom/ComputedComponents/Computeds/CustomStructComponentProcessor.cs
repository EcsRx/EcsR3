using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components;
using EcsR3.Examples.Custom.ComputedComponents.Components;
using R3;
using SystemsR3.Computeds.Conventions;

namespace EcsR3.Examples.Custom.ComputedComponents.Computeds;
/*
public class CustomComputedStructComponentProcessor : ComputedFromData<int, IComputedComponentGroup<StructNumberComponent, StructNumber2Component>>
{
    protected readonly IComponentPool<StructNumberComponent> ComponentPool1;
    protected readonly IComponentPool<StructNumber2Component> ComponentPool2;
    
    public CustomComputedStructComponentProcessor(IComponentDatabase componentDatabase, IComputedComponentGroup<StructNumberComponent, StructNumber2Component> dataSource) : base(dataSource)
    {
        ComponentPool1 = componentDatabase.GetPoolFor<StructNumberComponent>();
        ComponentPool2 = componentDatabase.GetPoolFor<StructNumber2Component>();
        RefreshData();
    }

    protected override Observable<Unit> RefreshWhen()
    { return Observable.Never<Unit>(); }

    protected override void UpdateComputedData()
    {
        var components1 = ComponentPool1.Components;
        var components2 = ComponentPool2.Components;

        ComputedData = 0;
        var batches = DataSource.Value;
        for (var i = 0; i < batches.Length; i++)
        {
            var batch = batches[i];
            ComputedData += components1[batch.Component1Allocation].Value + components2[batch.Component2Allocation].Value;
        }
    }
}*/