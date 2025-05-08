using System.Collections.Generic;
using System.Linq;
using SystemsR3.Extensions;
using EcsR3.Plugins.Views.ViewHandlers;
using SystemsR3.Pools;

namespace EcsR3.Plugins.Views.Pooling
{
    public class ViewPool : IViewPool
    {
        public readonly IList<ViewObjectContainer> PooledObjects = new List<ViewObjectContainer>();

        public PoolConfig PoolConfig { get; }
        public IViewHandler ViewHandler { get; }

        public ViewPool(IViewHandler viewHandler, PoolConfig poolConfig = null)
        {
            PoolConfig = poolConfig ?? new PoolConfig();
            ViewHandler = viewHandler;
        }
        
        public void PreAllocate(int? allocationCount = null)
        {
            var actualAllocation = allocationCount ?? PoolConfig.InitialSize;
            for (var i = 0; i < actualAllocation; i++)
            {
                var newInstance = ViewHandler.CreateView();
                ViewHandler.SetActiveState(newInstance, false);
                
                var objectContainer = new ViewObjectContainer(newInstance);
                PooledObjects.Add(objectContainer);
            }
        }

        public void DeAllocate(int dellocationCount)
        {
            PooledObjects.Where(x => !x.IsInUse)
                .Take(dellocationCount)
                .ToArray()
                .ForEachRun(OnDeallocateView);
        }

        private void OnDeallocateView(ViewObjectContainer x)
        {
            PooledObjects.Remove(x);
            ViewHandler.DestroyView(x.ViewObject);
        }

        public object AllocateInstance()
        {
            var availableViewObject = PooledObjects.FirstOrDefault(x => !x.IsInUse);
            if (availableViewObject == null)
            {
                PreAllocate(PoolConfig.ExpansionSize);
                availableViewObject = PooledObjects.First(x => !x.IsInUse);
            }

            availableViewObject.IsInUse = true;
            ViewHandler.SetActiveState(availableViewObject.ViewObject, true);
            return availableViewObject.ViewObject;
        }
        
        public void ReleaseInstance(object view)
        {
            var container = PooledObjects.FirstOrDefault(x => x.ViewObject == view);
            if(container == null) { return; }

            container.IsInUse = false;
            var viewObject = container.ViewObject;
            ViewHandler.SetActiveState(viewObject, false);
        }

        public void EmptyPool()
        {
            PooledObjects.ToArray()
                .ForEachRun(OnDeallocateView);

            PooledObjects.Clear();
        }
    }
}