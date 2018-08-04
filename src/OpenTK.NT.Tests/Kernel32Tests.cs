using System;
using OpenTK.NT.Native;
using Xunit;

namespace OpenTK.NT.Tests
{
    public class Kernel32Tests
    {
        // These two QueryPerformance functions should never fail on Win XP or later.
        [Fact]
        public void QueryPerformanceFrequency()
        {
            Assert.True(Kernel32.QueryPerformanceFrequency(out long freq));

            Assert.True(freq > 0);
        }

        [Fact]
        public void QueryPerformanceCounter()
        {
            Assert.True(Kernel32.QueryPerformanceCounter(out long perfCount));

            Assert.True(perfCount > 0);
        }

        [Fact]
        public void GetModuleHandle()
        {
            var result = Kernel32.GetModuleHandle();
            Assert.NotEqual(IntPtr.Zero, result);
        }
    }
}
