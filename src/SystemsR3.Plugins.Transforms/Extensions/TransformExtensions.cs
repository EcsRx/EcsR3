using System;
using System.Numerics;
using SystemsR3.Plugins.Transforms.Models;

namespace SystemsR3.Plugins.Transforms.Extensions
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Returns the forward direction of the transform as a Vector
        /// </summary>
        /// <param name="transform">The transform to operate on</param>
        /// <returns>The forward direction</returns>
        public static Vector3 Forward(this Transform transform)
        { return Vector3.Transform(Vector3.UnitZ, transform.Rotation); }
        
        /// <summary>
        /// Returns the up direction of the transform as a Vector
        /// </summary>
        /// <param name="transform">The transform to operate on</param>
        /// <returns>The up direction</returns>
        public static Vector3 Up(this Transform transform)
        { return Vector3.Transform(Vector3.UnitY, transform.Rotation); }
        
        /// <summary>
        /// Returns the right direction of the transform as a Vector
        /// </summary>
        /// <param name="transform">The transform to operate on</param>
        /// <returns>The right direction</returns>
        public static Vector3 Right(this Transform transform)
        { return Vector3.Transform(Vector3.UnitX, transform.Rotation); }
        
        /// <summary>
        /// Generates the rotation needed to look at the given destination
        /// </summary>
        /// <param name="source">The transform to operate on</param>
        /// <param name="destination">The destination to look at</param>
        /// <param name="upAxis">The up axis</param>
        /// <returns>The rotation to look at the given position</returns>
        /// <remarks>It doesnt apply directly as you may want to lerp/slerp the value yourself before applying</remarks>
        public static Quaternion GetLookAt(this Transform source, Vector3 destination, Vector3 upAxis)
        {
            var direction = Vector3.Normalize(destination - source.Position);
            var forward = Forward(source);
            var dot = Vector3.Dot(forward, direction);

            if (MathF.Abs(dot - (-1.0f)) < 0.000001f)
            { return new Quaternion(upAxis, MathF.PI); }
            if (Math.Abs(dot - (1.0f)) < 0.000001f)
            { return Quaternion.Identity; }

            var rotAngle = MathF.Acos(dot);
            var rotAxis = Vector3.Cross(forward, direction);
            rotAxis = Vector3.Normalize(rotAxis);
            return Quaternion.CreateFromAxisAngle(rotAxis, rotAngle);
        }
        
        /// <summary>
        /// Generates the rotation needed to look at the given destination
        /// </summary>
        /// <param name="source">The transform to operate on</param>
        /// <param name="destination">The destination to look at</param>
        /// <returns>The rotation to look at the given position</returns>
        /// <remarks>It doesnt apply directly as you may want to lerp/slerp the value yourself before applying</remarks>
        public static Quaternion GetLookAt(this Transform source, Vector3 destination)
        { return GetLookAt(source, destination, Vector3.UnitY); }
    }
}