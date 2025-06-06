using EcsR3.Computeds.Entities;
using EcsR3.Computeds.Entities.Conventions;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.Custom.ComputedComponents.Components;
using EcsR3.Extensions;

namespace EcsR3.Examples.Custom.ComputedComponents.Computeds;

public class ComputedEntityProcessor : ComputedFromEntityGroup<int>
{
    public IEntityComponentAccessor EntityComponentAccessor { get; }

    public ComputedEntityProcessor(IEntityComponentAccessor entityComponentAccessor, IComputedEntityGroup dataSource) :
        base(dataSource)
    {
        EntityComponentAccessor = entityComponentAccessor;
    }

    protected override bool UpdateComputedData()
    {
        ComputedData = 0;
        foreach (var entityId in DataSource)
        { 
            ComputedData +=  EntityComponentAccessor.GetComponent<NumberComponent>(entityId).Value 
                             + EntityComponentAccessor.GetComponent<Number2Component>(entityId).Value; 
        }

        return true;
    }
}