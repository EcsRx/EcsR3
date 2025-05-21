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
        public static float ToRadians(this Vector2 vector)
        { return MathF.Atan2(vector.Y, vector.X); }
        
        /// <summary>
        /// Converts the Vector to an angle (in radians)
        /// </summary>
        /// <param name="vector">The vector to convert to an angle</param>
        /// <returns>The angle in radians</returns>
        public static float ToDegrees(this Vector2 vector)
        { return MathF.Atan2(vector.Y, vector.X) * MathConstants.RadiansToDegrees; }

        /// <summary>
        /// Converts angle in degrees to a Vector2 direction
        /// </summary>
        /// <param name="degrees">The angle in degees</param>
        /// <returns>A vector2 direction from the angle provided</returns>
        public static Vector2 DegreesToVector2(this float degrees)
        {
            var radians = MathConstants.ToRadians(degrees);
            return new Vector2(MathF.Cos(radians), MathF.Sin(radians));
        }

        /// <summary>
        /// Converts radians to a Vector2 direction
        /// </summary>
        /// <param name="radians">The rotation in radians</param>
        /// <returns>A vector2 direction from the rotation provided</returns>
        public static Vector2 RadiansToVector2(this float radians)
        { return new Vector2(MathF.Cos(radians), MathF.Sin(radians)); }
        
        /// <summary>
        /// Gets the angle towards a destination vector
        /// </summary>
        /// <param name="source">The source position</param>
        /// <param name="destination">The destination to look at</param>
        /// <returns>Returns the angle (in degrees) needed to look at the given destination position</returns>
        public static float GetAngle(this Vector2 source, Vector2 destination)
        { return (destination - source).ToDegrees(); }
        
        /// <summary>
        /// Moves towards a given destination but at a given rate
        /// </summary>
        /// <param name="source">Source position</param>
        /// <param name="destination">Destination position</param>
        /// <param name="maxDistanceDelta">Distance to travel</param>
        /// <returns>The Vector2 representing the new position towards the destination</returns>
        /// <remarks>A Lerp moves at a percentage so you move more further away, and less as you get closer, this is constant as it moves on units</remarks>
        public static Vector2 MoveTowards(this Vector2 source, Vector2 destination, float maxDistanceDelta)
        {
            var toX = destination.X - source.X;
            var toY = destination.Y - source.Y;
            var sqDist = toX * toX + toY * toY;

            if (sqDist == 0 || (maxDistanceDelta >= 0 && sqDist <= maxDistanceDelta * maxDistanceDelta))
            { return destination; }

            var dist = MathF.Sqrt(sqDist);
            var resultX = source.X + toX / dist * maxDistanceDelta;
            var resultY = source.Y + toY / dist * maxDistanceDelta;
            return new Vector2(resultX, resultY);
        }
    }
}