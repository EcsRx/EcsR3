using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Plugins.UtilityAI.Utility;

namespace EcsR3.Plugins.UtilityAI.Extensions
{
    public static class HeatmapExtensions
    {
        /// <summary>
        /// Allows you to get highest scoring cell near an x/y index
        /// </summary>
        /// <param name="heatmap">The heatmap to scan</param>
        /// <param name="xIndex">The central x index</param>
        /// <param name="yIndex">The central y index</param>
        /// <param name="scanDistance">The distance to scan around the central position</param>
        /// <returns>A heatmap position of the highest scoring cell nearby</returns>
        public static HeatmapPosition GetHighestScoringAround(this Heatmap heatmap, int xIndex, int yIndex, int scanDistance = 1)
        {
            var highest = heatmap.GetScoreWithPosition(xIndex, yIndex);
            for (var x = -scanDistance; x <= scanDistance; x++)
            {
                if (x < 0 || x >= heatmap.Width) { continue; }
                for (var y = -scanDistance; y <= scanDistance; y++)
                {
                    if (y < 0 || y >= heatmap.Height) { continue; }
                    var checkX = xIndex + x;
                    var checkY = yIndex + y;
                    var score = heatmap.GetScore(checkX, checkY);
                    
                    if(score > highest.Score)
                    { highest = new HeatmapPosition(checkX, checkY, score); }
                }
            }
            return highest;
        }
        
        /// <summary>
        /// Allows you to get lowest scoring cell near an x/y index
        /// </summary>
        /// <param name="heatmap">The heatmap to scan</param>
        /// <param name="xIndex">The central x index</param>
        /// <param name="yIndex">The central y index</param>
        /// <param name="scanDistance">The distance to scan around the central position</param>
        /// <returns>A heatmap position of the lowest scoring cell nearby</returns>
        public static HeatmapPosition GetLowestScoringAround(this Heatmap heatmap, int xIndex, int yIndex, int scanDistance = 1)
        {
            var lowest = heatmap.GetScoreWithPosition(xIndex, yIndex);
            for (var x = -scanDistance; x <= scanDistance; x++)
            {
                if (x < 0 || x >= heatmap.Width) { continue; }
                for (var y = -scanDistance; y <= scanDistance; y++)
                {
                    if (y < 0 || y >= heatmap.Height) { continue; }
                    var checkX = xIndex + x;
                    var checkY = yIndex + y;
                    var score = heatmap.GetScore(checkX, checkY);
                    
                    if(score < lowest.Score)
                    { lowest = new HeatmapPosition(checkX, checkY, score); }
                }
            }
            return lowest;
        }
        
        /// <summary>
        /// Gets the average score around an area in the heatmap
        /// </summary>
        /// <param name="heatmap">The heatmap to scan</param>
        /// <param name="xIndex">The central x index</param>
        /// <param name="yIndex">The central y index</param>
        /// <param name="scanDistance">The distance to scan around the central position</param>
        /// <returns>The average score of the area scanned</returns>
        public static float GetAverageScoreAround(this Heatmap heatmap, int xIndex, int yIndex, int scanDistance = 1)
        {
            return heatmap.GetCellsAround(xIndex, yIndex, scanDistance)
                .Average(x => x.Score);
        }
                
        /// <summary>
        /// Allows you to get cell scores near an x/y index
        /// </summary>
        /// <param name="heatmap">The heatmap to scan</param>
        /// <param name="xIndex">The central x index</param>
        /// <param name="yIndex">The central y index</param>
        /// <param name="scanDistance">The distance to scan around the central position</param>
        /// <returns>An enumerable of all heatmap cells around that position</returns>
        public static IEnumerable<HeatmapPosition> GetCellsAround(this Heatmap heatmap, int xIndex, int yIndex, int scanDistance = 1)
        {
            for (var x = -scanDistance; x <= scanDistance; x++)
            {
                if (x < 0 || x >= heatmap.Width) { continue; }
                for (var y = -scanDistance; y <= scanDistance; y++)
                {
                    if (y < 0 || y >= heatmap.Height) { continue; }
                    var checkX = xIndex + x;
                    var checkY = yIndex + y;
                    var score = heatmap.GetScore(checkX, checkY);
                    yield return new HeatmapPosition(checkX, checkY, score);
                }
            }
        }
        
