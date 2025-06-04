using System;
using CommunityToolkit.HighPerformance;

namespace EcsR3.Helpers
{
    public static class ArrayHelper
    {
        public static void Resize2DArray_NoSpan<T>(ref T[,] original, int newD1Size, int newD2Size, T? fillNewWith = null) where T : struct
        {
            var newArray = new T[newD1Size, newD2Size];
            var d1Count = original.GetLength(0);
            var d2Count = original.GetLength(1);
            var d1UpperBounds = original.GetUpperBound(0);

            for (var columnIndex = 0; columnIndex <= d1UpperBounds; columnIndex++)
            {
                Array.Copy(original, columnIndex * d2Count, newArray, columnIndex * newD2Size, d2Count);
                if (fillNewWith is null) { continue; }
                
                for(var i=d2Count;i<newD2Size;i++)
                { newArray[columnIndex, i] = fillNewWith.Value; }
            }

            if (fillNewWith is null)
            {
                original = newArray;
                return;
            }

            for (var columnIndex = d1Count; columnIndex < newD1Size; columnIndex++)
            {
                for (var rowIndex = 0; rowIndex < newD2Size; rowIndex++)
                { newArray[columnIndex, rowIndex] = fillNewWith.Value; }
            }
            original = newArray;
        }
        
        public static void Resize2DArray<T>(ref T[,] original, int newD1Size, int newD2Size, T? fillNewWith = null) where T : struct
        {
            var newArray = new T[newD1Size, newD2Size];
            var d1Count = original.GetLength(0);
            var d2Count = original.GetLength(1);
            if (d1Count == 0 || d2Count == 0)
            {
                original = newArray;
                return;
            }
            
            var sourceSpan = new Span2D<T>(original);
            var targetSpan = new Span2D<T>(newArray);
            
            if (fillNewWith != null)
            { targetSpan.Fill(fillNewWith.Value); }

            var targetReplacement = targetSpan.Slice(0, 0, d1Count, d2Count);
            var sourceReplacement = sourceSpan.Slice(0, 0, d1Count, d2Count);
            sourceReplacement.CopyTo(targetReplacement);

            original = newArray;
        }
    }
}