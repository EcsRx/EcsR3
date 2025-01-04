using EcsR3.Entities;

namespace EcsR3.Groups
{
    public interface IHasPredicate
    {
        bool CanProcessEntity(IEntity entity);
    }
}