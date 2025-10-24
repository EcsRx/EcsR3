using EcsR3.Components;
using EcsR3.Entities;

namespace EcsR3.Systems.Batching.Convention.Multiplexing.Handlers
{
    public interface IMultiplexedRefJob<T> where T : IComponent
    {
        void Process(Entity entity, ref T component);
    }
    
    public interface IMultiplexedRefJob<T1, T2> 
        where T1 : IComponent where T2 : IComponent
    {
        void Process(Entity entity, ref T1 component1, ref T2 component2);
    }
    
    public interface IMultiplexedRefJob<T1, T2, T3> 
        where T1 : IComponent where T2 : IComponent where T3 : IComponent
    {
        void Process(Entity entity, ref T1 component1, ref T2 component2, ref T3 component3);
    }
    
    public interface IMultiplexedRefJob<T1, T2, T3, T4> 
        where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent
    {
        void Process(Entity entity, ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4);
    }
        
    public interface IMultiplexedRefJob<T1, T2, T3, T4, T5> 
        where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent
    {
        void Process(Entity entity, ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4, ref T5 component5);
    }
            
    public interface IMultiplexedRefJob<T1, T2, T3, T4, T5, T6> 
        where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent where T6 : IComponent
    {
        void Process(Entity entity, ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4, ref T5 component5, ref T6 component6);
    }
                
    public interface IMultiplexedRefJob<T1, T2, T3, T4, T5, T6, T7> 
        where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent where T6 : IComponent where T7 : IComponent
    {
        void Process(Entity entity, ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4, ref T5 component5, ref T6 component6, ref T7 component7);
    }
}