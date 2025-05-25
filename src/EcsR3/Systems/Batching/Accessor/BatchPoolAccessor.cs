using EcsR3.Components;
using EcsR3.Components.Database;

namespace EcsR3.Systems.Batching.Accessor
{
    public class BatchPoolAccessor<T1> where T1 : IComponent
    {
        private readonly IComponentPool<T1> _componentPool1;

        public BatchPoolAccessor(IComponentDatabase componentDatabase)
        {
            _componentPool1 = componentDatabase.GetPoolFor<T1>();
        }

        public T1[] GetPoolArrays()
        { return _componentPool1.Components; }
    }
    
    public class BatchPoolAccessor<T1, T2> where T1 : IComponent where T2 : IComponent
    {
        private readonly IComponentPool<T1> _componentPool1;
        private readonly IComponentPool<T2> _componentPool2;

        public BatchPoolAccessor(IComponentDatabase componentDatabase)
        {
            _componentPool1 = componentDatabase.GetPoolFor<T1>();
            _componentPool2 = componentDatabase.GetPoolFor<T2>();
        }

        public (T1[], T2[]) GetPoolArrays()
        { return (_componentPool1.Components, _componentPool2.Components); }
    }
        
    public class BatchPoolAccessor<T1, T2, T3> where T1 : IComponent where T2 : IComponent where T3 : IComponent
    {
        private readonly IComponentPool<T1> _componentPool1;
        private readonly IComponentPool<T2> _componentPool2;
        private readonly IComponentPool<T3> _componentPool3;

        public BatchPoolAccessor(IComponentDatabase componentDatabase)
        {
            _componentPool1 = componentDatabase.GetPoolFor<T1>();
            _componentPool2 = componentDatabase.GetPoolFor<T2>();
            _componentPool3 = componentDatabase.GetPoolFor<T3>();
        }

        public (T1[], T2[], T3[]) GetPoolArrays()
        { return (_componentPool1.Components, _componentPool2.Components, _componentPool3.Components); }
    }
        
    public class BatchPoolAccessor<T1, T2, T3, T4> where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent
    {
        private readonly IComponentPool<T1> _componentPool1;
        private readonly IComponentPool<T2> _componentPool2;
        private readonly IComponentPool<T3> _componentPool3;
        private readonly IComponentPool<T4> _componentPool4;

        public BatchPoolAccessor(IComponentDatabase componentDatabase)
        {
            _componentPool1 = componentDatabase.GetPoolFor<T1>();
            _componentPool2 = componentDatabase.GetPoolFor<T2>();
            _componentPool3 = componentDatabase.GetPoolFor<T3>();
            _componentPool4 = componentDatabase.GetPoolFor<T4>();
        }

        public (T1[], T2[], T3[], T4[]) GetPoolArrays()
        { return (_componentPool1.Components, _componentPool2.Components, _componentPool3.Components, _componentPool4.Components); }
    }
    
    public class BatchPoolAccessor<T1, T2, T3, T4, T5> where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent
    {
        private readonly IComponentPool<T1> _componentPool1;
        private readonly IComponentPool<T2> _componentPool2;
        private readonly IComponentPool<T3> _componentPool3;
        private readonly IComponentPool<T4> _componentPool4;
        private readonly IComponentPool<T5> _componentPool5;

        public BatchPoolAccessor(IComponentDatabase componentDatabase)
        {
            _componentPool1 = componentDatabase.GetPoolFor<T1>();
            _componentPool2 = componentDatabase.GetPoolFor<T2>();
            _componentPool3 = componentDatabase.GetPoolFor<T3>();
            _componentPool4 = componentDatabase.GetPoolFor<T4>();
            _componentPool5 = componentDatabase.GetPoolFor<T5>();
        }

        public (T1[], T2[], T3[], T4[], T5[]) GetPoolArrays()
        { return (_componentPool1.Components, _componentPool2.Components, _componentPool3.Components, _componentPool4.Components, _componentPool5.Components); }
    }
    public class BatchPoolAccessor<T1, T2, T3, T4, T5, T6> where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent where T6 : IComponent
    {
        private readonly IComponentPool<T1> _componentPool1;
        private readonly IComponentPool<T2> _componentPool2;
        private readonly IComponentPool<T3> _componentPool3;
        private readonly IComponentPool<T4> _componentPool4;
        private readonly IComponentPool<T5> _componentPool5;
        private readonly IComponentPool<T6> _componentPool6;

        public BatchPoolAccessor(IComponentDatabase componentDatabase)
        {
            _componentPool1 = componentDatabase.GetPoolFor<T1>();
            _componentPool2 = componentDatabase.GetPoolFor<T2>();
            _componentPool3 = componentDatabase.GetPoolFor<T3>();
            _componentPool4 = componentDatabase.GetPoolFor<T4>();
            _componentPool5 = componentDatabase.GetPoolFor<T5>();
            _componentPool6 = componentDatabase.GetPoolFor<T6>();
        }

        public (T1[], T2[], T3[], T4[], T5[], T6[]) GetPoolArrays()
        { return (_componentPool1.Components, _componentPool2.Components, _componentPool3.Components, 
            _componentPool4.Components, _componentPool5.Components, _componentPool6.Components); }
    }
    
    public class BatchPoolAccessor<T1, T2, T3, T4, T5, T6, T7> where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent where T6 : IComponent where T7 : IComponent
    {
        private readonly IComponentPool<T1> _componentPool1;
        private readonly IComponentPool<T2> _componentPool2;
        private readonly IComponentPool<T3> _componentPool3;
        private readonly IComponentPool<T4> _componentPool4;
        private readonly IComponentPool<T5> _componentPool5;
        private readonly IComponentPool<T6> _componentPool6;
        private readonly IComponentPool<T7> _componentPool7;

        public BatchPoolAccessor(IComponentDatabase componentDatabase)
        {
            _componentPool1 = componentDatabase.GetPoolFor<T1>();
            _componentPool2 = componentDatabase.GetPoolFor<T2>();
            _componentPool3 = componentDatabase.GetPoolFor<T3>();
            _componentPool4 = componentDatabase.GetPoolFor<T4>();
            _componentPool5 = componentDatabase.GetPoolFor<T5>();
            _componentPool6 = componentDatabase.GetPoolFor<T6>();
            _componentPool7 = componentDatabase.GetPoolFor<T7>();
        }

        public (T1[], T2[], T3[], T4[], T5[], T6[], T7[]) GetPoolArrays()
        { return (_componentPool1.Components, _componentPool2.Components, _componentPool3.Components, 
            _componentPool4.Components, _componentPool5.Components, _componentPool6.Components, 
            _componentPool7.Components); }
    }
}