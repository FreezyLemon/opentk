using Xunit;
using OpenTK.NT.Native;
using System;

namespace OpenTK.NT.Tests.User32Tests
{
    public class CursorTests
    {
        [Fact]
        public void LoadCursor()
        {
            var cursor = User32.Cursor.LoadCursor(CursorName.Hand);
            Assert.NotEqual(IntPtr.Zero, cursor);
        }

        [Fact]
        public void GetCursor()
        {
            Assert.NotEqual(IntPtr.Zero, User32.Cursor.GetCursor());
        }

        [Fact]
        public void GetCursorPos()
        {
            Assert.True(User32.Cursor.GetCursorPos(out var point));
        }

        [Fact]
        public void SetCursorPos()
        {
            Assert.True(User32.Cursor.SetCursorPos(0, 0));
        }
    }
}
