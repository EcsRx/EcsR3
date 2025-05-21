using SystemsR3.Plugins.Transforms.Extensions;
using Xunit;

namespace SystemsR3.Tests.Plugins.Transforms;

public class MathConstantsTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1.0f, 0.0175f)]
    [InlineData(200.0f, 3.4907f)]
    [InlineData(800.0f, 13.9626f)]
    [InlineData(180.0f, 3.1416f)]
    [InlineData(-180.0f, -3.1416f)]
    public void should_correctly_convert_to_radians(float degrees, float expectedRadians)
    {
        var actualRadians = MathConstants.ToRadians(degrees);
        Assert.Equal(expectedRadians.ToString("F4"), actualRadians.ToString("F4"));
    }
    
    [Theory]
    [InlineData(0, 0)]
    [InlineData(0.0175f, 1.0027f)]
    [InlineData(3.4907f, 200.0024f)]
    [InlineData(13.9626f, 799.9980f )]
    [InlineData(3.1416f, 180.0004f)]
    [InlineData(-3.1416f, -180.0004f)]
    public void should_correctly_convert_to_degrees(float radians, float expectedDegrees)
    {
        var actualDegrees = MathConstants.ToDegrees(radians);
        Assert.Equal(expectedDegrees.ToString("F4"), actualDegrees.ToString("F4"));
    }
}