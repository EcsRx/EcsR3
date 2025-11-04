namespace EcsR3.Plugins.UtilityAI.Keys
{
    public readonly struct ConsiderationLookup
    {
        public readonly int ConsiderationId;
        public readonly bool HasRelatedData;

        public ConsiderationLookup(int considerationId, bool hasRelatedData = false)
        {
            ConsiderationId = considerationId;
            HasRelatedData = hasRelatedData;
        }
    }
}