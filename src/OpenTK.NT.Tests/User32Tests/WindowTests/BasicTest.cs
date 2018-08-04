using System.Text;
using OpenTK.NT.Native;
using Xunit;

namespace OpenTK.NT.Tests.User32Tests.WindowTests
{
    public sealed class BasicTest : WindowTest
    {
        public override void RunTest()
        {
            var window = CreateWindow();

            Assert.True(User32.Window.IsWindowVisible(window));

            var builder = new StringBuilder(256);
            Assert.NotEqual(0, User32.Window.GetWindowText(window, builder, builder.Capacity));
            Assert.Equal("Test Window", builder.ToString());
        }
    }
}
