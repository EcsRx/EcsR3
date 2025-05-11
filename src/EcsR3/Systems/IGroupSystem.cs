using EcsR3.Groups;
using SystemsR3.Systems;

namespace EcsR3.Systems
{
    /// <summary>
    /// The base interface for all systems, this is rarely used directly
    /// </summary>
    public interface IGroupSystem : ISystem
    {
        /// <summary>
        /// The group to target with this system
        /// </summary>
        IGroup Group { get; }
    }
}