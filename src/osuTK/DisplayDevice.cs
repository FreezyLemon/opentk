//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2010 the Open Toolkit library.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace osuTK
{
    /// <summary>
    /// Defines a display device on the underlying system, and provides
    /// methods to query and change its display parameters.
    /// </summary>
    public class DisplayDevice
    {
        // TODO: Add properties that describe the 'usable' size of the Display, i.e. the maximized size without the taskbar etc.
        // TODO: Does not detect changes to primary device.

        private bool primary;
        private Rectangle bounds;
        private DisplayResolution current_resolution = new DisplayResolution();
        private List<DisplayResolution> available_resolutions = new List<DisplayResolution>();
        private IList<DisplayResolution> available_resolutions_readonly;

        internal object Id; // A platform-specific id for this monitor

        private static readonly object display_lock = new object();
        private static DisplayDevice primary_display;

        private static Platform.IDisplayDeviceDriver implementation;

        static DisplayDevice()
        {
            implementation = Platform.Factory.Default.CreateDisplayDeviceDriver();
        }

        public DisplayDevice()
        {
            available_resolutions_readonly = available_resolutions.AsReadOnly();
        }

        public DisplayDevice(DisplayResolution currentResolution, bool primary,
            IEnumerable<DisplayResolution> availableResolutions, Rectangle bounds,
            object id)
            : this()
        {
            // Todo: Consolidate current resolution with bounds? Can they fall out of sync right now?
            this.current_resolution = currentResolution;
            IsPrimary = primary;
            this.available_resolutions.AddRange(availableResolutions.Distinct());
            available_resolutions.Sort();
            #pragma warning disable 612,618
            this.bounds = bounds == Rectangle.Empty ? currentResolution.Bounds : bounds;
            #pragma warning restore 612,618
            this.Id = id;
        }

        /// <summary>
        /// Gets the bounds of this instance in pixel coordinates..
        /// </summary>
        public Rectangle Bounds
        {
            get { return bounds; }
            set
            {
                bounds = value;
                current_resolution.Height = bounds.Height;
                current_resolution.Width = bounds.Width;
            }
        }

        /// <summary>Gets a System.Int32 that contains the width of this display in pixels.</summary>
        public int Width { get { return current_resolution.Width; } }

        /// <summary>Gets a System.Int32 that contains the height of this display in pixels.</summary>
        public int Height { get { return current_resolution.Height; } }

        /// <summary>Gets a System.Int32 that contains number of bits per pixel of this display. Typical values include 8, 16, 24 and 32.</summary>
        public int BitsPerPixel
        {
            get { return current_resolution.BitsPerPixel; }
            set { current_resolution.BitsPerPixel = value; }
        }

        /// <summary>
        /// Gets a System.Single representing the vertical refresh rate of this display.
        /// </summary>
        public float RefreshRate
        {
            get { return current_resolution.RefreshRate; }
            set { current_resolution.RefreshRate = value; }
        }

        /// <summary>Gets a System.Boolean that indicates whether this Display is the primary Display in systems with multiple Displays.</summary>
        public bool IsPrimary
        {
            get { return primary; }
            set
            {
                if (value && primary_display != null && primary_display != this)
                {
                    primary_display.IsPrimary = false;
                }

                lock (display_lock)
                {
                    primary = value;
                    if (value)
                    {
                        primary_display = this;
                    }
                }
            }
        }

        /// <summary>
        /// Selects an available resolution that matches the specified parameters.
        /// </summary>
        /// <param name="width">The width of the requested resolution in pixels.</param>
        /// <param name="height">The height of the requested resolution in pixels.</param>
        /// <param name="bitsPerPixel">The bits per pixel of the requested resolution.</param>
        /// <param name="refreshRate">The refresh rate of the requested resolution in hertz.</param>
        /// <returns>The requested DisplayResolution or null if the parameters cannot be met.</returns>
        /// <remarks>
        /// <para>If a matching resolution is not found, this function will retry ignoring the specified refresh rate,
        /// bits per pixel and resolution, in this order. If a matching resolution still doesn't exist, this function will
        /// return the current resolution.</para>
        /// <para>A parameter set to 0 or negative numbers will not be used in the search (e.g. if refreshRate is 0,
        /// any refresh rate will be considered valid).</para>
        /// <para>This function allocates memory.</para>
        /// </remarks>
        public DisplayResolution SelectResolution(int width, int height, int bitsPerPixel, float refreshRate)
        {
            // since our resolutions are sorted, we get the resolution "as specified", with BPP and/or refresh rate
            // being the highest available if specified as 0 in the method call.
            return AvailableResolutions.FirstOrDefault(res =>
                res.Width == width &&
                res.Height == height &&
                (bitsPerPixel == 0 || res.BitsPerPixel == bitsPerPixel) &&
                (refreshRate == 0f || Math.Abs(res.RefreshRate - refreshRate) < 1f)
            );
        }

        /// <summary>
        /// Gets the list of <see cref="DisplayResolution"/> objects available on this device.
        /// </summary>
        public IList<DisplayResolution> AvailableResolutions
        {
            get { return available_resolutions_readonly; }
            set
            {
                available_resolutions = (List<DisplayResolution>)value;
                available_resolutions.Sort();
                available_resolutions_readonly = available_resolutions.AsReadOnly();
            }
        }

        /// <summary>Changes the resolution of the DisplayDevice.</summary>
        /// <param name="resolution">The resolution to set. <see cref="DisplayDevice.SelectResolution"/></param>
        /// <exception cref="Graphics.GraphicsModeException">Thrown if the requested resolution could not be set.</exception>
        /// <remarks>If the specified resolution is null, this function will restore the original DisplayResolution.</remarks>
        public void ChangeResolution(DisplayResolution resolution)
        {
            if (resolution == null)
            {
                this.RestoreResolution();
            }

            if (resolution == current_resolution)
            {
                return;
            }

            //effect.FadeOut();

            if (implementation.TryChangeResolution(this, resolution))
            {
                if (OriginalResolution == null)
                {
                    OriginalResolution = current_resolution;
                }
                current_resolution = resolution;
            }
            else
            {
                throw new Graphics.GraphicsModeException(String.Format("Device {0}: Failed to change resolution to {1}.",
                    this, resolution));
            }

            //effect.FadeIn();
        }

        /// <summary>Changes the resolution of the DisplayDevice.</summary>
        /// <param name="width">The new width of the DisplayDevice.</param>
        /// <param name="height">The new height of the DisplayDevice.</param>
        /// <param name="bitsPerPixel">The new bits per pixel of the DisplayDevice.</param>
        /// <param name="refreshRate">The new refresh rate of the DisplayDevice.</param>
        /// <exception cref="Graphics.GraphicsModeException">Thrown if the requested resolution could not be set.</exception>
        public void ChangeResolution(int width, int height, int bitsPerPixel, float refreshRate)
        {
            this.ChangeResolution(this.SelectResolution(width, height, bitsPerPixel, refreshRate));
        }

        /// <summary>Restores the original resolution of the DisplayDevice.</summary>
        /// <exception cref="Graphics.GraphicsModeException">Thrown if the original resolution could not be restored.</exception>
        public void RestoreResolution()
        {
            if (OriginalResolution != null)
            {
                //effect.FadeOut();

                if (implementation.TryRestoreResolution(this))
                {
                    current_resolution = OriginalResolution;
                    OriginalResolution = null;
                }
                else
                {
                    throw new Graphics.GraphicsModeException(String.Format("Device {0}: Failed to restore resolution.", this));
                }

                //effect.FadeIn();
            }
        }

        /// <summary>Gets the default (primary) display of this system.</summary>
        public static DisplayDevice Default
        {
            get { return implementation.GetDisplay(DisplayIndex.Primary); }
        }

        /// <summary>
        /// Gets the <see cref="DisplayDevice"/> for the specified <see cref="DisplayIndex"/>.
        /// </summary>
        /// <param name="index">The <see cref="DisplayIndex"/> that defines the desired display.</param>
        /// <returns>A <see cref="DisplayDevice"/> or null, if no device corresponds to the specified index.</returns>
        public static DisplayDevice GetDisplay(DisplayIndex index)
        {
            return implementation.GetDisplay(index);
        }

        /// <summary>
        /// Gets the original resolution of this instance.
        /// </summary>
        internal DisplayResolution OriginalResolution { get; set; }

        internal static DisplayDevice FromPoint(int x, int y)
        {
            for (DisplayIndex i = DisplayIndex.First; i < DisplayIndex.Sixth; i++)
            {
                DisplayDevice display = DisplayDevice.GetDisplay(i);
                if (display != null)
                {
                    if (display.Bounds.Contains(x, y))
                    {
                        return display;
                    }
                }
            }
            return null;
        }

        /// <summary> 
        /// Gets the <see cref="DisplayDevice"/> that has the largest intersecting area with a specific <see cref="Rectangle"/> structure. 
        /// </summary> 
        /// <param name="bounds">The <see cref="Rectangle"/> (usually window bounds) for which we want to find the <see cref="DisplayDevice"/>.</param> 
        /// <returns>A <see cref="DisplayDevice"/> that contains the biggest part of the specified <see cref="Rectangle"/> out of all available devices.</returns> 
        public static DisplayDevice FromRectangle(Rectangle bounds)
        {
            DisplayDevice result = null;
            int biggestArea = 0;
            foreach (var device in implementation.AvailableDevices.Where(d => d.Bounds.IntersectsWith(bounds)))
            {
                Rectangle windowPortionOnScreen = device.Bounds;
                windowPortionOnScreen.Intersect(bounds);

                int windowPortionArea = windowPortionOnScreen.Width * windowPortionOnScreen.Height;

                if (windowPortionArea > biggestArea)
                {
                    biggestArea = windowPortionArea;
                    result = device;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a System.String representing this DisplayDevice.
        /// </summary>
        /// <returns>A System.String representing this DisplayDevice.</returns>
        public override string ToString()
        {
            return (IsPrimary ? "Primary" : "Secondary") + $" display ({Width}x{Height}@{RefreshRate}Hz) with {available_resolutions.Count} modes available";
        }
    }
}
