using System.Numerics;

namespace SystemsR3.Plugins.Transforms.Models
{
    /// <summary>
    /// This represents a cross platform way of representing 3d transform concerns
    /// </summary>
    public class Transform
    {
        /// <summary>
        /// The position of the transform
        /// </summary>
        public Vector3 Position { get; set; } = Vector3.Zero;
        
        // The rotation of the transform
        public Quaternion Rotation { get; set; } = Quaternion.Identity;
        
        // The scale of the transform
        public Vector3 Scale { get; set; } = Vector3.One;
    }
}