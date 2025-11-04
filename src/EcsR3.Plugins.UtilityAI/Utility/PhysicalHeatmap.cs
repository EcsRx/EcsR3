using System;
using System.Collections;
using System.Collections.Generic;

namespace EcsR3.Plugins.UtilityAI.Utility
{
    /// <summary>
    /// An implementation of heatmap which takes into account physical positions and scaling
    /// </summary>
    /// <remarks>
    /// This is useful for when you want to map an x/y area into a smaller heatmap representation,
    /// i.e a 1000x1000 can be stored as a more efficient 10x10 heatmap etc
    /// </remarks>
    public class PhysicalHeatmap : IEnumerable<HeatmapPosition>
    {
        /// <summary>
        /// How much the physical space is scaled by (0-1)
        /// </summary>
        public float Scaling { get; }
        
        /// <summary>
        /// The physical width of the space
        /// </summary>
        public float Width { get; }
        
        /// <summary>
        /// The physical height of the space
        /// </summary>
        public float Height { get; }
        
        /// <summary>
        /// The position of the heatmap x in the physical space (acts as an offset for incoming x values)
        /// </summary>
        public float XPosition { get; }
        
        /// <summary>
        /// The position of the heatmap y in physical space (acts as an offset for incoming y values)
        /// </summary>
        public float YPosition { get; }
        
        public float CellWidth, CellHeight;
        
        /// <summary>
        /// Internal scaled heatmap which actually tracks scores etc
        /// </summary>
        public Heatmap Heatmap { get; private set; }
        
        /// <summary>
        /// Sets up the internal heatmap with the scaling/position offsets required
        /// </summary>
        /// <param name="width">The physical width required (this is scaled to an internal scaled width)</param>
        /// <param name="height">The physical height required (this is scaled to an internal scaled height)</param>
        /// <param name="scale">The scaling factor to apply to the physical dimensions (1.0f being no scaling 0.1f being 1/10 scale, i.e 100x100 physical with 0.1f scaling would become 10x10), defaults to 0.1f (1/10th)</param>
        /// <param name="xPosition">Optional x position offset in the physical dimension (this is the top left of the heatmap)</param>
        /// <param name="yPosition">Optional y position offset in the physicam dimension (this is the top left of the heatmap)</param>
        /// <remarks>
        /// This acts as a virtual layer over an internal heatmap to allow you to use native physical coordinates and have it apply to a scaled down heatmap.
        ///
        /// For example if you had an arena at coordinates Vector2(50, 50) and it had a width of 100,100 then in reality you have cells from 50-150 x/y physical world,
        /// if you were to scale it by 0.1f then you would end up with the 100x100 becoming 10x10 in memory, and position 50,50 would be cell 0,0 and position 150,150
        /// would be the maximum 10,10 cell. However if you were to scale by 0.5f you would have a 50x50 internal grid, or if there was no position offset the world
        /// coords would start at 0,0 not 50,50 and end at 100,100 not 150,150.
        /// </remarks>
        public PhysicalHeatmap(int width, int height, float scale = 0.1f, float xPosition = 0.0f, float yPosition = 0.0f)
        {
            Scaling = scale;
            Width = width;
            Height = height;
            XPosition = xPosition;
            YPosition = yPosition;
            
            var internalWidth = (int)MathF.Round(width * Scaling);
            var internalHeight = (int)MathF.Round(height * Scaling);
            Heatmap = new Heatmap(internalWidth, internalHeight);

            CellWidth = Width / Heatmap.Width;
            CellHeight = Height / Heatmap.Height;
        }
        
        public int XPositionToIndex(float x)
        { return (int)MathF.Round((x - XPosition) * Scaling); }

        public float XIndexToPosition(int x)
        { return (x / Scaling) + XPosition; }
        
        public int YPositionToIndex(float y)
        { return (int)MathF.Round((y - YPosition) * Scaling); }
        
        public float YIndexToPosition(int y)
        { return (y / Scaling) + YPosition; }
        
        public float GetScore(float x, float y)
        {
            var scaledX = XPositionToIndex(x);
            var scaledY = YPositionToIndex(y);
            return Heatmap.GetScore(scaledX, scaledY);
        }
        
        public HeatmapPosition GetScoreWithScaledPosition(float x, float y)
        {
            var scaledX = XPositionToIndex(x);
            var scaledY = YPositionToIndex(y);
            var score = Heatmap.GetScore(scaledX, scaledY);
            return new HeatmapPosition(scaledX, scaledY, score);
        }

        public (int scaledX, int scaledY) ScalePosition(float x, float y)
        {
            var scaledX = XPositionToIndex(x);
            var scaledY = YPositionToIndex(y);
            return (scaledX, scaledY);
        }
        
        public void AddValue(float x, float y, float value)
        {
            var scaledX = XPositionToIndex(x);
            var scaledY = YPositionToIndex(y);
            Heatmap.AddValue(scaledX, scaledY, value);
        }
        
        public void DeductValue(float x, float y, float value)
        {
            var scaledX = XPositionToIndex(x);
            var scaledY = YPositionToIndex(y);
            Heatmap.DeductValue(scaledX, scaledY, value);
        }
        
        public void SetValue(float x, float y, float value)
        {
            var scaledX = XPositionToIndex(x);
            var scaledY = YPositionToIndex(y);
            Heatmap.SetValue(scaledX, scaledY, value);
        }

        public IEnumerator<HeatmapPosition> GetEnumerator()
        { return Heatmap.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }
    }
}