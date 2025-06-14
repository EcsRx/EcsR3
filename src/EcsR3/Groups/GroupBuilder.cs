﻿using System;
using System.Collections.Generic;
using EcsR3.Components;
using EcsR3.Entities;

namespace EcsR3.Groups
{
    public class GroupBuilder
    {
        private List<Type> _withComponents;
        private List<Type> _withoutComponents;

        public GroupBuilder()
        {
            _withComponents = new List<Type>();
            _withoutComponents = new List<Type>();
        }

        public GroupBuilder Create()
        {
            _withComponents = new List<Type>();
            _withoutComponents = new List<Type>();
            return this;
        }

        public GroupBuilder WithComponent<T>() where T : class, IComponent
        {
            _withComponents.Add(typeof(T));
            return this;
        }
        
        public GroupBuilder WithoutComponent<T>() where T : class, IComponent
        {
            _withoutComponents.Add(typeof(T));
            return this;
        }
        
        public GroupBuilder WithStructComponent<T>() where T : struct, IComponent
        {
            _withComponents.Add(typeof(T));
            return this;
        }
        
        public GroupBuilder WithoutStructComponent<T>() where T : struct, IComponent
        {
            _withoutComponents.Add(typeof(T));
            return this;
        }


        public IGroup Build()
        { return new Group(_withComponents, _withoutComponents); }
    }
}