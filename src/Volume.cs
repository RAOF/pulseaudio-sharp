//
//  Copyright © 2009 Christopher James Halse Rogers <raof@ubuntu.com>
//
//  Volume.cs is a part of Pulseaudio#
//
//  Pulseaudio# is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  Pulseaudio# is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with Pulseaudio#.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Pulseaudio
{
    public class Constants
    {
        public const int MaxChannels = 32;
    }

    [StructLayout (LayoutKind.Sequential)]
    public class Volume
    {
        private Byte channels;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst=Constants.MaxChannels)]
        private UInt32 [] values;

        private static readonly UInt32 norm = 0x10000U;
        private static readonly UInt32 mute = 0;

        public Volume()
        {
            values = new uint[Constants.MaxChannels];
            pa_cvolume_init (this);
            channels = 2;
        }

        /// <summary>
        /// Reset the volume of all channels to no change (100%)
        /// </summary>
        public void Reset ()
        {
            pa_cvolume_set (this, channels, norm);
        }

        /// <summary>
        /// Set the volume of all channels.
        /// </summary>
        /// <param name="val">
        /// A <see cref="System.Double"/>.  The nominal range is 0 - 1.0, with 0 corresponding to mute
        /// and 1.0 corresponding to full volume (or, equivalently, no volume adjustment).
        /// It is possible to set values > 1.0.  This is not an error, but is not generally a good idea.
        /// </param>
        public void Set (double val)
        {
            pa_cvolume_set (this, channels,  (UInt32)(val * norm));
        }

        public Volume Copy ()
        {
            Volume retVal = new Volume ();
            retVal.channels = channels;
            values.CopyTo (retVal.values, 0);
            return retVal;
        }

        public override bool Equals (object obj)
        {
            if (obj is Volume) {
                return (this as Volume) == (obj as Volume);
            }
            return false;
        }

        public override int GetHashCode ()
        {
            return base.GetHashCode ();
        }

        public static bool operator== (Volume a, Volume b)
        {
            return pa_cvolume_equal (a, b) != 0;
        }
        public static bool operator!= (Volume a, Volume b)
        {
            return !(a==b);
        }

        public override string ToString ()
        {
            StringBuilder buffer = new StringBuilder (PA_CVOLUME_SNPRINT_BUFFER_MAX);
            pa_cvolume_snprint (buffer, new IntPtr (buffer.Capacity), this);
            return buffer.ToString ();
        }

        public double ToScalarAvg ()
        {
            UInt32 rawVol = pa_cvolume_avg (this);
            return ((((double)rawVol) - mute) / norm);
        }

        [DllImport ("pulse")]
        private static extern int pa_cvolume_equal (Volume a, Volume b);
        [DllImport ("pulse")]
        private static extern void pa_cvolume_init ([Out] Volume a);
        [DllImport ("pulse")]
        private static extern void pa_cvolume_set ([In, Out] Volume vol, uint channels, UInt32 val);
        [DllImport ("pulse")]
        private static extern void pa_cvolume_snprint (System.Text.StringBuilder buffer, IntPtr bufferSize, Volume vol);
        static readonly int PA_CVOLUME_SNPRINT_BUFFER_MAX = 320;
        [DllImport ("pulse")]
        private static extern UInt32 pa_cvolume_avg (Volume vol);
    }
}
