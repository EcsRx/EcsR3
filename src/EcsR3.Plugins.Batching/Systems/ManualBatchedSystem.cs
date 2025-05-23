using System;
using SystemsR3.Extensions;
using SystemsR3.Systems.Conventional;
using SystemsR3.Threading;
using EcsR3.Collections;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Computeds.Groups;
using EcsR3.Entities;
using EcsR3.Groups;
using EcsR3.Groups.Observable;
using EcsR3.Systems;
using R3;

namespace EcsR3.Plugins.Batching.Systems
{
    public abstract class ManualBatchedSystem : IManualSystem, IGroupSystem
    {
        public abstract IGroup Group { get; }
        
        public IComputedGroupManager ComputedGroupManager { get; }
        public IComponentDatabase ComponentDatabase { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }
        public IThreadHandler ThreadHandler { get; }
        
        protected IComputedEntityGroup ObservableGroup { get; private set; }
        protected bool ShouldParallelize { get; private set; }
        protected IDisposable Subscriptions;

        protected ManualBatchedSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup, IThreadHandler threadHandler, IComputedGroupManager computedGroupManager)
        {
            ComponentDatabase = componentDatabase;
            ComponentTypeLookup = componentTypeLookup;
            ThreadHandler = threadHandler;
            ComputedGroupManager = computedGroupManager;
        }

        protected abstract void RebuildBatch();
        
        /// <summary>
        /// This describes when the system should be processed
        /// </summary>
        /// <returns>A trigger indicating that the process should run</returns>
        protected abstract Observable<bool> ReactWhen();

        /// <summary>
        /// Do anything before the batch gets processed
        /// </summary>
        protected virtual void BeforeProcessing(){}
        
        /// <summary>
        /// Do anything after the batch has been processed
        /// </summary>
        protected virtual void AfterProcessing(){}
        
        /// <summary>
        /// The wrapper for processing the underlying batch
        /// </summary>
        protected abstract void ProcessBatch();

        public virtual void StartSystem()
        {
            ObservableGroup = ComputedGroupManager.GetComputedGroup(Group);
            ShouldParallelize = this.ShouldMutliThread();
            
            var subscriptions = new CompositeDisposable();
            ProcessGroupSubscription(ObservableGroup.OnAdded)
                .Subscribe(_ => RebuildBatch())
                .AddTo(subscriptions);
           
            ProcessGroupSubscription(ObservableGroup.OnRemoved)
                .Subscribe(_ => RebuildBatch())
                .AddTo(subscriptions);
            
            RebuildBatch();
            ReactWhen().Subscribe(_ => RunBatch()).AddTo(subscriptions);
            
            Subscriptions = subscriptions;
        }

        /// <summary>
        /// This processes the group level subscription, allowing you to change how the change of a group should be run 
        /// </summary>
        /// <param name="groupChange"></param>
        /// <returns>The observable stream that should be subscribed to</returns>
        /// <remarks>Out the box it will just pass through the observable but in a lot of cases you may want to
        /// throttle the group changes so multiple ones within a single frame would be run once.</remarks>
        protected virtual Observable<IEntity> ProcessGroupSubscription(Observable<IEntity> groupChange)
        { return groupChange; }

        private void RunBatch()
        {
            BeforeProcessing();
            ProcessBatch();
            AfterProcessing();
        }

        public virtual void StopSystem()
        { Subscriptions.Dispose(); }
    }
}