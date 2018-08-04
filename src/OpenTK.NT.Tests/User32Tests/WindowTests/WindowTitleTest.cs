using System.Text;
using OpenTK.NT.Native;
using Xunit;

namespace OpenTK.NT.Tests.User32Tests.WindowTests
{
    public sealed class WindowTitleTest : WindowTest
    {
        public override void RunTest()
        {
            var window = CreateWindow();

            // "+ 1" needed for \0 character at the end
            var builder = new StringBuilder(GetWindowTitle().Length + 1);
            Assert.Equal(GetWindowTitle().Length, User32.Window.GetWindowText(window, builder, builder.Capacity));
            Assert.Equal(GetWindowTitle(), builder.ToString());
        }

        protected override string GetWindowTitle() => "Any Unicode text in this should work!+#-äöüôàíæƌʤ˩βϰЦіխעڻ١ṡ–";
    }
}
