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

    public class SinkInput : IDisposable
    {
        private bool disposed = false;
        private Context ctx;
        private NativeSinkInputInfo info;

        internal SinkInput (NativeSinkInputInfo info, Context c)
        {
            ctx = c;
            this.info = info;
            Name = Marshal.PtrToStringAnsi (info.name);
            ResampleMethod = Marshal.PtrToStringAnsi (info.resample_method);
            Driver = Marshal.PtrToStringAnsi (info.driver);
            PropList temp = new PropList (info.prop_handle);
            Properties = temp.Copy ();
            ctx.SinkInputEvent += HandleSinkInputEvent;
        }

        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        public void Dispose (bool explicitlyCalled)
        {
            if (!disposed) {
                if (explicitlyCalled) {
                    Properties.Dispose ();
                    ctx.SinkInputEvent -= HandleSinkInputEvent;
                }
                disposed = true;
            }
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
        public Volume Volume {
            get { return info.volume.Copy (); }
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

        public class VolumeChangedEventArgs : EventArgs
        {
            public VolumeChangedEventArgs (Volume vol)
            {
                newVolume = vol;
            }

            public Volume newVolume { get; private set; }
        }

        public event EventHandler<VolumeChangedEventArgs> VolumeChanged;

        private void HandleSinkInputEvent (object sender, ServerEventArgs args)
        {
            Context c = sender as Context;
            if (args.Type == EventType.Changed && args.index == Index && c != null) {
                Operation o = ctx.GetSinkInputInfoByIndex (Index, (NativeSinkInputInfo i, int eol) => {
                    if (eol == 0) {
                        UpdateInfo (i);
                    }
                });
                o.Dispose ();
            }
        }

        private void UpdateInfo (NativeSinkInputInfo info)
        {
            if (this.info.volume != info.volume) {
                EventHandler<VolumeChangedEventArgs> handler;
                handler = VolumeChanged;
                if (handler != null) {
                    handler (this, new VolumeChangedEventArgs (info.volume));
                }
            }
            this.info = info;
            ResampleMethod = Marshal.PtrToStringAnsi (info.resample_method);
            PropList temp = new PropList (info.prop_handle);
            Properties = temp.Copy ();
        }
    }
}
