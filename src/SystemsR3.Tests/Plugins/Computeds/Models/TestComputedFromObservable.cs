using R3;
using SystemsR3.Computeds.Data;

namespace SystemsR3.Tests.Plugins.Computeds.Models;

public class TestComputedFromObservable : ComputedFromObservable<int, int>
{
    public TestComputedFromObservable(ReactiveProperty<int> observable) : base(observable)
    {}

    public override int Transform(int dataSource)
    { return dataSource; }
}