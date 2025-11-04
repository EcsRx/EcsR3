using System.Linq;
using EcsR3.Plugins.UtilityAI.Utility;
using Xunit;

namespace EcsR3.Tests.Plugins.UtilityAI;

public class HeatmapTests
{
    [Fact]
    public void should_correctly_add_score()
    {
        var width = 10;
        var height = 10;
        var heatmap = new Heatmap(width, height);
        
        heatmap.AddValue(5, 5, 3);
        heatmap.AddValue(5, 5, 2);

        var result = heatmap.GetScore(5, 5);
        Assert.Equal(5, result);
        Assert.Equal(99, heatmap.Count(x => x.Score == 0));
        
        var resultWithPosition = heatmap.GetScoreWithPosition(5, 5);
        Assert.Equal(5, resultWithPosition.Score);
        Assert.Equal(5, resultWithPosition.X);
        Assert.Equal(5, resultWithPosition.Y);
    }
    
    [Fact]
    public void should_correctly_deduct_score()
    {
        var width = 10;
        var height = 10;
        var heatmap = new Heatmap(width, height);
        
        heatmap.DeductValue(5, 5, 3);
        heatmap.DeductValue(5, 5, 2);

        var result = heatmap.GetScore(5, 5);
        Assert.Equal(-5, result);
        Assert.Equal(99, heatmap.Count(x => x.Score == 0));
        
        var resultWithPosition = heatmap.GetScoreWithPosition(5, 5);
        Assert.Equal(-5, resultWithPosition.Score);
        Assert.Equal(5, resultWithPosition.X);
        Assert.Equal(5, resultWithPosition.Y);
    }
    
    [Fact]
    public void should_correctly_set_score()
    {
        var width = 10;
        var height = 10;
        var heatmap = new Heatmap(width, height);
        
        heatmap.SetValue(5, 5, 3);

        var result = heatmap.GetScore(5, 5);
        Assert.Equal(3, result);
        Assert.Equal(99, heatmap.Count(x => x.Score == 0));
        
        var resultWithPosition = heatmap.GetScoreWithPosition(5, 5);
        Assert.Equal(3, resultWithPosition.Score);
        Assert.Equal(5, resultWithPosition.X);
        Assert.Equal(5, resultWithPosition.Y);
    }
}