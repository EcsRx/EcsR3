using System.Numerics;
using SystemsR3.Plugins.Transforms.Models;

namespace SystemsR3.Plugins.Transforms.Extensions
{
    public static class Transform2DExtensions
    {
        /// <summary>
        /// Returns the transforms rotation but converted to degrees
        /// </summary>
        /// <param name="transform">The transform to operate on</param>
        /// <returns>The value of the rotation in degrees</returns>
        public static float RotationToDegrees(this Transform2D transform)
        { return transform.Rotation * MathConstants.RadiansToDegrees; }
        
        /// <summary>
        /// Converts the rotation in degrees into radians and sets the rotation for the transform
        /// </summary>
        /// <param name="transform">The transform to operate on</param>
        /// <param name="rotationAsDegrees">The rotation value in radians to be converted and applied to the transform</param>
        public static void RotationFromDegrees(this Transform2D transform, float rotationAsDegrees)
        { transform.Rotation = rotationAsDegrees * MathConstants.DegreesToRadians; }

        /// <summary>
        /// Returns the forward direction of the transform as a Vector (assumes rotation in degrees)
        /// </summary>
        /// <param name="transform">The transform to operate on</param>
        /// <returns>The forward direction in radians</returns>
        public static Vector2 Forward(this Transform2D transform)
        { return transform.Rotation.RadiansToVector2(); }

        /// <summary>
        /// Returns the right direction of the transform as a Vector
        /// </summary>
        /// <param name="transform">The transform to operate on</param>
        /// <returns>The right direction in radians</returns>
        public static Vector2 Right(this Transform2D transform)
        {
            var newRotation = transform.Rotation + (90 * MathConstants.DegreesToRadians);
            return newRotation.RadiansToVector2();
        }

        /// <summary>
        /// Generates the rotation needed to look at the given destination in radians
        /// </summary>
        /// <param name="source">The transform to operate on</param>
        /// <param name="destination">The destination to look at</param>
        /// <returns>The rotation in radians to look at the given position</returns>
        /// <remarks>It doesnt apply directly as you may want to lerp/slerp the value yourself before applying</remarks>
        public static float GetLookAt(this Transform2D source, Vector2 destination)
        { return source.Position.GetAngle(destination) * MathConstants.DegreesToRadians; }
        
        /// <summary>
        /// Sets the rotation to look at the given destination
        /// </summary>
        /// <param name="source">The transform to operate on</param>
        /// <param name="destination">The destination to look at</param>
        /// <remarks>It applies the value internally</remarks>
        public static void LookAt(this Transform2D source, Vector2 destination)
        { source.Rotation = GetLookAt(source, destination); }
    }
}