        /// <summary>
        /// Allows you to get highest scoring cell near an x/y position
        /// </summary>
        /// <param name="heatmap">The heatmap to scan</param>
        /// <param name="xPosition">The central x position</param>
        /// <param name="yPosition">The central y position</param>
        /// <param name="scanDistance">The distance to scan around the central position</param>
        /// <returns>A heatmap position of the highest scoring cell nearby</returns>
        public static HeatmapPosition GetHighestScoringAround(this PhysicalHeatmap heatmap, float xPosition, float yPosition, int scanDistance = 1)
        {
            var xIndex = heatmap.XPositionToIndex(xPosition);
            var yIndex = heatmap.YPositionToIndex(yPosition);
            return heatmap.Heatmap.GetHighestScoringAround(xIndex, yIndex, scanDistance);
        }
        
        /// <summary>
        /// Allows you to get lowest scoring cell near an x/y position
        /// </summary>
        /// <param name="heatmap">The heatmap to scan</param>
        /// <param name="xPosition">The central x position</param>
        /// <param name="yPosition">The central y position</param>
        /// <param name="scanDistance">The distance to scan around the central position</param>
        /// <returns>A heatmap position of the lowest scoring cell nearby</returns>
        public static HeatmapPosition GetLowestScoringAround(this PhysicalHeatmap heatmap, float xPosition, float yPosition, int scanDistance = 1)
        {
            var xIndex = heatmap.XPositionToIndex(xPosition);
            var yIndex = heatmap.YPositionToIndex(yPosition);
            return heatmap.Heatmap.GetLowestScoringAround(xIndex, yIndex, scanDistance);
        }
        
        /// <summary>
        /// Allows you to get cell scores near an x/y position
        /// </summary>
        /// <param name="heatmap">The heatmap to scan</param>
        /// <param name="xPosition">The central x position</param>
        /// <param name="yPosition">The central y position</param>
        /// <param name="scanDistance">The distance to scan around the central position</param>
        /// <returns>An enumerable of all heatmap cells around that position</returns>        
        public static IEnumerable<HeatmapPosition> GetCellsAround(this PhysicalHeatmap heatmap, float xPosition, float yPosition, int scanDistance = 1)
        {
            var xIndex = heatmap.XPositionToIndex(xPosition);
            var yIndex = heatmap.YPositionToIndex(yPosition);
            return heatmap.Heatmap.GetCellsAround(xIndex, yIndex, scanDistance);
        }
        
        /// <summary>
        /// Gets the average score around an area in the heatmap
        /// </summary>
        /// <param name="heatmap">The heatmap to scan</param>
        /// <param name="xPosition">The central x index</param>
        /// <param name="yPosition">The central y index</param>
        /// <param name="scanDistance">The distance to scan around the central position</param>
        /// <returns>The average score of the area scanned</returns>
        public static float GetAverageScoreAround(this PhysicalHeatmap heatmap, float xPosition, float yPosition, int scanDistance = 1)
        {
            return heatmap.GetCellsAround(xPosition, yPosition, scanDistance)
                .Average(x => x.Score);
        }
        
        /// <summary>
        /// Provides the centered physical position of the x index
        /// </summary>
        /// <param name="heatmap">The heatmap to use</param>
        /// <param name="xIndex">The x index to get the centralised position for</param>
        /// <returns>The physical x position for the cell at that index offset to its central position</returns>
        public static float XIndexToCenteredPosition(this PhysicalHeatmap heatmap, int xIndex)
        { return heatmap.XIndexToPosition(xIndex) + heatmap.CellWidth/2; }
        
        /// <summary>
        /// Provides the centered physical position of the x index
        /// </summary>
        /// <param name="heatmap">The heatmap to use</param>
        /// <param name="yIndex">The y index to get the centralised position for</param>
        /// <returns>The physical y position for the cell at that index offset to its central position</returns>
        public static float YIndexToCenteredPosition(this PhysicalHeatmap heatmap, int yIndex)
        { return heatmap.YIndexToPosition(yIndex) + heatmap.CellHeight/2; }
    }
}