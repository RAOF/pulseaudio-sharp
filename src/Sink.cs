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
using System.Collections.Generic;
using System.Linq;
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
        public UInt32 index;                 /**< Index of the sink */
        IntPtr description;           /**< Description of this sink */
        public SampleSpec sample_spec;       /**< Sample spec of this sink */
        public ChannelMap channel_map;       /**< Channel map */
        public UInt32 owner_module;          /**< Index of the owning module of this sink, or PA_INVALID_INDEX */
        public Volume volume;         /**< Volume of the sink */
        public int mute;                     /**< Mute switch of the sink */
        public UInt32 monitor_source;        /**< Index of the monitor source connected to this sink */
        IntPtr monitor_source_name;   /**< The name of the monitor source */
        public UInt64 latency;               /**< Length of queued audio in the output buffer. */
        IntPtr driver;                /**< Driver name. */
        public SinkFlags flags;              /**< Flags */
        IntPtr proplist;              /**< Property list \since 0.9.11 */
        public UInt64 configured_latency;    /**< The latency this device has been configured to. \since 0.9.11 */
        public UInt32 base_volume;           /**< Some kind of "base" volume that refers to unamplified/unattenuated volume in the context of the output device. \since 0.9.15 */
        public SinkState state;              /**< State \since 0.9.15 */
        public UInt32 n_volume_steps;        /**< Number of volume steps for sinks which do not support arbitrary volumes. \since 0.9.15 */
        public UInt32 card;                  /**< Card index, or PA_INVALID_INDEX. \since 0.9.15 */

        public string Name {
            get {
                return Marshal.PtrToStringAnsi (name);
            }
        }

        public string Description {
            get {                return Marshal.PtrToStringAnsi (description);
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

        public PropList Properties {
            get {
                return new PropList (proplist);
            }
        }

        public SinkInfo()
        {
        }
    }

    public class Sink : IDisposable {
        private void UpdateFromInfo (SinkInfo i)
        {
            if (info.volume != i.volume) {
                EventHandler<VolumeChangedEventArgs> handler;
                lock (eventHandlerLock) {
                    handler = _volumeChangedHandler;
                }
                if (handler != null) {
                    handler (this, new VolumeChangedEventArgs (i.volume));
                }
            }
            info = i;
            Description = i.Description;
            MonitorSourceName = i.MonitorSourceName;
            Driver = i.Driver;
        }

        private bool disposed = false;
        private SinkInfo info;
        private Context context;

        public Sink (Context c, SinkInfo info)
        {
            context = c;
            this.info = info;
            Name = info.Name;
            Description = info.Description;
            MonitorSourceName = info.MonitorSourceName;
            Driver = info.Driver;
            Properties = info.Properties.Copy ();
            context.SinkEvent += HandleRawSinkEvent;
        }

        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        protected virtual void Dispose (bool explicitlyCalled)
        {
            if (!disposed) {
                if (explicitlyCalled) {
                    //Unregister our server info callbacks
                    context.SinkEvent -= HandleRawSinkEvent;
                }
                disposed = true;
            }
        }

        public string Name {
            get; private set;
        }

        public string Description {
            get; private set;
        }

        public PropList Properties {
            get; private set;
        }

        public UInt32 Index {
            get {
                return info.index;
            }
        }

        public SampleSpec SampleSpec {
            get {
                //SampleSpec is a struct, so this returns a copy.
                return info.sample_spec;
            }
        }

        public ChannelMap ChannelMap {
            get {
                //ChannelMap is a struct, so this returns a copy.
                return info.channel_map;
            }
        }
        public UInt32 OwnerModule {
            get {
                return info.owner_module;
            }
        }

        public Volume Volume {
            get {
                return info.volume.Copy ();
            }
        }

        public int Mute {
            get {
                return info.mute;
            }
        }

        public UInt32 MonitorSource {
            get {
                return info.monitor_source;
            }
        }

        public string MonitorSourceName {
            get; private set;
        }

        public UInt64 Latency {
            get {
                return info.latency;
            }
        }

        public string Driver {
            get; private set;
        }

        public SinkFlags Flags {
            get {
                return info.flags;
            }
        }

        public UInt64 ConfiguredLatency {
            get {
                return info.configured_latency;
            }
        }

        public UInt32 BaseVolume {
            get {
                return info.base_volume;
            }
        }

        public SinkState State {
            get {
                return info.state;
            }
        }

        public UInt32 NumVolumeSteps {
            get {
                return info.n_volume_steps;
            }
        }

        public UInt32 Card {
            get {
                return info.card;
            }
        }

        public delegate void VolumeCallback (Volume vol);
        public Operation GetVolume (VolumeCallback cb)
        {
            return context.GetSinkInfoByIndex (info.index, (SinkInfo i, int eol) =>
            {
                if (eol == 0) {
                    cb (i.volume);
                }} );
        }

        public Operation SetVolume (Volume vol, Context.OperationSuccessCallback cb)
        {
            return context.SetSinkVolume (info.index, vol, cb);
        }


        private readonly object eventHandlerLock = new object ();
        private EventHandler<VolumeChangedEventArgs> _volumeChangedHandler;
        public event EventHandler<VolumeChangedEventArgs> VolumeChanged {
            add {
                lock (eventHandlerLock) {
                    _volumeChangedHandler += value;
                }
            }
            remove {
                lock (eventHandlerLock) {
                    _volumeChangedHandler -= value;
                }
            }
        }

        void HandleRawSinkEvent (object sender, ServerEventArgs e)
        {
            Context c = sender as Context;
            if (e.Type == EventType.Changed) {
                if (e.index == info.index) {
                    Operation o = c.GetSinkInfoByIndex (info.index, (SinkInfo i, int eol) =>
                    {
                        if (eol == 0) {
                            UpdateFromInfo (i);
                        }});
                    o.Dispose ();
                }
            }
        }

        public class VolumeChangedEventArgs : EventArgs
        {
            public VolumeChangedEventArgs (Volume vol)
            {
                newVolume = vol;
            }

            public Volume newVolume { get; private set; }
        }
    }
}
