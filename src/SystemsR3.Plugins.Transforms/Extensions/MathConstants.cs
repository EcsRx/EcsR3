using System;

namespace SystemsR3.Plugins.Transforms.Extensions
{
    public static class MathConstants
    {
        /// <summary>
        /// Constant for converting degrees to radians
        /// </summary>
        public static readonly float RadiansToDegrees = 180.0f/MathF.PI;
        
        /// <summary>
        /// Constant for converting radians to degrees
        /// </summary>
        public static readonly float DegreesToRadians = MathF.PI/180.0f;
        
        /// <summary>
        /// Helper for converting radians to degrees
        /// </summary>
        /// <param name="radians">Input radians</param>
        /// <returns>Output degrees</returns>
        public static float ToDegrees(float radians)
        { return radians * RadiansToDegrees; }
        
        /// <summary>
        /// Helper for converting degrees to radians
        /// </summary>
        /// <param name="radians">Input degrees</param>
        /// <returns>Output radians</returns>
        public static float ToRadians(float degrees)
        { return degrees * DegreesToRadians; }
    }
}