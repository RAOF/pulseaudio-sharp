//  
//  Copyright © 2009 Christopher James Halse Rogers <raof@ubuntu.com>
// 
//  Sink.cs is a part of Pulseaudio#
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
    public enum SinkState {
        Invalid = -1,
        /**< This state is used when the server does not support sink state introspection \since 0.9.15 */
        
        Running = 0,
        /**< Running, sink is playing and used by at least one non-corked sink-input \since 0.9.15 */

        Idle = 1,
        /**< When idle, the sink is playing but there is no non-corked sink-input attached to it \since 0.9.15 */
        
        Suspended = 2,
        /**< When suspended, actual sink access can be closed, for instance \since 0.9.15 */
    }

    [Flags]
    public enum SinkFlags {
        HardwareVolume = 0x0001,
        /**< Supports hardware volume control */
        
        LatencyQuery = 0x0002,
        /**< Supports latency querying */
        
        IsHardware = 0x0004,
        /**< Is a hardware sink of some kind, in contrast to
         * "virtual"/software sinks \since 0.9.3 */
        
        IsNetwork = 0x0008,
        /**< Is a networked sink of some kind. \since 0.9.7 */
        
        HardwareMute = 0x0010,
        /**< Supports hardware mute control \since 0.9.11 */
        
        dBVolume = 0x0020,
        /**< Volume can be translated to dB with pa_sw_volume_to_dB()
         * \since 0.9.11 */
        
        FlatVolume = 0x0040,
        /**< This sink is in flat volume mode, i.e. always the maximum of
         * the volume of all connected inputs. \since 0.9.15 */
        
        DynamicLatency = 0x0080
        /**< The latency can be adjusted dynamically depending on the
         * needs of the connected streams. \since 0.9.15 */
    }

    [StructLayout (LayoutKind.Sequential)]
    public class SinkInfo
    {
        IntPtr name;                  /**< Name of the sink */
        UInt32 index;                 /**< Index of the sink */
        IntPtr description;           /**< Description of this sink */
        SampleSpec sample_spec;       /**< Sample spec of this sink */
        ChannelMap channel_map;       /**< Channel map */
        UInt32 owner_module;          /**< Index of the owning module of this sink, or PA_INVALID_INDEX */
        public Volume volume;         /**< Volume of the sink */
        int mute;                     /**< Mute switch of the sink */
        UInt32 monitor_source;        /**< Index of the monitor source connected to this sink */
        IntPtr monitor_source_name;   /**< The name of the monitor source */
        UInt64 latency;               /**< Length of queued audio in the output buffer. */
        IntPtr driver;                /**< Driver name. */
        SinkFlags flags;              /**< Flags */
        IntPtr proplist;              /**< Property list \since 0.9.11 */
        UInt64 configured_latency;    /**< The latency this device has been configured to. \since 0.9.11 */
        UInt32 base_volume;           /**< Some kind of "base" volume that refers to unamplified/unattenuated volume in the context of the output device. \since 0.9.15 */
        SinkState state;              /**< State \since 0.9.15 */
        UInt32 n_volume_steps;        /**< Number of volume steps for sinks which do not support arbitrary volumes. \since 0.9.15 */
        UInt32 card;                  /**< Card index, or PA_INVALID_INDEX. \since 0.9.15 */
        
        public string Name {
            get {
                return Marshal.PtrToStringAnsi (name);
            }
        }

        public string Description {
            get {
                return Marshal.PtrToStringAnsi (description);
            }
        }

        public string MonitorSourceName {
            get {
                return Marshal.PtrToStringAnsi (monitor_source_name);
            }
        }
        
        public string Driver {
            get {
                return Marshal.PtrToStringAnsi (driver);
            }
        }
        
        public SinkInfo()
        {
        }
    }
}
