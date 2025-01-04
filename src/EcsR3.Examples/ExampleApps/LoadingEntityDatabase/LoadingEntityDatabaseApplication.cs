using System;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;
using SystemsR3.Infrastructure.Ninject;
using EcsR3.Examples.ExampleApps.LoadingEntityDatabase.Blueprints;
using EcsR3.Examples.ExampleApps.LoadingEntityDatabase.Modules;
using EcsR3.Plugins.Persistence;
using EcsR3.Plugins.Persistence.Extensions;

namespace EcsR3.Examples.ExampleApps.LoadingEntityDatabase
{
    // We extend from EcsRxPersistedApplication which has built in helpers for persisting entity DB
    public class LoadingEntityDatabaseApplication : EcsR3PersistedApplication
    {
        public override IDependencyRegistry DependencyRegistry { get; }  = new NinjectDependencyRegistry();

        // Tell it to look for the JSON file now rather than the binary one
        public override string EntityDatabaseFile => JsonEntityDatabaseModule.CustomEntityDatabaseFile;
        
        private bool _quit;

        protected override void LoadModules()
        {
            base.LoadModules();
            
            // Add support for serializing/deserializing System.Numerics
            DependencyRegistry.LoadModule<EnableNumericsModule>();
        }
        
        protected override void ResolveApplicationDependencies()
        {
            // Replace our default binary entity database with json,
            // we do this here as the plugins loaded by now and we need to override 
            // bindings set in place by the plugin.
            DependencyRegistry.LoadModule<JsonEntityDatabaseModule>();
            
            // Add our debug output pipeline for displaying the collections
            DependencyRegistry.LoadModule<EntityDebugModule>();
            
            base.ResolveApplicationDependencies();
        }

        protected override void ApplicationStarted()
        {
            HandleInput();
        }
        
        private void HandleInput()
        {
            var defaultCollection = EntityDatabase.GetCollection();
            var debugPipeline = DependencyResolver.ResolvePipeline(EntityDebugModule.DebugPipeline);
            var randomBlueprint = new RandomEntityBlueprint();

            while (!_quit)
            {
                Console.Clear();
                Console.WriteLine("Debug this application and look at whats inside the entity database for more info");
                Console.WriteLine("When the application is closed it will save the current entity database");
                Console.WriteLine("Look in the bin folder for an entity-database.json file, alter it if you want");
                Console.WriteLine();
                Console.WriteLine($" - {defaultCollection.Count} Entities Loaded");
                
                // Uncomment this if you want to see all the entity content in console window
                //debugPipeline.Execute(EntityCollectionManager.EntityDatabase);
                
                Console.WriteLine();
                Console.WriteLine(" - Press Enter To Add Another Random Entity");
                Console.WriteLine(" - Press Space To Save and Quit");
                
                var keyPressed = Console.ReadKey();
                if (keyPressed.Key == ConsoleKey.Enter)
                { defaultCollection.CreateEntity(randomBlueprint); }
                else if (keyPressed.Key == ConsoleKey.Spacebar)
                { _quit = true; }
            }

            SaveEntityDatabase();
        }
    }
}