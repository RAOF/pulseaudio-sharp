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
using System.Runtime.InteropServices;

namespace Pulseaudio
{
    public class Constants
    {
        public const int MaxChannels = 32;
    }
    
    public class Volume
    {
        private struct CVolume
        {
            public Byte channels;
            [MarshalAs (UnmanagedType.ByValArray, SizeConst=Constants.MaxChannels)]
            public UInt32 [] values;
        }

        private CVolume vol;
        private const UInt32 norm = 0x10000U;
        private const UInt32 mute = 0;
        
        public Volume()
        {
            vol = pa_cvolume_init (vol);
            vol.channels = 2;
        }

        public void Reset ()
        {
            vol = pa_cvolume_set (vol, vol.channels, norm);
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
            return pa_cvolume_equal (a.vol, b.vol) != 0;
        }
        public static bool operator!= (Volume a, Volume b)
        {
            return !(a==b);
        }

        [DllImport ("pulse")]
        private static extern int pa_cvolume_equal (CVolume a, CVolume b);
        [DllImport ("pulse")]
        private static extern CVolume pa_cvolume_init (CVolume a);
        [DllImport ("pulse")]
        private static extern CVolume pa_cvolume_set (CVolume vol, uint channels, UInt32 val);
    }
}
