//  
//  Copyright © 2009 Christopher James Halse Rogers <raof@ubuntu.com>
// 
//  SinkInput.cs is a part of Pulseaudio#
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
    [StructLayout (LayoutKind.Sequential)]
    public class SinkInputInfo
    {
        public UInt32 index;                        /**< Index of the sink input */
        public string name;                         /**< Name of the sink input */
        public UInt32 owner_module;                 /**< Index of the module this sink input belongs to, or PA_INVALID_INDEX when it does not belong to any module */
        public UInt32 client;                       /**< Index of the client this sink input belongs to, or PA_INVALID_INDEX when it does not belong to any client */
        public UInt32 sink;                         /**< Index of the connected sink */
        public SampleSpec sample_spec;              /**< The sample specification of the sink input */
        public ChannelMap channel_map;              /**< Channel map */
        public Volume volume;                       /**< The volume of this sink input */
        public UInt64 buffer_usec;                  /**< Latency due to buffering in sink input, see pa_latency_info for details */
        public UInt64 sink_usec;                    /**< Latency of the sink device, see pa_latency_info for details */
        public string resample_method;              /**< The resampling method used by this sink input. */
        public string driver;                       /**< Driver name */
        public int mute;                            /**< Stream muted \since 0.9.7 */
        // The Pulseaudio API really, really wants this to be an opaque pointer.
        // Wrap it as an opaque pointer, and provide a property exposing a properly wrapped
        // PropList.
        private IntPtr prop_handle;                 /**< Property list \since 0.9.11 */

        public PropList proplist {
            get {
                return new PropList (prop_handle);
            }
        }
    }
}
