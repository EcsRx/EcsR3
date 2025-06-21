# Breaking Change Summaries

See individual release notes for more information, but here is a quick summary of each major versions breaking changes (if any).

- `8.0` - Split from `EcsRx` and have made this its own repo called `EcsR3`
- `9.0` - Changed how routing for entity changes were handled and removed `IEntityDatabase`
- `10.0` - Refactored almost all `IComputed` related classes and contracts, `Batching` plugin is no more and included in core
- `11.0` - `IEntity` no longer exists, just `Entity`, components are no longer accessed DIRECTLY on entity instead via `IEntityComponentAccessor`, removed `IReactToGroupEx`