using System.IO;
using System.Threading.Tasks;
using EcsR3.Collections.Entities;
using SystemsR3.Infrastructure.Extensions;
using EcsR3.Infrastructure;
using EcsR3.Plugins.Persistence.Modules;
using EcsR3.Plugins.Persistence.Pipelines;

namespace EcsR3.Plugins.Persistence
{
    public abstract class EcsR3PersistedApplication : EcsR3Application
    {
        public ISaveEntityCollectionPipeline SaveEntityCollectionPipeline;
        public ILoadEntityCollectionPipeline LoadEntityCollectionPipeline;

        public virtual string EntityDatabaseFile => PersistityModule.DefaultEntityDatabaseFile;
        public virtual bool LoadOnStart => true;
        public virtual bool SaveOnStop => true;
        
        protected override void LoadPlugins()
        {
            base.LoadPlugins();
            RegisterPlugin(new PersistencePlugin());
        }
        
        protected override void ResolveApplicationDependencies()
        {
            SaveEntityCollectionPipeline = DependencyResolver.Resolve<ISaveEntityCollectionPipeline>();
            LoadEntityCollectionPipeline = DependencyResolver.Resolve<ILoadEntityCollectionPipeline>();

            if(LoadOnStart)
            { LoadEntityDatabase().Wait(); }
            
            base.ResolveApplicationDependencies();
        }
        
        protected virtual async Task LoadEntityDatabase()
        {
            // If there is no file just ignore loading
            if (!File.Exists(EntityDatabaseFile)) { return; }
            
            var entityCollection = await LoadEntityCollectionPipeline.Execute();
            DependencyRegistry.Unbind<IEntityCollection>();
            DependencyRegistry.Bind<IEntityCollection>(x => x.ToInstance(entityCollection));
        }
        
        protected virtual Task SaveEntityDatabase()
        {
            // Update our database with any changes that have happened since it loaded
            return SaveEntityCollectionPipeline.Execute(EntityCollection);
        }

        public override void StopApplication()
        {
            if(SaveOnStop)
            { SaveEntityDatabase().Wait(); }
            
            base.StopApplication();
        }
    }
}