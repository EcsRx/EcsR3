using System;
using SystemsR3.Utility;
using Xunit;

namespace SystemsR3.Tests.Utility;

public class RefBufferTests
{
    struct TestStruct
    {
        public int Data;
    }
    
    [Fact]
    public void should_pass_refs_with_ref_buffer()
    {
        var someStruct1 = new TestStruct() { Data = 1 };
        var someStruct2 = new TestStruct() { Data = 2 };
        var someStruct3 = new TestStruct() { Data = 3 };
        var arrayOfStructs = new[] { someStruct1, someStruct2, someStruct3 };

        var refBuffer = new RefBuffer<TestStruct>(arrayOfStructs, [0, 2]);

        for (var i = 0; i < refBuffer.Count; i++)
        {
            ref var refData = ref refBuffer[i];
            refData.Data += 20;
        }
            
        Assert.Equal(21, arrayOfStructs[0].Data);
        Assert.Equal(2, arrayOfStructs[1].Data);
        Assert.Equal(23, arrayOfStructs[2].Data);
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    public void should_throw_out_of_range_exception_if_index_is_out_of_range(int index)
    {
        var someStruct1 = new TestStruct() { Data = 1 };
        var someStruct2 = new TestStruct() { Data = 2 };
        var someStruct3 = new TestStruct() { Data = 3 };
        var arrayOfStructs = new[] { someStruct1, someStruct2, someStruct3 };

        var refBuffer = new RefBuffer<TestStruct>(arrayOfStructs, [0, 2]);

        Assert.Throws<IndexOutOfRangeException>(() =>
        {
            var data = refBuffer[index];
        });
    }
}