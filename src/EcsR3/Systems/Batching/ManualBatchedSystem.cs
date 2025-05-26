using EcsR3.Components.Database;
using EcsR3.Computeds.Components;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities;
using EcsR3.Groups;
using R3;
using SystemsR3.Extensions;
using SystemsR3.Systems.Conventional;
using SystemsR3.Threading;

namespace EcsR3.Systems.Batching
{
    public abstract class ManualBatchedSystem : IManualSystem, IGroupSystem
    {
        public abstract IGroup Group { get; }
        
        public IComponentDatabase ComponentDatabase { get; }
        public IComputedComponentGroupRegistry ComputedComponentGroupRegistry { get; }
        public IThreadHandler ThreadHandler { get; }
        
        protected IComputedComponentGroup ComputedComponentGroup { get; private set; }
        protected bool ShouldMultithread { get; set; }
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
        protected abstract Observable<Unit> ReactWhen();

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

        /// <summary>
        /// Get the underlying component group and return its basic form
        /// </summary>
        /// <returns></returns>
        protected abstract IComputedComponentGroup GetComponentGroup();

        /// <summary>
        /// Gets the entity from its provided id
        /// </summary>
        /// <param name="entityId">The entity id to lookup</param>
        /// <returns>The entity instance associated with this id</returns>
        public IEntity GetEntity(int entityId)
        { return ComputedComponentGroup.DataSource.Get(entityId); }
        
        public virtual void StartSystem()
        {
            Subscriptions = new CompositeDisposable();
            
            ComputedComponentGroup = GetComponentGroup();
            ShouldMultithread = this.ShouldMutliThread();
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