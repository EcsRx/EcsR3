using System;
using System.Collections.Generic;
using EcsR3.Components;
using EcsR3.Components.Lookups;
using EcsR3.Computeds.Entities.Registries;
using EcsR3.Extensions;
using EcsR3.Groups;
using SystemsR3.Extensions;

namespace EcsR3.Computeds.Components.Registries
{
    public class ComputedComponentGroupRegistry : IComputedComponentGroupRegistry
    {
        private const string CastError = "Existing computed group does not match generic layout, ensure all component groups are created with the same generic layout.";
     
        // We need to ensure there is only 1 computed component listing per group
        public Dictionary<LookupGroup, IComputedComponentGroup> _computedGroups { get; }

        public IEnumerable<IComputedComponentGroup> ComputedGroups => _computedGroups.Values;

        public IComponentTypeLookup ComponentTypeLookup { get; }
        public IComputedEntityGroupRegistry ComputedEntityGroupRegistry { get; }
        
        private readonly object _lock = new object();
        
        public ComputedComponentGroupRegistry(IComputedEntityGroupRegistry computedEntityGroupRegistry, IComponentTypeLookup componentTypeLookup)
        {
            ComputedEntityGroupRegistry = computedEntityGroupRegistry;
            ComponentTypeLookup = componentTypeLookup;

            _computedGroups = new Dictionary<LookupGroup, IComputedComponentGroup>();
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _computedGroups.Values.DisposeAll();
                _computedGroups.Clear();
            }
        }

        public IComputedComponentGroup<T1> GetComputedGroup<T1>() where T1 : IComponent
        {
            var lookupGroup = ComponentTypeLookup.GetLookupGroupFor(typeof(T1));
            if (_computedGroups.TryGetValue(lookupGroup, out var existingComputedGroup))
            {
                if (!(existingComputedGroup is IComputedComponentGroup<T1> castComponentGroup)) 
                { throw new InvalidCastException(CastError); }
                return castComponentGroup;
            }

            var computedEntityGroup = ComputedEntityGroupRegistry.GetComputedGroup(lookupGroup);
            var computedGroup = new ComputedComponentGroup<T1>(ComponentTypeLookup, computedEntityGroup);
            _computedGroups.Add(lookupGroup, computedGroup);
            return computedGroup;
        }

        public IComputedComponentGroup<T1, T2> GetComputedGroup<T1, T2>() where T1 : IComponent where T2 : IComponent
        {
            var lookupGroup = ComponentTypeLookup.GetLookupGroupFor(typeof(T1), typeof(T2));
            if (_computedGroups.TryGetValue(lookupGroup, out var existingComputedGroup))
            {
                if (!(existingComputedGroup is IComputedComponentGroup<T1, T2> castComponentGroup)) 
                { throw new InvalidCastException(CastError); }
                return castComponentGroup;
            }

            var computedEntityGroup = ComputedEntityGroupRegistry.GetComputedGroup(lookupGroup);
            var computedGroup = new ComputedComponentGroup<T1, T2>(ComponentTypeLookup, computedEntityGroup);
            _computedGroups.Add(lookupGroup, computedGroup);
            return computedGroup;
        }

        public IComputedComponentGroup<T1, T2, T3> GetComputedGroup<T1, T2, T3>() where T1 : IComponent where T2 : IComponent where T3 : IComponent
        {
            var lookupGroup = ComponentTypeLookup.GetLookupGroupFor(typeof(T1), typeof(T2), typeof(T3));
            if (_computedGroups.TryGetValue(lookupGroup, out var existingComputedGroup))
            {
                if (!(existingComputedGroup is IComputedComponentGroup<T1, T2, T3> castComponentGroup)) 
                { throw new InvalidCastException(CastError); }
                return castComponentGroup;
            }

            var computedEntityGroup = ComputedEntityGroupRegistry.GetComputedGroup(lookupGroup);
            var computedGroup = new ComputedComponentGroup<T1, T2, T3>(ComponentTypeLookup, computedEntityGroup);
            _computedGroups.Add(lookupGroup, computedGroup);
            return computedGroup;
        }

