namespace EcsR3.Collections.Entities
{
    public class CreationHasher : ICreationHasher
    {
        public int RollingHashValue { get; set; }
        
        public int GenerateHash()
        {
            var hash = RollingHashValue.GetHashCode();
            if(RollingHashValue == int.MaxValue)
            { RollingHashValue = 0; }
            else
            { RollingHashValue++; }
            return hash;
        }
    }
}