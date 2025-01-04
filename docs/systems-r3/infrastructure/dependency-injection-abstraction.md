# Dependency Injection Abstraction

As this framework is built to enable plugin development and work on any platform/framework, there has been some efforts to streamline how DI is handled within the framework by creating an abstraction over the underlying DI system.

## But why?

So lets say you wanted to create an RPG plugin where it contained components, events, systems etc for handling buffs, items, inventory etc. Now none of this logic really is dependent on Unity, Monogame etc... its all just raw .net, but you will want to setup the DI concerns for this plugin so it can be loaded into any of those platforms and just work.

To be able to do that we need to have the notion of DI in the framework without having an ACTUAL DI container available. As there would be no point having a hard dependency on Zenject for your plugin if you wanted to consume it in the Monogame world etc.

> You may be thinking `"why have your own abstraction when Microsoft has now got its own DI abstractions?"` and thats a really good point, there are 2 reasons. First being that not all DI frameworks (especially more game dev specific ones) adhere to the MS one so we cant rely upon them conforming to that interface. 
> 
> Second there is some implicit behaviour in the MS DI framework that is not entirely consistent, such as how keyed/named and non named services are segregated, different DI frameworks do this differently, so we wrap some underlying logic to give the same behaviour across all implementations.

## `IDependencyRegistry` and `IDependencyResolver`

There are 2 main interfaces that are used to make DI possible, one is the `IDependencyRegistry` which is responisbile for the binding/registering of dependencies and is used in the first stage of setting up dependency trees.

Then there is the latter `IDependencyResolver` which is responsible for resolving the registered dependencies. For the most part you wont need to worry about how these get handled internally, you just need to be aware of which one you want to use.

### `IDependencyRegistry` Features

#### Binding - `Bind<From, To>`, `Bind<T>`, `Unbind<T>`

This lets you bind a given type to another type, so it could be `Bind<ISomething, Something>()` or if you want to self bind the concrete type just do `Bind<Something>()`. You can also unbind a type by using `Unbind<Something>()`. You can also add named bindings and other configuration by passing in a configuration object (discussed further on).

#### Modules/Setup - `LoadModule<T>`, `LoadModule(IDependencyModule)`

There is also support for creating modules that setup your DI configuration, this requires you to implement `IDependencyModule` and has a `Setup` method which provides you the container to setup your bindings on.

### Binding Configuration

If you want to configure how a binding should work, you can pass into the `Bind` methods an optional configuration object which exposes the current properties.

### AsSingleton: `bool`

Setting this to `true` will mean that only one instance of this binding should exist, so if you were to do:
```csharp
dependencyRegistry.Bind<IEventSystem, EventSystem>(new BindingConfiguration{AsSingleton = true});
```
Then resolve `IEventSystem` in multiple places you will always get back the same instance, which is extremely handy for infrastructure style objects which should act as singletons. If you provide `false` as the value it will return a new instance for every resolve request.

### WithName: `string`

This will allow you to give the binding a name for resolving via name.

### ToInstance: `object`

This allows you to bind to an actual instance of an object rather than a type, which is useful if you need to manually setup something yourself.

```csharp
var someInstance = new Something(foo, bar);
dependencyRegistry.Bind<ISomething>(new BindingConfiguration{ToInstance = someInstance});
```

### ToMethod: `Func<IDependencyContainer, object>`

This allows you to lazy bind something to a method rather than an instance/type, which is useful if you want to setup something in a custom way once all DI configuration has been processed.

```csharp
var bindingConfiguration = new BindingConfiguration({
    ToMethod: container =>
      {
          var foo = container.Resolve<Foo>();
          return new Something(foo, "woop woop");
      });
});
dependencyRegistry.Bind<ISomething>(bindingConfiguration);
```

There is a slightly nicer way to set this up using a builder pattern extension shown further on.

### `IDependencyResolver` Features

#### Resolving - `Resolve<T>`, `ResolveAll<T>`

This lets you get an instance of something from the DI container, if you want a single instance do `Resolve<T>()` or if you want all instances matching that type do `ResolveAll<T>()` which returns an enumerable of the given type. You can also request an instance of a type with a given name providing you have bound the type with a name, by doing `Resolve<ISomething>("something-1")` which would return the implementation of `Something` named "something-1".

## Extension helpers

The configuration object is simple but can be unsightly for larger configurations, to assist in this we have added a couple of extension methods which can be used to make your binding config a little more succinct.

### Builder helpers

There is a builder pattern helper which lets you setup your binding config via a builder rather than an instance of `BindingConfiguration`, this can be used by just creating a lambda within the bind method like so:

```csharp
dependencyRegistry.Bind<ISomething>(config => config
  .AsSingleton()
  .WithName("something-1")
});
```

This lets you setup the configuration in a nice way, it also has type safety so you can setup instances and methods using it like so:

```csharp
// With instance
dependencyRegistry.Bind<ISomething>(config => config
  .ToInstance(new InstanceOfISomething())
});

// Wouldnt work, incorrect type
dependencyRegistry.Bind<ISomething>(config => config
  .ToInstance(new InstanceOfISomethingElse()) // error, not ISomething
});

// With method
dependencyRegistry.Bind<ISomething>(config => config
  .ToMethod(dependencyResolver =>
  {
      var foo = dependencyResolver.Resolve<Foo>();
      return new Something(foo, "woop woop");
  })
});
```

## Breaking Changes in 7.*

The following old DI methods have been removed as not all DI frameworks supported them (mainly Microsoft DI *sigh*).

- `OnActivation`
- `WhenInjectedInto`
- `WithNamedConstructorArgs`
- `WithTypedConstructorArgs`

These old methods used to exist and provide a cross platform way of being able to express these concerns, however most of this can still be handled by using the method based resolution and adding a custom activation function in as a side effect.

The only one which doesnt have a way to resolve really is the `WhenInjectedInto` scenario but that is extremely niche and can be handed via facade interfaces or some other approach that just proxies the type and is used for the various classes which need specific implementations.

> You still have access to the underlying `NativeRegistry` and `NativeResolver` so you can still use the native equivalents to express these concerns if you need them, just be aware in cross platform plugins any native code will limit the platforms your plugin can be used in.