using System.Numerics;
using SystemsR3.Plugins.Transforms.Extensions;
using Xunit;

namespace SystemsR3.Tests.Plugins.Transforms;

public class Vector2ExtensionTests
{
    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 1, 0.7854f)]
    [InlineData(20, 20, 0.7854f)]
    [InlineData(-20, -20, -2.3562f)]
    [InlineData(0, 1, 1.5708f)]
    public void should_correctly_convert_to_radians(float x, float y, float expectedRadians)
    {
        var actualRadians = new Vector2(x,y).ToRadians();
        Assert.Equal(expectedRadians.ToString("F4"), actualRadians.ToString("F4"));
    }
    
    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 1, 45f)]
    [InlineData(20, 20, 45f)]
    [InlineData(-20, -20, -135f)]
    [InlineData(0, 1, 90f)]
    public void should_correctly_convert_to_degrees(float x, float y, float expectedDegrees)
    {
        var actualDegrees = new Vector2(x,y).ToDegrees();
        Assert.Equal(expectedDegrees.ToString("F4"), actualDegrees.ToString("F4"));
    }
        
    [Theory]
    [InlineData(0, 1.0f, 0)]
    [InlineData(0.1f, 0.9950f, 0.0998f)]
    [InlineData(0.543f, 0.8562f, 0.5167f)]
    [InlineData(1.2f, 0.3624f, 0.9320f)]
    [InlineData(-1.2f, 0.3624f, -0.9320f)]
    [InlineData(321f, 0.8486f, 0.5291f)]
    public void should_correctly_convert_from_radians_to_vector2(float radians, float expectedX, float expectedY)
    {
        var actualVector = radians.RadiansToVector2();
        Assert.Equal(expectedX.ToString("F4"), actualVector.X.ToString("F4"));
        Assert.Equal(expectedY.ToString("F4"), actualVector.Y.ToString("F4"));
    }
    
    [Theory]
    [InlineData(0, 1.0f, 0f)]
    [InlineData(45f, 0.7071f, 0.7071f)]
    [InlineData(-50f, 0.6428f, -0.7660f)]
    [InlineData(410f, 0.6428f, 0.7660f)]
    [InlineData(163f, -0.9563f, 0.2924f)]
    [InlineData(0.2456f, 1, 0.0043f)]
    public void should_correctly_convert_from_degrees_to_vector2(float degrees, float expectedX, float expectedY)
    {
        var actualVector = degrees.DegreesToVector2();
        Assert.Equal(expectedX.ToString("F4"), actualVector.X.ToString("F4"));
        Assert.Equal(expectedY.ToString("F4"), actualVector.Y.ToString("F4"));
    }
    
    [Theory]
    [InlineData(0, 0, 0, 0, 0)]
    [InlineData(1, 0, 1, 1, 90)]
    [InlineData(1, 1, 1, 1, 0)]
    [InlineData(0, 0, 1, 1, 45)]
    [InlineData(-1, -1, 1, 1, 45)]
    [InlineData(0.24, -2.5f, -0.65f, 0, 109.5957f)]
    public void should_correctly_get_angle_between_vectors(float sourceX, float sourceY, float destinationX, float destinationY, float expectedAngle)
    {
        var v1 = new Vector2(sourceX, sourceY);
        var v2 = new Vector2(destinationX, destinationY);
        var actualAngle = v1.GetAngle(v2);
        Assert.Equal(expectedAngle.ToString("F4"), actualAngle.ToString("F4"));
    }
    
    [Theory]
    [InlineData(0, 0, 0, 0, 0, 0, 0)]
    [InlineData(0, 0, 1, 1, 0.1f, 0.0707f, 0.0707f)]
    [InlineData(0, 0, 100, 100, 1.0f, 0.7071f, 0.7071f)]
    [InlineData(-100, 0, 100, 100, 1.0f, -99.1056f, 0.4472f)]
    public void should_correctly_move_towards_vector(float sourceX, float sourceY, float destinationX, float destinationY, float movementSpeed, float expectedX, float expectedY)
    {
        var v1 = new Vector2(sourceX, sourceY);
        var v2 = new Vector2(destinationX, destinationY);
        var actualPosition = v1.MoveTowards(v2, movementSpeed);
        Assert.Equal(expectedX.ToString("F4"), actualPosition.X.ToString("F4"));
        Assert.Equal(expectedY.ToString("F4"), actualPosition.Y.ToString("F4"));
    }
}