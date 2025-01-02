using System;
using System.Numerics;

namespace SystemsR3.Plugins.Transforms.Extensions
{
    public static class VectorExtensions
    {
        /// <summary>
        /// Converts the Vector to an angle
        /// </summary>
        /// <param name="vector">The vector to convert to an angle</param>
        /// <returns>The angle in degrees</returns>
        public static float ToDegrees(this Vector2 vector)
        { return MathF.Atan2(vector.Y, vector.X) * Transform2DExtensions.RadiansToDegrees; }
        
        /// <summary>
        /// Converts the Vector to an angle (in radians)
        /// </summary>
        /// <param name="vector">The vector to convert to an angle</param>
        /// <returns>The angle in radians</returns>
        public static float ToRadians(this Vector2 vector)
        { return MathF.Atan2(vector.Y, vector.X); }
        
        /// <summary>
        /// Gets the angle towards a destination vector (in radians)
        /// </summary>
        /// <param name="source">The source position</param>
        /// <param name="destination">The destination to look at</param>
        /// <param name="offsetInRadians">Optional offset applied to the calculation, 0 by default but different coordinate systems may need offsets applied</param>
        /// <returns>Returns the angle (in radians) needed to look at the given destination position</returns>
        public static float GetAngleFor(this Vector2 source, Vector2 destination, float offsetInRadians = 0f)
        { return (destination - source).ToRadians() + offsetInRadians; }
    }
}