using System;
using SystemsR3.Attributes;
using EcsR3.Entities;
using EcsR3.Examples.Custom.Groups;
using EcsR3.Groups;
using EcsR3.Systems;

namespace EcsR3.Examples.Custom.Systems
{
    [Priority(10)]
    public class FirstSystem : ISetupSystem
    {
        public IGroup Group => new MessageGroup();

        public void Setup(IEntity entity)
        {
            Console.WriteLine("SYSTEM 1");
        }
    }
}