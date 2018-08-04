using System;
using OpenTK.NT.Native;
using Xunit;

namespace OpenTK.NT.Tests
{
    public unsafe class Gdi32Tests
    {
        private readonly IntPtr _deviceContext;

        public Gdi32Tests()
        {
            _deviceContext = User32.DeviceContext.GetDC();
        }

        [Fact]
        public void DescribePixelFormat()
        {
            int pixelFormatAmount = Gdi32.DescribePixelFormat(_deviceContext, 0, PixelFormatDescriptor.SizeInBytes, null);

            Assert.NotEqual(0, pixelFormatAmount);

            var pfd = new PixelFormatDescriptor();
            for (int i = 1; i <= pixelFormatAmount; i++)
            {
                Assert.NotEqual(0, Gdi32.DescribePixelFormat(_deviceContext, i, PixelFormatDescriptor.SizeInBytes, ref pfd));
                Assert.NotEqual(0, pfd.Size);
                // I really don't want to assert anything else here, mostly because I don't want to assume that a certain
                // field of the PFD is always going to be populated / non-zero.
            }
        }

        [Fact]
        public void GetDeviceCaps()
        {
            Assert.NotEqual(0, Gdi32.GetDeviceCaps(_deviceContext, GetDeviceCapsIndex.AspectX));
            Assert.NotEqual(0, Gdi32.GetDeviceCaps(_deviceContext, GetDeviceCapsIndex.AspectY));
            Assert.NotEqual(0, Gdi32.GetDeviceCaps(_deviceContext, GetDeviceCapsIndex.BitsPerPixel));
            Assert.NotEqual(0, Gdi32.GetDeviceCaps(_deviceContext, GetDeviceCapsIndex.HorizontalResolution));
            Assert.NotEqual(0, Gdi32.GetDeviceCaps(_deviceContext, GetDeviceCapsIndex.VerticalResolution));
        }
    }
}
