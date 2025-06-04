using EcsR3.Helpers;
using Xunit;

namespace EcsR3.Tests.EcsR3.Helpers;

public class ArrayHelperTests
{
    [Fact]
    public void should_correctly_resize_multidimensional_array_without_spans()
    {
        var actual = new int[2, 3]
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };
        
        var expected = new int[3,4]
        {
            {1, 2, 3, 0 }, 
            {4, 5, 6, 0}, 
            {0, 0, 0, 0}
        };

        ArrayHelper.Resize2DArray_NoSpan(ref actual, 3, 4);
        
        Assert.Equal(3, actual.GetLength(0));
        Assert.Equal(4, actual.GetLength(1));
        
        for (var x = 0; x < 3; x++)
        {
            for(var y =0;y<4;y++)
            {
                Assert.Equal(expected[x,y], actual[x,y]);
            }
        }
    }
    
    [Fact]
    public void should_correctly_resize_multidimensional_array_with_default_values_without_spans()
    {
        var actual = new int[2, 3]
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };
        
        var expected = new int[3,4]
        {
            {1, 2, 3, -1 }, 
            {4, 5, 6, -1}, 
            {-1, -1, -1, -1}
        };

        ArrayHelper.Resize2DArray_NoSpan(ref actual, 3, 4, -1);
        
        Assert.Equal(3, actual.GetLength(0));
        Assert.Equal(4, actual.GetLength(1));
        
        for (var x = 0; x < 3; x++)
        {
            for(var y =0;y<4;y++)
            {
                Assert.Equal(expected[x,y], actual[x,y]);
            }
        }
    }
    
    [Fact]
    public void should_correctly_resize_multidimensional_array()
    {
        var actual = new int[2, 3]
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };
        
        var expected = new int[3,4]
        {
            {1, 2, 3, 0 }, 
            {4, 5, 6, 0}, 
            {0, 0, 0, 0}
        };

        ArrayHelper.Resize2DArray(ref actual, 3, 4);
        
        Assert.Equal(3, actual.GetLength(0));
        Assert.Equal(4, actual.GetLength(1));
        
        for (var x = 0; x < 3; x++)
        {
            for(var y =0;y<4;y++)
            {
                Assert.Equal(expected[x,y], actual[x,y]);
            }
        }
    }
    
    [Fact]
    public void should_correctly_resize_multidimensional_array_with_default_values()
    {
        var actual = new int[2, 3]
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };
        
        var expected = new int[3,4]
        {
            {1, 2, 3, -1}, 
            {4, 5, 6, -1}, 
            {-1, -1, -1, -1}
        };

        ArrayHelper.Resize2DArray(ref actual, 3, 4, -1);
        
        Assert.Equal(3, actual.GetLength(0));
        Assert.Equal(4, actual.GetLength(1));
        
        for (var x = 0; x < 3; x++)
        {
            for(var y =0;y<4;y++)
            {
                Assert.Equal(expected[x,y], actual[x,y]);
            }
        }
    }
}