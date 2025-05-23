using EcsR3.Computeds.Entities.Registries;
using EcsR3.Groups;
using SystemsR3.Factories;

namespace EcsR3.Computeds.Entities.Factories
{
    public interface IComputedEntityGroupFactory : IFactory<LookupGroup, IComputedEntityGroup> {}
}