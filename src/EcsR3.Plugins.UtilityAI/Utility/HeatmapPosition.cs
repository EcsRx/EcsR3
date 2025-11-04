namespace EcsR3.Plugins.UtilityAI.Utility
{
    public struct HeatmapPosition
    {
        public int X, Y;
        public float Score;

        public HeatmapPosition(int x, int y, float score)
        {
            X = x;
            Y = y;
            Score = score;
        }
    }
}