using System.Numerics;

namespace SystemsR3.Plugins.Transforms.Models
{
    /// <summary>
    /// This represents a cross platform way of representing 2d transform concerns
    /// </summary>
    public class Transform2D
    {
        /// <summary>
        /// The position of the transform
        /// </summary>
        public Vector2 Position { get; set; } = Vector2.Zero;
        
        /// <summary>
        /// This is the rotation of the transform in radians
        /// </summary>
        public float Rotation { get; set; } = 0.0f;
        
        /// <summary>
        /// The scale of the transform
        /// </summary>
        public Vector2 Scale { get; set; } = Vector2.One;
    }
}