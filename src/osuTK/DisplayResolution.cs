/* Licensed under the MIT/X11 license.
 * Copyright (c) 2006-2008 the osuTK team.
 * This notice may not be removed.
 * See license.txt for licensing detailed licensing details.
 */

using System;
using System.Collections.Generic;
using System.Drawing;

namespace osuTK
{
    /// <summary>Contains information regarding a monitor's display resolution.</summary>
    public class DisplayResolution : IComparable<DisplayResolution>, IEquatable<DisplayResolution>
    {
        private static readonly Dictionary<(double upper, double lower), string> aspectRatios = new Dictionary<(double upper, double lower), string>
        {
            { (1.77, 1.8), "16:9" },
            { (1.33, 1.34), "4:3" },
            { (1.5, 1.5), "3:2" },
            { (1.66, 1.67), "5:3" },
            { (1.25, 1.25), "5:4" },
            { (1.6, 1.6), "16:10" },
            { (1.88, 1.89), "17:9" },
            { (2, 2), "18:9" },
            { (2.33, 2.34), "21:9" },
        };
        
        private Rectangle bounds;

        internal DisplayResolution() { }

        // Creates a new DisplayResolution object for the primary DisplayDevice.
        internal DisplayResolution(int x, int y, int width, int height, int bitsPerPixel, float refreshRate)
        {
            // Refresh rate may be zero, since this information may not be available on some platforms.
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException("width", "Must be greater than zero.");
            }
            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException("height", "Must be greater than zero.");
            }
            if (bitsPerPixel <= 0)
            {
                throw new ArgumentOutOfRangeException("bitsPerPixel", "Must be greater than zero.");
            }
            if (refreshRate < 0)
            {
                throw new ArgumentOutOfRangeException("refreshRate", "Must be greater than, or equal to zero.");
            }

            this.bounds = new Rectangle(x, y, width, height);
            this.BitsPerPixel = bitsPerPixel;
            this.RefreshRate = refreshRate;

            double ratio = width / (double) height;
            foreach (var aspectRatio in aspectRatios.Keys)
            {
                if (ratio >= aspectRatio.lower && ratio <= aspectRatio.upper)
                {
                    AspectRatio = aspectRatios[aspectRatio];
                    break;
                }
            }
        }

        /// <summary>
        /// Gets a System.Drawing.Rectangle that contains the bounds of this display device.
        /// </summary>
        [Obsolete("This property will return invalid results if a monitor changes resolution. Use DisplayDevice.Bounds instead.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public Rectangle Bounds
        {
            get { return bounds; }
        }

        /// <summary>Gets a System.Int32 that contains the width of this display in pixels.</summary>
        public int Width
        {
            get { return bounds.Width; }
            internal set { bounds.Width = value; }
        }

        /// <summary>Gets a System.Int32 that contains the height of this display in pixels.</summary>
        public int Height
        {
            get { return bounds.Height; }
            internal set { bounds.Height = value; }
        }

        /// <summary>Gets a System.Int32 that contains number of bits per pixel of this display. Typical values include 8, 16, 24 and 32.</summary>
        public int BitsPerPixel { get; internal set; }

        /// <summary>
        /// Gets a System.Single representing the vertical refresh rate of this display.
        /// </summary>
        public float RefreshRate { get; internal set; }

        /// <summary>
        /// A friendly name for the proportions between width and height of this resolution.
        /// </summary>
        public string AspectRatio { get; } = "Unknown";
        
        /// <summary>
        /// Returns a System.String representing this DisplayResolution.
        /// </summary>
        /// <returns>A System.String representing this DisplayResolution.</returns>
        public override string ToString()
        {
            #pragma warning disable 612,618
            return String.Format("{0}x{1}@{2}Hz", Bounds, BitsPerPixel, RefreshRate);
            #pragma warning restore 612,618
        }

        public int CompareTo(DisplayResolution other)
        {
            // Descending height
            if (other.Height - Height != 0)
            {
                return other.Height - Height;
            }

            // Descending width
            if (other.Width - Width != 0)
            {
                return other.Width - Width;
            }

            // Descending refresh rate
            if (RefreshRate > other.RefreshRate)
            {
                return -1;
            }

            if (RefreshRate < other.RefreshRate)
            {
                return 1;
            }

            // Descending bits per pixel
            return other.BitsPerPixel - BitsPerPixel;
        }

        public bool Equals(DisplayResolution other)
        {
            if (other == null)
            {
                return false;
            }

            return
                Width == other.Width &&
                Height == other.Height &&
                BitsPerPixel == other.BitsPerPixel &&
                RefreshRate == other.RefreshRate;
        }

        /// <summary>Determines whether the specified resolutions are equal.</summary>
        /// <param name="obj">The System.Object to check against.</param>
        /// <returns>True if the System.Object is an equal DisplayResolution; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (this.GetType() == obj.GetType())
            {
                return Equals((DisplayResolution) obj);
            }

            return false;
        }

        /// <summary>Returns a unique hash representing this resolution.</summary>
        /// <returns>A System.Int32 that may serve as a hash code for this resolution.</returns>
        public override int GetHashCode()
        {
            #pragma warning disable 612,618
            return Bounds.GetHashCode() ^ BitsPerPixel ^ RefreshRate.GetHashCode();
            #pragma warning restore 612,618
        }

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator== (DisplayResolution left, DisplayResolution right)
        {
            if (((object)left) == null && ((object)right) == null)
            {
                return true;
            }
            else if ((((object)left) == null && ((object)right) != null) ||
                     (((object)left) != null && ((object)right) == null))
            {
                return false;
            }
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(DisplayResolution left, DisplayResolution right)
        {
            return !(left == right);
        }
    }
}
