/* Licensed under the MIT/X11 license.
 * Copyright (c) 2011 Xamarin, Inc.
 * Copyright 2013 Xamarin Inc
 * This notice may not be removed from any source distribution.
 * See license.txt for licensing detailed licensing details.
 */

using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Platform;
using OpenTK.Platform.Android;

using ES30 = OpenTK.Graphics.ES30;

namespace OpenTK
{
    internal sealed class GLCalls
    {
        public GLVersion Version;

        public delegate void glScissor (int x, int y, int width, int height);
        public delegate void glViewport (int x, int y, int width, int height);

        public glScissor Scissor;
        public glViewport Viewport;

        public static GLCalls GetGLCalls (GLVersion api)
        {
            switch (api) {
            case GLVersion.ES3:
                return CreateES3 ();
            }
            throw new ArgumentException ("api");
        }

        public static GLCalls CreateES3 ()
        {
            return new GLCalls () {
                Version                 = GLVersion.ES3,
                Scissor                 = (x, y, w, h)        => ES30.GL.Scissor(x, y, w, h),
                Viewport                = (x, y, w, h)        => ES30.GL.Viewport(x, y, w, h),
                };
        }
    }
}
