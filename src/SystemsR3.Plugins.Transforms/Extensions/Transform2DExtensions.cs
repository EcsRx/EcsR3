using System;
using System.Numerics;
using SystemsR3.Plugins.Transforms.Models;

namespace SystemsR3.Plugins.Transforms.Extensions
{
    public static class Transform2DExtensions
    {
        public static readonly float RadiansToDegrees = 180.0f / MathF.PI;
        public static readonly float DegreesToRadians = MathF.PI / 180.0f;
        public static readonly float Radian90Degrees = 90 * DegreesToRadians;
        
        /// <summary>
        /// Returns the transforms rotation but converted to degrees
        /// </summary>
        /// <param name="transform">The transform to operate on</param>
        /// <returns>The value of the rotation in degrees</returns>
        public static float RotationToDegrees(this Transform2D transform)
        { return transform.Rotation * RadiansToDegrees; }
        
        /// <summary>
        /// Converts the rotation in degrees into radians and sets the rotation for the transform
        /// </summary>
        /// <param name="transform">The transform to operate on</param>
        /// <param name="rotationAsDegrees">The rotation value in degrees to be converted and applied to the transform</param>
        public static void RotationFromDegrees(this Transform2D transform, float rotationAsDegrees)
        { transform.Rotation = rotationAsDegrees * DegreesToRadians; }

        /// <summary>
        /// Returns the forward direction of the transform as a Vector
        /// </summary>
        /// <param name="transform">The transform to operate on</param>
        /// <returns>The forward direction</returns>
        public static Vector2 Forward(this Transform2D transform)
        { return new Vector2(MathF.Sin(transform.Rotation), -MathF.Cos(transform.Rotation)); }

        /// <summary>
        /// Returns the right direction of the transform as a Vector
        /// </summary>
        /// <param name="transform">The transform to operate on</param>
        /// <returns>The right direction</returns>
        public static Vector2 Right(this Transform2D transform)
        {
            var newRotation = transform.Rotation + Radian90Degrees;
            return new Vector2(MathF.Sin(newRotation), -MathF.Cos(newRotation));
        }

        /// <summary>
        /// Generates the rotation needed to look at the given destination
        /// </summary>
        /// <param name="source">The transform to operate on</param>
        /// <param name="destination">The destination to look at</param>
        /// <param name="offsetInRadians">Optional offset applied to the calculation, 0 by default but different coordinate systems may need offsets applied</param>
        /// <returns>The rotation to look at the given position</returns>
        /// <remarks>It doesnt apply directly as you may want to lerp/slerp the value yourself before applying</remarks>
        public static float GetLookAt(this Transform2D source, Vector2 destination, float offsetInRadians = 0f)
        { return source.Position.GetAngleFor(destination, offsetInRadians); }
    }
}