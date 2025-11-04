using System.Linq;
using EcsR3.Plugins.UtilityAI.Utility;
using Xunit;

namespace EcsR3.Tests.Plugins.UtilityAI;

public class PhysicalHeatmapTests
{
    [Fact]
    public void should_correctly_add_score()
    {
        var physicalWidth = 100;
        var physicalHeight = 100;
        var scaling = 0.1f;
        var heatmap = new PhysicalHeatmap(physicalWidth, physicalHeight, scaling);
        
        heatmap.AddValue(50, 50, 3);
        heatmap.AddValue(50, 50, 2);

        var result = heatmap.GetScore(50, 50);
        Assert.Equal(5, result);
        Assert.Equal(99, heatmap.Count(x => x.Score == 0));

        var resultWithPosition = heatmap.GetScoreWithScaledPosition(50, 50);
        Assert.Equal(5, resultWithPosition.Score);
        Assert.Equal(5, resultWithPosition.X);
        Assert.Equal(5, resultWithPosition.Y);
    }
        
    [Fact]
    public void should_correctly_deduct_score()
    {
        var physicalWidth = 100;
        var physicalHeight = 100;
        var scaling = 0.1f;
        var heatmap = new PhysicalHeatmap(physicalWidth, physicalHeight, scaling);
        
        heatmap.DeductValue(50, 50, 3);
        heatmap.DeductValue(50, 50, 2);

        var result = heatmap.GetScore(50, 50);
        Assert.Equal(-5, result);
        Assert.Equal(99, heatmap.Count(x => x.Score == 0));
        
        var resultWithPosition = heatmap.GetScoreWithScaledPosition(50, 50);
        Assert.Equal(-5, resultWithPosition.Score);
        Assert.Equal(5, resultWithPosition.X);
        Assert.Equal(5, resultWithPosition.Y);
    }
    
    [Fact]
    public void should_correctly_set_score()
    {
        var physicalWidth = 100;
        var physicalHeight = 100;
        var scaling = 0.1f;
        var heatmap = new PhysicalHeatmap(physicalWidth, physicalHeight, scaling);
        
        heatmap.SetValue(50, 50, 3);

        var result = heatmap.GetScore(50, 50);
        Assert.Equal(3, result);
        Assert.Equal(99, heatmap.Count(x => x.Score == 0));
        
        var resultWithPosition = heatmap.GetScoreWithScaledPosition(50, 50);
        Assert.Equal(3, resultWithPosition.Score);
        Assert.Equal(5, resultWithPosition.X);
        Assert.Equal(5, resultWithPosition.Y);
    }
    
    [Fact]
    public void should_correctly_add_score_with_offset()
    {
        var physicalWidth = 100;
        var physicalHeight = 100;
        var scaling = 0.1f;
        var heatmap = new PhysicalHeatmap(physicalWidth, physicalHeight, scaling, 50, 50);
        
        heatmap.AddValue(100, 100, 3);
        heatmap.AddValue(100, 100, 2);

        var result = heatmap.GetScore(100, 100);
        Assert.Equal(5, result);
        Assert.Equal(99, heatmap.Count(x => x.Score == 0));
        
        var resultWithPosition = heatmap.GetScoreWithScaledPosition(100, 100);
        Assert.Equal(5, resultWithPosition.Score);
        Assert.Equal(5, resultWithPosition.X);
        Assert.Equal(5, resultWithPosition.Y);
    }
        
    [Fact]
    public void should_correctly_deduct_score_with_offset()
    {
        var physicalWidth = 100;
        var physicalHeight = 100;
        var scaling = 0.1f;
        var heatmap = new PhysicalHeatmap(physicalWidth, physicalHeight, scaling, 50, 50);
        
        heatmap.DeductValue(100, 100, 3);
        heatmap.DeductValue(100, 100, 2);

        var result = heatmap.GetScore(100, 100);
        Assert.Equal(-5, result);
        Assert.Equal(99, heatmap.Count(x => x.Score == 0));
        
        var resultWithPosition = heatmap.GetScoreWithScaledPosition(100, 100);
        Assert.Equal(-5, resultWithPosition.Score);
        Assert.Equal(5, resultWithPosition.X);
        Assert.Equal(5, resultWithPosition.Y);
    }
    
    [Fact]
    public void should_correctly_set_score_with_offset()
    {
        var physicalWidth = 100;
        var physicalHeight = 100;
        var scaling = 0.1f;
        var heatmap = new PhysicalHeatmap(physicalWidth, physicalHeight, scaling, 50, 50);
        
        heatmap.SetValue(100, 100, 3);

        var result = heatmap.GetScore(100, 100);
        Assert.Equal(3, result);
        Assert.Equal(99, heatmap.Count(x => x.Score == 0));
        
        var resultWithPosition = heatmap.GetScoreWithScaledPosition(100, 100);
        Assert.Equal(3, resultWithPosition.Score);
        Assert.Equal(5, resultWithPosition.X);
        Assert.Equal(5, resultWithPosition.Y);
    }
}