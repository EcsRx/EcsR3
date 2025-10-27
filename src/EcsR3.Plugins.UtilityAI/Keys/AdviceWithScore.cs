namespace EcsR3.Plugins.UtilityAI.Keys
{
    public readonly struct AdviceWithScore
    {
        public readonly int AdviceId;
        public readonly float Score;

        public AdviceWithScore(int adviceId, float score)
        {
            AdviceId = adviceId;
            Score = score;
        }
    }
}