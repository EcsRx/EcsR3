using R3;
using SystemsR3.Computeds.Data;

namespace SystemsR3.Tests.Plugins.Computeds.Models
{
    public class TestComputedFromData : ComputedFromData<int, DummyData>
    {
        public Subject<Unit> ManuallyRefresh = new Subject<Unit>();
        
        public TestComputedFromData(DummyData data) : base(data)
        {}

        public override Observable<Unit> RefreshWhen()
        { return ManuallyRefresh; }

        public override int Transform(DummyData data)
        { return data.Data; }
    }
}