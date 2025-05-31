using System;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.Custom.SystemPriority.Groups;
using EcsR3.Groups;
using EcsR3.Systems;
using EcsR3.Systems.Reactive;
using SystemsR3.Attributes;

namespace EcsR3.Examples.Custom.SystemPriority.Systems
{
    [Priority(10)]
    public class FirstSystem : ISetupSystem
    {
        public IGroup Group => new MessageGroup();

        public void Setup(IEntityComponentAccessor entityComponentAccessor, int entityId)
        {
            Console.WriteLine("SYSTEM 1");
        }
    }
}