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
    internal class NativeSinkInputInfo
    {
        public UInt32 index;                        /**< Index of the sink input */
        public IntPtr name;                         /**< Name of the sink input */
        public UInt32 owner_module;                 /**< Index of the module this sink input belongs to, or PA_INVALID_INDEX when it does not belong to any module */
        public UInt32 client;                       /**< Index of the client this sink input belongs to, or PA_INVALID_INDEX when it does not belong to any client */
        public UInt32 sink;                         /**< Index of the connected sink */
        public SampleSpec sample_spec;              /**< The sample specification of the sink input */
        public ChannelMap channel_map;              /**< Channel map */
        public Volume volume;                       /**< The volume of this sink input */
        public UInt64 buffer_usec;                  /**< Latency due to buffering in sink input, see pa_latency_info for details */
        public UInt64 sink_usec;                    /**< Latency of the sink device, see pa_latency_info for details */
        public IntPtr resample_method;              /**< The resampling method used by this sink input. */
        public IntPtr driver;                       /**< Driver name */
        public int mute;                            /**< Stream muted \since 0.9.7 */
        public IntPtr prop_handle;                  /**< Property list \since 0.9.11 */
    }

    public class SinkInput
    {
        private Context ctx;
        private NativeSinkInputInfo info;

        internal SinkInput (NativeSinkInputInfo info, Context c)
        {
            ctx = c;
            this.info = info;
            Name = Marshal.PtrToStringAnsi (info.name);
            ResampleMethod = Marshal.PtrToStringAnsi (info.resample_method);
            Driver = Marshal.PtrToStringAnsi (info.driver);
        }

        public string Name {
            get;
            private set;
        }

        public string ResampleMethod {
            get;
            private set;
        }

        public string Driver {
            get;
            private set;
        }

        public UInt32 Index {
            get { return info.index; }
        }
        public UInt32 OwnerModule {
            get { return info.owner_module; }
        }
        public UInt32 Client {
            get { return info.client; }
        }
        public UInt32 Sink {
            get { return info.sink; }
        }
        public SampleSpec Spec {
            get { return info.sample_spec; }
        }
        public ChannelMap Map {
            get { return info.channel_map; }
        }
        public Volume Vol {
            get { return info.volume; }
        }
        public UInt64 BufferLatency {
            get { return info.buffer_usec; }
        }
        public UInt64 SinkLatency {
            get { return info.sink_usec; }
        }
        public int Mute {
            get { return info.mute; }
        }
        public PropList Properties {
            get;
            private set;
        }

        public Operation SetVolume (Volume v, Context.OperationSuccessCallback cb)
        {
            return ctx.SetSinkInputVolume (Index, v, cb);
        }
    }
}
