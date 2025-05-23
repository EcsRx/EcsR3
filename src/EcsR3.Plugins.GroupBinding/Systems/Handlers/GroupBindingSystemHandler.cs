using System;
using System.Linq;
using System.Reflection;
using SystemsR3.Attributes;
using SystemsR3.Executor.Handlers;
using SystemsR3.Systems;
using SystemsR3.Types;
using EcsR3.Collections;
using EcsR3.Computeds.Entities;
using EcsR3.Plugins.GroupBinding.Attributes;
using EcsR3.Plugins.GroupBinding.Exceptions;
using EcsR3.Systems;
using EcsR3.Groups;

namespace EcsR3.Plugins.GroupBinding.Systems.Handlers
{
    /// <summary>
    /// This will check all ISystem implementations to see if it contains any properties or fields that are
    /// IObservableGroups and if they have an attribute to indicate population from a group
    /// </summary>
    /// <remarks>
    /// The priority is 10 higher than SuperHigh just to make sure it runs before most common systems
    /// </remarks>
    [Priority(PriorityTypes.SuperHigh + 10)]
    public class GroupBindingSystemHandler : IConventionalSystemHandler
    {
        private static readonly Type FromGroupAttributeType = typeof(FromGroupAttribute); 
        private static readonly Type FromComponentsAttributeType = typeof(FromComponentsAttribute); 
        
        public IComputedGroupManager ComputedGroupManager { get; }

        public GroupBindingSystemHandler(IComputedGroupManager computedGroupManager)
        { ComputedGroupManager = computedGroupManager; }

        public bool CanHandleSystem(ISystem system)
        { return true; }

        public IGroup GetGroupFromAttributeIfAvailable(ISystem system, MemberInfo member)
        {
            var fromGroupAttribute = (FromGroupAttribute)member.GetCustomAttribute(FromGroupAttributeType, true);
            if (fromGroupAttribute != null)
            {
                var possibleGroup = fromGroupAttribute.Group;
                if (possibleGroup != null)
                { return possibleGroup; }

                if (system is IGroupSystem groupSystem)
                { return groupSystem.Group; }

                throw new MissingGroupSystemInterfaceException(system, member);
            }
            
            var fromComponentsAttribute = (FromComponentsAttribute)member.GetCustomAttribute(FromComponentsAttributeType, true);
            if (fromComponentsAttribute != null)
            { return fromComponentsAttribute.Group; }

            return Group.Empty;
        }

        public PropertyInfo[] GetApplicableProperties(Type systemType)
        {
            return systemType.GetProperties()
                .Where(x => x.CanWrite && x.PropertyType == typeof(IComputedEntityGroup))
                .ToArray();
        }
        
        public FieldInfo[] GetApplicableFields(Type systemType)
        {
            return systemType.GetFields()
                .Where(x => x.FieldType == typeof(IComputedEntityGroup))
                .ToArray();
        }

        public void ProcessProperty(PropertyInfo property, ISystem system)
        {
            var group = GetGroupFromAttributeIfAvailable(system, property);
            if (group == null) { return; }

            property.SetValue(system, ComputedGroupManager.GetComputedGroup(group));
        }

        public void ProcessField(FieldInfo field, ISystem system)
        {
            var group = GetGroupFromAttributeIfAvailable(system, field);
            if (group == null) { return; }

            field.SetValue(system, ComputedGroupManager.GetComputedGroup(group));
        }
        
        public void SetupSystem(ISystem system)
        {
            var systemType = system.GetType();
            var observableGroupProperties = GetApplicableProperties(systemType);
            var observableGroupFields = GetApplicableFields(systemType);

            if (observableGroupProperties.Length == 0 && observableGroupFields.Length == 0)
            { return; }

            foreach (var observableGroupProperty in observableGroupProperties)
            { ProcessProperty(observableGroupProperty, system); }
            
            foreach (var observableGroupField in observableGroupFields)
            { ProcessField(observableGroupField, system); }
        }

        public void DestroySystem(ISystem system)
        {
            // Nothing to destroy
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}