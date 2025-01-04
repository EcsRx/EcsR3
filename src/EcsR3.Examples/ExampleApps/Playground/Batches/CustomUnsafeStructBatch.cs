using System.Runtime.InteropServices;
using EcsR3.Examples.ExampleApps.Playground.Components;

namespace EcsR3.Examples.ExampleApps.Playground.Batches
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CustomUnsafeStructBatch
    {
        public int EntityId;
        public StructComponent Basic;
        public StructComponent2 Basic2;
    }
}