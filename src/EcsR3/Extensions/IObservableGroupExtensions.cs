using System.Collections.Generic;
using EcsR3.Entities;
using EcsR3.Groups.Observable;

namespace EcsR3.Extensions
{
    public static class IObservableGroupExtensions
    {
        public static IEnumerable<IEntity> Query(this IObservableGroup observableGroupAccesssor, IObservableGroupQuery query)
        { return query.Execute(observableGroupAccesssor); }
    }
}