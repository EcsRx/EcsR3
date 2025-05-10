using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;
using EcsR3.Plugins.Persistence.Modules;
using EcsR3.Plugins.Persistence.Pipelines;
using LazyData.Json.Handlers;
using LazyData.Mappings.Mappers;
using LazyData.Mappings.Types;
using LazyData.Registries;
using LazyJsonSerializer = LazyData.Json.JsonSerializer;
using LazyJsonDeserializer = LazyData.Json.JsonDeserializer;
using Persistity.Serializers.LazyData.Json;

namespace EcsR3.Examples.ExampleApps.LoadingEntityDatabase.Modules
{
    public class JsonEntityDatabaseModule : IDependencyModule
    {
        public const string CustomEntityDatabaseFile = "entity-database.json";
        
        public void Setup(IDependencyRegistry registry)
        {
            // Override our default save pipeline (binary ones) with the json one
            registry.Unbind<ISaveEntityCollectionPipeline>();
            registry.Bind<ISaveEntityCollectionPipeline>(builder =>
                builder.ToMethod(CreateJsonSavePipeline).AsSingleton());
            
            // Override our default load pipeline (binary ones) with the json one
            registry.Unbind<ILoadEntityCollectionPipeline>();
            registry.Bind<ILoadEntityCollectionPipeline>(builder =>
                builder.ToMethod(CreateJsonLoadPipeline).AsSingleton());
        }

        public ISaveEntityCollectionPipeline CreateJsonSavePipeline(IDependencyResolver resolver)
        {
            // We manually create our serializer here as we dont want the default behaviour which
            // which would be to only persist things with `[Persist]` and `[PersistData]` attributes
            // we want to persist EVERYTHING
            var mappingRegistry = new MappingRegistry(resolver.Resolve<EverythingTypeMapper>());
            var primitiveTypeMappings = resolver.ResolveAll<IJsonPrimitiveHandler>();
            
            // Create the lazy serializer to serialize everything, then wrap it in the persistity one
            var everythingSerializer = new LazyJsonSerializer(mappingRegistry, primitiveTypeMappings);
            var serializer = new JsonSerializer(everythingSerializer);
            
            // Piggyback off the existing save pipeline helper, which lets you set your format and filename
            return PersistityModule.CreateSavePipeline(resolver, serializer, CustomEntityDatabaseFile);
        }
        
        public ILoadEntityCollectionPipeline CreateJsonLoadPipeline(IDependencyResolver resolver)
        {
            // Manually build deserializer as we want to load everything
            var mappingRegistry = new MappingRegistry(resolver.Resolve<EverythingTypeMapper>());
            var typeCreator = resolver.Resolve<ITypeCreator>();
            var primitiveTypeMappings = resolver.ResolveAll<IJsonPrimitiveHandler>();

            // Create deserializer for everything
            var everythingDeserializer = new LazyJsonDeserializer(mappingRegistry, typeCreator, primitiveTypeMappings);
            var deserializer = new JsonDeserializer(everythingDeserializer);
            
            // Use existing load pipeline helper to customize format and filename
            return PersistityModule.CreateLoadPipeline(resolver, deserializer, CustomEntityDatabaseFile);
        }
        
    }
}