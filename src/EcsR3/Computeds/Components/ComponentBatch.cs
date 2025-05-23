namespace EcsR3.Computeds.Components
{
    public readonly struct ComponentBatch<T1>
    {
        public readonly int EntityId;
        
        public readonly int Component1Allocation;

        public ComponentBatch(int entityId, int component1Allocation)
        {
            EntityId = entityId;
            Component1Allocation = component1Allocation;
        }
    }
    
    public readonly struct ComponentBatch<T1, T2>
    {
        public readonly int EntityId;
        
        public readonly int Component1Allocation;
        public readonly int Component2Allocation;

        public ComponentBatch(int entityId, int component1Allocation, int component2Allocation)
        {
            EntityId = entityId;
            Component1Allocation = component1Allocation;
            Component2Allocation = component2Allocation;
        }
    }
    
    public readonly struct ComponentBatch<T1, T2, T3>
    {
        public readonly int EntityId;
        
        public readonly int Component1Allocation;
        public readonly int Component2Allocation;
        public readonly int Component3Allocation;

        public ComponentBatch(int entityId, int component1Allocation, int component2Allocation, int component3Allocation)
        {
            EntityId = entityId;
            Component1Allocation = component1Allocation;
            Component2Allocation = component2Allocation;
            Component3Allocation = component3Allocation;
        }
    }
    
    public readonly struct ComponentBatch<T1, T2, T3, T4>
    {
        public readonly int EntityId;
        
        public readonly int Component1Allocation;
        public readonly int Component2Allocation;
        public readonly int Component3Allocation;
        public readonly int Component4Allocation;

        public ComponentBatch(int entityId, int component1Allocation, int component2Allocation, int component3Allocation, int component4Allocation)
        {
            EntityId = entityId;
            Component1Allocation = component1Allocation;
            Component2Allocation = component2Allocation;
            Component3Allocation = component3Allocation;
            Component4Allocation = component4Allocation;
        }
    }
        
    public readonly struct ComponentBatch<T1, T2, T3, T4, T5>
    {
        public readonly int EntityId;
        
        public readonly int Component1Allocation;
        public readonly int Component2Allocation;
        public readonly int Component3Allocation;
        public readonly int Component4Allocation;
        public readonly int Component5Allocation;

        public ComponentBatch(int entityId, int component1Allocation, int component2Allocation, int component3Allocation, int component4Allocation, int component5Allocation)
        {
            EntityId = entityId;
            Component1Allocation = component1Allocation;
            Component2Allocation = component2Allocation;
            Component3Allocation = component3Allocation;
            Component4Allocation = component4Allocation;
            Component5Allocation = component5Allocation;
        }
    }
    
    public readonly struct ComponentBatch<T1, T2, T3, T4, T5, T6>
    {
        public readonly int EntityId;
        
        public readonly int Component1Allocation;
        public readonly int Component2Allocation;
        public readonly int Component3Allocation;
        public readonly int Component4Allocation;
        public readonly int Component5Allocation;
        public readonly int Component6Allocation;

        public ComponentBatch(int entityId, int component1Allocation, int component2Allocation, int component3Allocation, int component4Allocation, int component5Allocation, int component6Allocation)
        {
            EntityId = entityId;
            Component1Allocation = component1Allocation;
            Component2Allocation = component2Allocation;
            Component3Allocation = component3Allocation;
            Component4Allocation = component4Allocation;
            Component5Allocation = component5Allocation;
            Component6Allocation = component6Allocation;
        }
    }
        
    public readonly struct ComponentBatch<T1, T2, T3, T4, T5, T6, T7>
    {
        public readonly int EntityId;
        
        public readonly int Component1Allocation;
        public readonly int Component2Allocation;
        public readonly int Component3Allocation;
        public readonly int Component4Allocation;
        public readonly int Component5Allocation;
        public readonly int Component6Allocation;
        public readonly int Component7Allocation;

        public ComponentBatch(int entityId, int component1Allocation, int component2Allocation, int component3Allocation, int component4Allocation, int component5Allocation, int component6Allocation, int component7Allocation)
        {
            EntityId = entityId;
            Component1Allocation = component1Allocation;
            Component2Allocation = component2Allocation;
            Component3Allocation = component3Allocation;
            Component4Allocation = component4Allocation;
            Component5Allocation = component5Allocation;
            Component6Allocation = component6Allocation;
            Component7Allocation = component7Allocation;
        }
    }
}