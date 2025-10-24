using EcsR3.Components;
using EcsR3.Entities;

namespace EcsR3.Systems.Batching.Handlers
{
    public interface IMultiplexedJob<in T> where T : IComponent
    {
        void Process(Entity entity, T component);
    }
    
    public interface IMultiplexedJob<in T1, in T2> 
        where T1 : IComponent where T2 : IComponent
    {
        void Process(Entity entity, T1 component1, T2 component2);
    }
    
    public interface IMultiplexedJob<in T1, in T2, in T3> 
        where T1 : IComponent where T2 : IComponent where T3 : IComponent
    {
        void Process(Entity entity, T1 component1, T2 component2, T3 component3);
    }
    
    public interface IMultiplexedJob<in T1, in T2, in T3, in T4> 
        where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent
    {
        void Process(Entity entity, T1 component1, T2 component2, T3 component3, T4 component4);
    }
        
    public interface IMultiplexedJob<in T1, in T2, in T3, in T4, in T5> 
        where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent
    {
        void Process(Entity entity, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5);
    }
            
    public interface IMultiplexedJob<in T1, in T2, in T3, in T4, in T5, in T6> 
        where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent where T6 : IComponent
    {
        void Process(Entity entity, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5, T6 component6);
    }
                
    public interface IMultiplexedJob<in T1, in T2, in T3, in T4, in T5, in T6, in T7> 
        where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent where T6 : IComponent where T7 : IComponent
    {
        void Process(Entity entity, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5, T6 component6, T7 component7);
    }
}