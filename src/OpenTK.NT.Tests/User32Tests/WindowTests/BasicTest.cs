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
        }
    }
}
