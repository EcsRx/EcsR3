﻿using System.Runtime.InteropServices;
using EcsR3.Entities;

namespace EcsR3.Computeds.Components
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct ComponentBatch<T1>
    {
        public readonly Entity Entity;
        
        public readonly int Component1Allocation;

        public ComponentBatch(Entity entity, int component1Allocation)
        {
            Entity = entity;
            Component1Allocation = component1Allocation;
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct ComponentBatch<T1, T2>
    {
        public readonly Entity Entity;
        
        public readonly int Component1Allocation;
        public readonly int Component2Allocation;

        public ComponentBatch(Entity entity, int component1Allocation, int component2Allocation)
        {
            Entity = entity;
            Component1Allocation = component1Allocation;
            Component2Allocation = component2Allocation;
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct ComponentBatch<T1, T2, T3>
    {
        public readonly Entity Entity;
        
        public readonly int Component1Allocation;
        public readonly int Component2Allocation;
        public readonly int Component3Allocation;

        public ComponentBatch(Entity entity, int component1Allocation, int component2Allocation, int component3Allocation)
        {
            Entity = entity;
            Component1Allocation = component1Allocation;
            Component2Allocation = component2Allocation;
            Component3Allocation = component3Allocation;
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct ComponentBatch<T1, T2, T3, T4>
    {
        public readonly Entity Entity;
        
        public readonly int Component1Allocation;
        public readonly int Component2Allocation;
        public readonly int Component3Allocation;
        public readonly int Component4Allocation;

        public ComponentBatch(Entity entity, int component1Allocation, int component2Allocation, int component3Allocation, int component4Allocation)
        {
            Entity = entity;
            Component1Allocation = component1Allocation;
            Component2Allocation = component2Allocation;
            Component3Allocation = component3Allocation;
            Component4Allocation = component4Allocation;
        }
    }
        
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct ComponentBatch<T1, T2, T3, T4, T5>
    {
        public readonly Entity Entity;
        
        public readonly int Component1Allocation;
        public readonly int Component2Allocation;
        public readonly int Component3Allocation;
        public readonly int Component4Allocation;
        public readonly int Component5Allocation;

        public ComponentBatch(Entity entity, int component1Allocation, int component2Allocation, int component3Allocation, int component4Allocation, int component5Allocation)
        {
            Entity = entity;
            Component1Allocation = component1Allocation;
            Component2Allocation = component2Allocation;
            Component3Allocation = component3Allocation;
            Component4Allocation = component4Allocation;
            Component5Allocation = component5Allocation;
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct ComponentBatch<T1, T2, T3, T4, T5, T6>
    {
        public readonly Entity Entity;
        
        public readonly int Component1Allocation;
        public readonly int Component2Allocation;
        public readonly int Component3Allocation;
        public readonly int Component4Allocation;
        public readonly int Component5Allocation;
        public readonly int Component6Allocation;

        public ComponentBatch(Entity entity, int component1Allocation, int component2Allocation, int component3Allocation, int component4Allocation, int component5Allocation, int component6Allocation)
        {
            Entity = entity;
            Component1Allocation = component1Allocation;
            Component2Allocation = component2Allocation;
            Component3Allocation = component3Allocation;
            Component4Allocation = component4Allocation;
            Component5Allocation = component5Allocation;
            Component6Allocation = component6Allocation;
        }
    }
        
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct ComponentBatch<T1, T2, T3, T4, T5, T6, T7>
    {
        public readonly Entity Entity;
        
        public readonly int Component1Allocation;
        public readonly int Component2Allocation;
        public readonly int Component3Allocation;
        public readonly int Component4Allocation;
        public readonly int Component5Allocation;
        public readonly int Component6Allocation;
        public readonly int Component7Allocation;

        public ComponentBatch(Entity entity, int component1Allocation, int component2Allocation, int component3Allocation, int component4Allocation, int component5Allocation, int component6Allocation, int component7Allocation)
        {
            Entity = entity;
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