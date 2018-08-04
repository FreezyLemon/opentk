using OpenTK.NT.Native;
using Xunit;

namespace OpenTK.NT.Tests.User32Tests.WindowTests
{
    public sealed class ClientToScreenTest : WindowTest
    {
        public override void RunTest()
        {
            var window = CreateWindow();

            int x = 150;
            int y = 250;
            var point = new Point(x, y);

            Assert.True(User32.Window.ClientToScreen(window, ref point));
            // The offset isn't exactly x/y because there are also title bars etc. to include
            var windowPos = GetWindowPosition();
            Assert.True(point.X >= x + windowPos.X);
            Assert.True(point.Y >= y + windowPos.Y);

            Assert.True(User32.Window.ScreenToClient(window, ref point));
            Assert.Equal(x, point.X);
            Assert.Equal(y, point.Y);
        }
    }
}
