//  
//  Copyright © 2009 Christopher James Halse Rogers <raof@ubuntu.com>
// 
//  Channel.cs is a part of Pulseaudio#
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
    public enum ChannelPosition
    {
        Invalid = -1,
        Mono = 0,
        
        FrontLeft,               /* Apple calls this 'Left' */
        FrontRight,              /* Apple calls this 'Right' */
        FrontCenter,             /* Apple calls this 'Center' */
        
        Left = FrontLeft,
        Right = FrontRight,
        Center = FrontCenter,
        
        RearCentre,              /* Microsoft calls this 'Back Center', Apple calls this 'Center Surround' */
        RearLeft,                /* Microsoft calls this 'Back Left', Apple calls this 'Left Surround' */
        RearRight,               /* Microsoft calls this 'Back Right', Apple calls this 'Right Surround' */
        
        LFE,                     /* Microsoft calls this 'Low Frequency', Apple calls this 'LFEScreen' */
        SubWoofer = LFE,
        
        FrontLeftOfCenter,       /* Apple calls this 'Left Center' */
        FrontRightOfCenter,      /* Apple calls this 'Right Center */
        
        SideLeft,                /* Apple calls this 'Left Surround Direct' */
        SideRight,               /* Apple calls this 'Right Surround Direct' */
        
        PA_CHANNEL_POSITION_AUX0,
        PA_CHANNEL_POSITION_AUX1,
        PA_CHANNEL_POSITION_AUX2,
        PA_CHANNEL_POSITION_AUX3,
        PA_CHANNEL_POSITION_AUX4,
        PA_CHANNEL_POSITION_AUX5,
        PA_CHANNEL_POSITION_AUX6,
        PA_CHANNEL_POSITION_AUX7,
        PA_CHANNEL_POSITION_AUX8,
        PA_CHANNEL_POSITION_AUX9,
        PA_CHANNEL_POSITION_AUX10,
        PA_CHANNEL_POSITION_AUX11,
        PA_CHANNEL_POSITION_AUX12,
        PA_CHANNEL_POSITION_AUX13,
        PA_CHANNEL_POSITION_AUX14,
        PA_CHANNEL_POSITION_AUX15,
        PA_CHANNEL_POSITION_AUX16,
        PA_CHANNEL_POSITION_AUX17,
        PA_CHANNEL_POSITION_AUX18,
        PA_CHANNEL_POSITION_AUX19,
        PA_CHANNEL_POSITION_AUX20,
        PA_CHANNEL_POSITION_AUX21,
        PA_CHANNEL_POSITION_AUX22,
        PA_CHANNEL_POSITION_AUX23,
        PA_CHANNEL_POSITION_AUX24,
        PA_CHANNEL_POSITION_AUX25,
        PA_CHANNEL_POSITION_AUX26,
        PA_CHANNEL_POSITION_AUX27,
        PA_CHANNEL_POSITION_AUX28,
        PA_CHANNEL_POSITION_AUX29,
        PA_CHANNEL_POSITION_AUX30,
        PA_CHANNEL_POSITION_AUX31,
        
        TopCenter,              /* Apple calls this 'Top Center Surround' */
        
        TopFrontLeft,           /* Apple calls this 'Vertical Height Left' */
        TopFrontRight,          /* Apple calls this 'Vertical Height Right' */
        TopFrontCenter,         /* Apple calls this 'Vertical Height Center' */
        
        TopRearLeft,            /* Microsoft and Apple call this 'Top Back Left' */
        TopRearRight,           /* Microsoft and Apple call this 'Top Back Right' */
        TopRearCenter,          /* Microsoft and Apple call this 'Top Back Center' */
        
        PA_CHANNEL_POSITION_MAX
    }

    [StructLayout (LayoutKind.Sequential)]
    public struct ChannelMap        
    {
        public byte channels;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst=Constants.MaxChannels)]
        public ChannelPosition [] map;

        private void Init ()
        {
            map = new ChannelPosition[Constants.MaxChannels];
            pa_channel_map_init (ref this);
        }
        
        public static ChannelMap StereoMapping ()
        {
            ChannelMap mapping = new ChannelMap ();
            mapping.Init ();
            pa_channel_map_init_stereo (ref mapping);
            return mapping;
        }

        public static ChannelMap MonoMapping ()
        {
            ChannelMap mapping = new ChannelMap ();
            mapping.Init ();
            pa_channel_map_init_mono (ref mapping);
            return mapping;
        }
        
        [DllImport ("pulse")]
        private static extern void pa_channel_map_init (ref ChannelMap map);
        [DllImport ("pulse")]
        private static extern void pa_channel_map_init_stereo (ref ChannelMap map);
        [DllImport ("pulse")]
        private static extern void pa_channel_map_init_mono (ref ChannelMap map);
    }
}
