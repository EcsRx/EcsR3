using System.Collections;
using System.Collections.Generic;
using CommunityToolkit.HighPerformance;

namespace EcsR3.Plugins.UtilityAI.Utility
{
    /// <summary>
    /// Provides a way to track scores in a 2d grid, like a heightmap of sorts
    /// </summary>
    /// <remarks>
    /// Heatmaps provide an simple/efficient way to gauge a score of something in a given area,
    /// for example if you had a 10x10 grid and you wanted to gauge how dangerous each cell is
    /// you could add together all the enemies in each grid cell.
    ///
    /// This can then be used when scoring considerations/advice/actions without needing to know
    /// about each individual stiumulai in cell.
    ///
    /// There is a PhysicalHeatmap which works at a higher level providing virtualised world coordinate translation
    /// if you need to have a heatmap in a physical space but have its resolution scaled, i.e a 1000x1000 area being
    /// split up into 10x10 chunks for a gauge on how well a battle is going etc.
    /// </remarks>
    public class Heatmap : IEnumerable<HeatmapPosition>
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        protected float[,] Scores { get; }
        
        /// <summary>
        /// Provide the width/height you want to track scores for
        /// </summary>
        /// <param name="width">The width of the heatmap</param>
        /// <param name="height">The height of the heatmap</param>
        public Heatmap(int width, int height)
        {
            Width = width;
            Height = height;
            Scores = new float[width, height];
        }

        public void Reset()
        { Scores.AsSpan2D().Fill(0); }

        private int ClampWidthValue(int x)
        {
            if(x >= Width) { return Width-1; }
            return x < 0 ? 0 : x;
        }

        private int ClampHeightValue(int y)
        {
            if(y >= Height) { return Height-1; }
            return y < 0 ? 0 : y;
        }

        public void AddValue(int x, int y, float value)
        {
            x = ClampWidthValue(x);
            y = ClampHeightValue(y);
            Scores[x, y] += value;
        }
        
        public void DeductValue(int x, int y, float value)
        {
            x = ClampWidthValue(x);
            y = ClampHeightValue(y);
            Scores[x, y] -= value;
        }
        
        public void SetValue(int x, int y, float value)
        {
            x = ClampWidthValue(x);
            y = ClampHeightValue(y);
            Scores[x, y] = value;
        }
        
        public float GetScore(int x, int y)
        {
            x = ClampWidthValue(x);
            y = ClampHeightValue(y);
            return Scores[x, y];
        }
        
        public HeatmapPosition GetScoreWithPosition(int x, int y)
        {
            x = ClampWidthValue(x);
            y = ClampHeightValue(y);
            var score = Scores[x, y];
            return new HeatmapPosition(x, y, score);
        }

        public IEnumerator<HeatmapPosition> GetEnumerator()
        {
            for (var x = 0; x < Scores.GetLength(0); x++)
            {
                for (var y = 0; y < Scores.GetLength(1); y++)
                { yield return new HeatmapPosition(x, y, Scores[x, y]); }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }
    }
}