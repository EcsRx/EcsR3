using System.IO;
using System.Threading.Tasks;
using SystemsR3.Infrastructure.Extensions;
using EcsR3.Collections.Database;
using EcsR3.Infrastructure;
using EcsR3.Plugins.Persistence.Modules;
using EcsR3.Plugins.Persistence.Pipelines;

namespace EcsR3.Plugins.Persistence
{
    public abstract class EcsRxPersistedApplication : EcsRxApplication
    {
        public ISaveEntityDatabasePipeline SaveEntityDatabasePipeline;
        public ILoadEntityDatabasePipeline LoadEntityDatabasePipeline;

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
            SaveEntityDatabasePipeline = DependencyResolver.Resolve<ISaveEntityDatabasePipeline>();
            LoadEntityDatabasePipeline = DependencyResolver.Resolve<ILoadEntityDatabasePipeline>();

            if(LoadOnStart)
            { LoadEntityDatabase().Wait(); }
            
            base.ResolveApplicationDependencies();
        }
        
        protected virtual async Task LoadEntityDatabase()
        {
            // If there is no file just ignore loading
            if (!File.Exists(EntityDatabaseFile)) { return; }
            
            var entityDatabase = await LoadEntityDatabasePipeline.Execute();
            DependencyRegistry.Unbind<IEntityDatabase>();
            DependencyRegistry.Bind<IEntityDatabase>(x => x.ToInstance(entityDatabase));
        }
        
        protected virtual Task SaveEntityDatabase()
        {
            // Update our database with any changes that have happened since it loaded
            return SaveEntityDatabasePipeline.Execute(EntityDatabase);
        }

        public override void StopApplication()
        {
            if(SaveOnStop)
            { SaveEntityDatabase().Wait(); }
            
            base.StopApplication();
        }
    }
}