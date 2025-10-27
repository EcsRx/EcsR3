namespace EcsR3.Plugins.UtilityAI.Keys
{
    public readonly struct ConsiderationKeyWithScore
    {
        public readonly ConsiderationKey ConsiderationKey;
        public readonly float Score;

        public ConsiderationKeyWithScore(ConsiderationKey considerationKey, float score)
        {
            ConsiderationKey = considerationKey;
            Score = score;
        }
    }
}