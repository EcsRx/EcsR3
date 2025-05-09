using SystemsR3.Pools;
using SystemsR3.Pools.Config;

namespace SystemsR3.Tests.TestCode
{
    public class SomeTestClass
    {
        
    }

    public class SomeTestMethodClass
    {
        
    }

    public class SomeBaseTestClass
    {
        
    }
    
    public class SomeDerivedTestClass : SomeBaseTestClass
    {
        
    }

    public class TestClass1 : ITestInterface1
    {
        
    }

    public class TestClass2 : ITestInterface2
    {
        
    }

    public class TestPooledObject
    {
        public bool IsDestroyed { get; set; }
    }
    
    public class TestObjectPool : ObjectPool<TestPooledObject>
    {
        public TestObjectPool(PoolConfig config = null) : base(config)
        { }

        public override TestPooledObject Create()
        { return new TestPooledObject(); }

        public override void Destroy(TestPooledObject instance)
        { instance.IsDestroyed = true; }
    }
}