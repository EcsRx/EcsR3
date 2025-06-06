using R3;
using SystemsR3.Computeds.Conventions;

namespace SystemsR3.Tests.Plugins.Computeds.Models;

public class TestComputedFromObservable : ComputedFromObservable<int, int>
{
    public TestComputedFromObservable(ReactiveProperty<int> observable) : base(observable)
    {}

    public void ForceDataRefresh(int value) => RefreshData(value);

    protected override bool UpdateComputedData(int data)
    {
        var willChange = ComputedData != data;
        ComputedData = data;
        return willChange;
    }
}