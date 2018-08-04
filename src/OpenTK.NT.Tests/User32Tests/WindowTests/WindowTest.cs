using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using OpenTK.NT.Native;
using Xunit;

namespace OpenTK.NT.Tests.User32Tests.WindowTests
{
    public abstract class WindowTest
    {
        private readonly string _className;
        private readonly IntPtr _hInstance;

        protected WindowTest()
        {
            _className = Guid.NewGuid().ToString();
            _hInstance = Marshal.GetHINSTANCE(typeof(WindowTest).Module);
        }

        protected virtual ExtendedWindowClass GetWindowClass()
        {
            return new ExtendedWindowClass
            {
                Size = ExtendedWindowClass.SizeInBytes,
                ClassName = _className,
                Instance = _hInstance,
                WindowProc = WindowProcedure,
            };
        }

        protected virtual string GetWindowTitle() => "Test Window";

        protected virtual WindowStyles GetWindowStyle() => 0;

        protected virtual ExtendedWindowStyles GetExtendedWindowStyle() => 0;

        protected virtual Point GetWindowPosition() => new Point(300, 200);

        protected virtual System.Drawing.Size GetWindowSize() => new System.Drawing.Size(1280, 720);

        protected virtual IntPtr WindowProcedure(IntPtr window, WindowMessage message, IntPtr wParam, IntPtr lParam)
        {
            switch (message)
            {
                case WindowMessage.Create:
                    User32.Window.ShowWindow(window, ShowWindowCommand.ShowNormal);
                    break;
                case WindowMessage.Close:
                    if (!User32.Window.DestroyWindow(window))
                    {
                        throw new Win32Exception("Window couldn't be destroyed!");
                    }
                    return IntPtr.Zero;
                case WindowMessage.Destroy:
                    if (!User32.WindowClass.UnregisterClass(_className, _hInstance))
                    {
                        throw new Win32Exception("Class couldn't be unregistered!");
                    }
                    break;
            }

            return User32.Window.DefWindowProc(window, message, wParam, lParam);
        }

        protected IntPtr CreateWindow()
        {
            var windowClass = GetWindowClass();

            var classAtom = User32.WindowClass.RegisterClassEx(ref windowClass);
            if (classAtom == 0)
            {
                throw new Win32Exception("Registering the window class failed!");
            }

            var position = GetWindowPosition();
            var size = GetWindowSize();
            var window = User32.Window.CreateWindowEx(GetExtendedWindowStyle(), classAtom, GetWindowTitle(), GetWindowStyle(), position.X, position.Y, size.Width, size.Height, moduleInstance: _hInstance);

            if (window == IntPtr.Zero)
            {
                throw new Win32Exception("Window creation failed!");
            }

            User32.Window.ShowWindow(window, ShowWindowCommand.ShowNormal);

            return window;
        }

        [Fact]
        public abstract void RunTest();
    }
}
