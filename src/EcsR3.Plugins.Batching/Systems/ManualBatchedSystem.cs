using SystemsR3.Extensions;
using SystemsR3.Systems.Conventional;
using SystemsR3.Threading;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities;
using EcsR3.Groups;
using EcsR3.Systems;
using R3;

namespace EcsR3.Plugins.Batching.Systems
{
    public abstract class ManualBatchedSystem : IManualSystem, IGroupSystem
    {
        public abstract IGroup Group { get; }
        
        public IComponentDatabase ComponentDatabase { get; }
        public IComputedComponentGroupRegistry ComputedComponentGroupRegistry { get; }
        public IThreadHandler ThreadHandler { get; }
        
        protected IComputedComponentGroup ComputedComponentGroup { get; private set; }
        protected bool ShouldParallelize { get; private set; }
        protected CompositeDisposable Subscriptions;

        protected ManualBatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler)
        {
            ComponentDatabase = componentDatabase;
            ComputedComponentGroupRegistry = computedComponentGroupRegistry;
            ThreadHandler = threadHandler;
        }
        
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

        protected abstract IComputedComponentGroup GetComponentGroup();

        public IEntity GetEntity(int entityId)
        { return ComputedComponentGroup.DataSource.Get(entityId); }
        
        public virtual void StartSystem()
        {
            Subscriptions = new CompositeDisposable();
            
            ComputedComponentGroup = GetComponentGroup();
            ShouldParallelize = this.ShouldMutliThread();
            ReactWhen().Subscribe(_ => RunBatch()).AddTo(Subscriptions);
        }
        
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