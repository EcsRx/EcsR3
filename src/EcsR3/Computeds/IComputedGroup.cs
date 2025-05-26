using System;
using EcsR3.Groups;

namespace EcsR3.Computeds
{
    public interface IComputedGroup : IDisposable
    {
        /// <summary>
        /// The underlying group
        /// </summary>
        /// <remarks>
        /// The token contains both components required
        /// </remarks>
        LookupGroup Group { get; }
    }
}