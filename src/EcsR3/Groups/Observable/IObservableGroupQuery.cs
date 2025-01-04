using System.Collections.Generic;
using EcsR3.Entities;

namespace EcsR3.Groups.Observable
{
    public interface IObservableGroupQuery
    {
        IEnumerable<IEntity> Execute(IObservableGroup observableGroup);
    }
}