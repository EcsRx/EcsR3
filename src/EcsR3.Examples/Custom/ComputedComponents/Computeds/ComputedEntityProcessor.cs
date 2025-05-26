using EcsR3.Computeds.Entities;
using EcsR3.Computeds.Entities.Conventions;
using EcsR3.Examples.Custom.ComputedComponents.Components;
using EcsR3.Extensions;

namespace EcsR3.Examples.Custom.ComputedComponents.Computeds;

public class ComputedEntityProcessor : ComputedFromEntityGroup<int>
{
    public ComputedEntityProcessor(IComputedEntityGroup dataSource) : base(dataSource)
    {}

    protected override void UpdateComputedData()
    {
        ComputedData = 0;
        foreach (var entity in DataSource)
        { 
            ComputedData +=  entity.GetComponent<NumberComponent>().Value 
                           + entity.GetComponent<Number2Component>().Value; 
        }
    }
}