namespace EcsR3.Collections.Entities
{
    public class CreationHasher : ICreationHasher
    {
        public const int StartingValue = 1;
        
        public int RollingHashValue { get; set; } = StartingValue;
        
        public int GenerateHash()
        {
            var hash = RollingHashValue.GetHashCode();
            if(RollingHashValue == int.MaxValue)
            { RollingHashValue = StartingValue; }
            else
            { RollingHashValue++; }
            return hash;
        }
    }
}