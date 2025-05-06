using System;
using System.Threading.Tasks;
using SystemsR3.Infrastructure.Dependencies;
using EcsR3.Plugins.Persistence.Extensions;
using EcsR3.Plugins.Persistence.Pipelines;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Persistity.Core;

namespace EcsR3.Examples.ExampleApps.LoadingEntityDatabase.Modules
{
    public class EntityDebugModule : IDependencyModule
    {
        public const string DebugPipeline = "DebugPipeline";
        
        public void Setup(IDependencyRegistry registry)
        {
            // Make a new pipeline for showing json output
            registry.BuildPipeline(DebugPipeline, builder => builder
                // Fork from the 2nd step (serializer) on the existing JSON Saving Pipeline
                .ForkDataFrom<ISaveEntityCollectionPipeline>(2)
                // Then spit out the json to the console in a nice way
                .ThenInvoke(WriteEntityData));
        }

        private Task<DataObject> WriteEntityData(DataObject data)
        {
            var prettyText = JToken.Parse(data.AsString).ToString(Formatting.Indented);
            Console.WriteLine(prettyText);
            return Task.FromResult(data);
        }
    }
}