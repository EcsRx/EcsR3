﻿using EcsR3.Blueprints;
using EcsR3.Entities;
using EcsR3.Examples.ExampleApps.ComputedGroupExample.Components;
using EcsR3.Extensions;

namespace EcsR3.Examples.ExampleApps.ComputedGroupExample.Blueprints
{
    public class CharacterBlueprint : IBlueprint
    {
        public string Name { get; }
        public int Health { get; }

        public CharacterBlueprint(string name, int health)
        {
            Name = name;
            Health = health;
        }

        public void Apply(IEntity entity)
        {
            var healthComponent = new HasHealthComponent
            {
                CurrentHealth = Health,
                MaxHealth = Health
            };

            var nameComponent = new HasNameComponent
            {
                Name = Name
            };

            entity.AddComponents(nameComponent, healthComponent);
        }
    }
}