        public IComputedComponentGroup<T1, T2, T3, T4> GetComputedGroup<T1, T2, T3, T4>() where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent
        {
            var lookupGroup = ComponentTypeLookup.GetLookupGroupFor(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
            if (_computedGroups.TryGetValue(lookupGroup, out var existingComputedGroup))
            {
                if (!(existingComputedGroup is IComputedComponentGroup<T1, T2, T3, T4> castComponentGroup)) 
                { throw new InvalidCastException(CastError); }
                return castComponentGroup;
            }

            var computedEntityGroup = ComputedEntityGroupRegistry.GetComputedGroup(lookupGroup);
            var computedGroup = new ComputedComponentGroup<T1, T2, T3, T4>(ComponentTypeLookup, computedEntityGroup);
            _computedGroups.Add(lookupGroup, computedGroup);
            return computedGroup;
        }
        
        public IComputedComponentGroup<T1, T2, T3, T4, T5> GetComputedGroup<T1, T2, T3, T4, T5>() where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent
        {
            var lookupGroup = ComponentTypeLookup.GetLookupGroupFor(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
            if (_computedGroups.TryGetValue(lookupGroup, out var existingComputedGroup))
            {
                if (!(existingComputedGroup is IComputedComponentGroup<T1, T2, T3, T4, T5> castComponentGroup)) 
                { throw new InvalidCastException(CastError); }
                return castComponentGroup;
            }

            var computedEntityGroup = ComputedEntityGroupRegistry.GetComputedGroup(lookupGroup);
            var computedGroup = new ComputedComponentGroup<T1, T2, T3, T4, T5>(ComponentTypeLookup, computedEntityGroup);
            _computedGroups.Add(lookupGroup, computedGroup);
            return computedGroup;
        }
        
        public IComputedComponentGroup<T1, T2, T3, T4, T5, T6> GetComputedGroup<T1, T2, T3, T4, T5, T6>() where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent where T6 : IComponent
        {
            var lookupGroup = ComponentTypeLookup.GetLookupGroupFor(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
            if (_computedGroups.TryGetValue(lookupGroup, out var existingComputedGroup))
            {
                if (!(existingComputedGroup is IComputedComponentGroup<T1, T2, T3, T4, T5, T6> castComponentGroup)) 
                { throw new InvalidCastException(CastError); }
                return castComponentGroup;
            }

            var computedEntityGroup = ComputedEntityGroupRegistry.GetComputedGroup(lookupGroup);
            var computedGroup = new ComputedComponentGroup<T1, T2, T3, T4, T5, T6>(ComponentTypeLookup, computedEntityGroup);
            _computedGroups.Add(lookupGroup, computedGroup);
            return computedGroup;
        }
        
        public IComputedComponentGroup<T1, T2, T3, T4, T5, T6, T7> GetComputedGroup<T1, T2, T3, T4, T5, T6, T7>() where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent where T6 : IComponent where T7 : IComponent
        {
            var lookupGroup = ComponentTypeLookup.GetLookupGroupFor(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));
            if (_computedGroups.TryGetValue(lookupGroup, out var existingComputedGroup))
            {
                if (!(existingComputedGroup is IComputedComponentGroup<T1, T2, T3, T4, T5, T6, T7> castComponentGroup)) 
                { throw new InvalidCastException(CastError); }
                return castComponentGroup;
            }

            var computedEntityGroup = ComputedEntityGroupRegistry.GetComputedGroup(lookupGroup);
            var computedGroup = new ComputedComponentGroup<T1, T2, T3, T4, T5, T6, T7>(ComponentTypeLookup, computedEntityGroup);
            _computedGroups.Add(lookupGroup, computedGroup);
            return computedGroup;
        }
    }
}