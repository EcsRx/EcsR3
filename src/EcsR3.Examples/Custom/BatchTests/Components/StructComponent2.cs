using System.Runtime.InteropServices;
using EcsR3.Components;

namespace EcsR3.Examples.Custom.BatchTests.Components
{
    [StructLayout(LayoutKind.Sequential)]
    public struct StructComponent2 : IComponent
    {
        public bool IsTrue;
        public int Value;
    }
}