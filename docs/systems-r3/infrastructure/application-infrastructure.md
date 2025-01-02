# Infrastructure

As part of SystemsR3 there is some basic infrastructure provided for you (if you choose to use `SystemsR3.Infrastructure`), this contains:

- A dependency injection abstraction system (So you can consume DI on any platform with any DI framework)
- An `ISystemsR3Application` interface as well as a default implementation `SystemsR3Application` (So you can start your app in a consistent way)
- A plugin framework via `ISystemsR3Plugin` (so you can write your own plugins which can be re-used across many projects and shared with others)
- A default `EventSystem` (So you can send events around your application, which implements `IEventSystem`)

All of this combined basically provides you an entry point to start creating your applications.

## Why use this?

To have some sort of consistency and contract in place for extensibility, for example by adding the infrastructure you can out the box consume any SystemsR3 plugins (assuming they dont contain any native platform code), you can also make use of specific lifetime methods and conventions.

If you have a specific scenario and dont want to use the built in infrastructure then can easily just ignore this and put your own stuff in place, but this would then mean you are then incompatible with a lot of good stuff that comes with the consistency and the community all adhering to those contracts.
