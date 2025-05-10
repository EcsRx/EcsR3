using System;

namespace EcsR3.Entities.Routing
{
    public readonly struct ComponentContract : IEquatable<ComponentContract>
    {
        public readonly int[] ComponentIds;

        public ComponentContract(int[] componentIds)
        {
            ComponentIds = componentIds;
        }

        public int[] GetMatchingComponentIds(int[] comparingComponentIds)
        {
            Span<int> result = stackalloc int[comparingComponentIds.Length];
            var lastIndex = 0;
            for (var i = 0; i < ComponentIds.Length; i++)
            {
                var requiredComponentId = ComponentIds[i];
                for (var j = 0; j < comparingComponentIds.Length; j++)
                {
                    if(requiredComponentId == comparingComponentIds[j])
                    { 
                        result[lastIndex++] = requiredComponentId;
                        break;
                    }
                }
            }
            
            return result[..lastIndex].ToArray();
        }
        
        public int GetMatchingComponentIdsNoAlloc(int[] comparingComponentIds, int[] resultBuffer)
        {
            var lastIndex = 0;
            for (var i = 0; i < ComponentIds.Length; i++)
            {
                var requiredComponentId = ComponentIds[i];
                for (var j = 0; j < comparingComponentIds.Length; j++)
                {
                    if(requiredComponentId == comparingComponentIds[j])
                    { 
                        resultBuffer[lastIndex++] = requiredComponentId;
                        break;
                    }
                }
            }
            return lastIndex-1;
        }

        public bool Equals(ComponentContract other)
        {
            return Equals(ComponentIds, other.ComponentIds);
        }

        public override bool Equals(object obj)
        {
            return obj is ComponentContract other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (ComponentIds != null ? ComponentIds.GetHashCode() : 0);
        }

        public static bool operator ==(ComponentContract left, ComponentContract right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ComponentContract left, ComponentContract right)
        {
            return !left.Equals(right);
        }
    }
}