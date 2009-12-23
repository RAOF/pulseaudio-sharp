//
//  Copyright © 2009 Christopher James Halse Rogers <raof@ubuntu.com>
//
//  Context.cs is a part of Pulseaudio#
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
using System.Collections.Generic;
using System.Threading;

namespace Pulseaudio
{
    public class Context
    {
        HandleRef context;
        MainLoop loop;

        public delegate void ConnectionStateHandler ();

        public event ConnectionStateHandler Ready;
        public event ConnectionStateHandler Connecting;

        public Context ()
        {
            loop = new GLibMainLoop ();
            context = new HandleRef (this, pa_context_new (loop.GetAPI (), "LibFoo"));
            pa_context_set_state_callback (context, ContextNotifyHandler, new IntPtr (0));
        }

        public void Connect ()
        {
            pa_context_connect (context, null, ContextConnectionFlags.None, new IntPtr (0));
        }

        public UInt32 ServerAPI {
            get {
                return pa_context_get_server_protocol_version (context);
            }
        }

        public string Version {
            get {
                return "pulseaudio 0.9.16-test2";
            }
        }

        public ConnectionState State {
            get {
                return pa_context_get_state (context);
            }
        }

        [Flags]
        public enum ContextConnectionFlags
        {
            None = 0,
            NoAutoSpawn = 1,
            NoFail= 2
        }

        private delegate void ContextNotifyCB (IntPtr context, IntPtr userdata);

        private void ContextNotifyHandler (IntPtr context, IntPtr userdata)
        {
            HandleRef c = new HandleRef (this, context);
            switch (pa_context_get_state (c)) {
            case ConnectionState.Ready:
                if (Ready != null) {
                    Ready ();
                }
                break;
            case ConnectionState.Connecting:
                if (Connecting != null) {
                    Connecting ();
                }
                break;
            }
        }

        public enum ConnectionState {
            Unconnected,    /**< The context hasn't been connected yet */
            Connecting,     /**< A connection is being established */
            Authorising,    /**< The client is authorizing itself to the daemon */
            SettingName,    /**< The client is passing its application name to the daemon */
            Ready,          /**< The connection is established, the context is ready to execute operations */
            Failed,         /**< The connection failed or was disconnected */
            Terminated      /**< The connection was terminated cleanly */
        }


        public Error LastError {
            get {
                return new Error (pa_context_errno (context));
            }
        }

        [DllImport ("pulse")]
        private static extern Error.Code pa_context_errno (HandleRef context);

        public void EnumerateSinkInputs (SinkInputCallback cb)
        {
            var wrapped_cb = new pa_sink_input_info_cb ((IntPtr c, NativeSinkInputInfo info, int eol, IntPtr userdata) =>
            {
                if (eol == 0) {
                    cb (new SinkInput (info), eol);
                } else {
                    cb (null, eol);
                }
            });
            pa_context_get_sink_input_info_list (context, wrapped_cb, IntPtr.Zero);
        }

        public delegate void SinkInputCallback (SinkInput info, int eol);
        private delegate void pa_sink_input_info_cb (IntPtr context, NativeSinkInputInfo info, int eol, IntPtr userdata);

        internal Operation EnumerateSinks (SinkInfoCallback cb)
        {
            var wrapped_cb = new pa_sink_info_cb ((IntPtr c, SinkInfo info, int eol, IntPtr userdata) => {
                cb (info, eol);
            });
            return new Operation (pa_context_get_sink_info_list (context, wrapped_cb, IntPtr.Zero));
        }

        internal delegate void SinkInfoCallback (SinkInfo info, int eol);
        private delegate void pa_sink_info_cb (IntPtr context, SinkInfo info, int eol, IntPtr userdata);


        public Operation EnumerateSinks (SinkCallback cb)
        {
            return EnumerateSinks ((SinkInfo info, int eol) => {
                if (eol == 0) {
                    cb (new Sink (this, info));
                }
            });
        }

        public delegate void SinkCallback (Sink s);

        internal Operation GetSinkInfoByIndex (UInt32 index, SinkInfoCallback cb)
        {
            var wrapped_cb = new pa_sink_info_cb ((IntPtr c, SinkInfo info, int eol, IntPtr userdata) => {
                cb (info, eol);
            });
            return new Operation (pa_context_get_sink_info_by_index (context, index, wrapped_cb, IntPtr.Zero));
        }

        [DllImport ("pulse")]
        private static extern IntPtr pa_context_get_sink_info_by_index (HandleRef context, UInt32 index, pa_sink_info_cb cb, IntPtr userdata);


        public Operation SetSinkVolume (UInt32 index, Volume vol, OperationSuccessCallback cb)
        {
            var wrapped_cb = new pa_context_success_cb ((IntPtr context, int success, IntPtr userdata) => {
                cb (success);
            });
            try {
                return new Operation (pa_context_set_sink_volume_by_index (context,
                                                                           index,
                                                                           vol,
                                                                           wrapped_cb,
                                                                           IntPtr.Zero));
            } catch (ArgumentNullException e) {
                throw new Exception (String.Format ("Error setting sink volume: {0}", LastError.Message));
            }
        }


        [DllImport("pulse")]
        private static extern IntPtr pa_context_set_sink_volume_by_index(HandleRef context, UInt32 idx, Volume vol, pa_context_success_cb cb, IntPtr userdata);

        public delegate void OperationSuccessCallback (int success);
        private delegate void pa_context_success_cb (IntPtr context, int success, IntPtr userdata);

        [DllImport ("pulse")]
        private static extern IntPtr pa_context_new (IntPtr mainloop_api, string appName);
        [DllImport ("pulse")]
        private static extern int pa_context_connect (HandleRef context, string server, ContextConnectionFlags flags,
                                                      IntPtr spawnApi);
        [DllImport ("pulse")]
        private static extern UInt32 pa_context_get_server_protocol_version (HandleRef context);
        [DllImport ("pulse")]
        private static extern void pa_context_set_state_callback (HandleRef context,
                                                                  ContextNotifyCB cb,
                                                                  IntPtr userdata);
        [DllImport ("pulse")]
        private static extern ConnectionState pa_context_get_state (HandleRef context);

        [DllImport ("pulse")]
        private static extern IntPtr pa_context_get_sink_input_info_list (HandleRef context,
                                                                          pa_sink_input_info_cb cb,
                                                                          IntPtr userdata);
        [DllImport ("pulse")]
        private static extern IntPtr pa_context_get_sink_info_list (HandleRef context,
                                                                    pa_sink_info_cb cb,
                                                                    IntPtr userdata);

        public delegate void ServerInfoCallback (ServerInfo info);
        private delegate void pa_server_info_cb (IntPtr context, NativeServerInfo info, IntPtr userdata);
        [DllImport ("pulse")]
        private static extern IntPtr pa_context_get_server_info (HandleRef context,
                                                                 pa_server_info_cb cb,
                                                                 IntPtr userdata);
        public Operation GetServerInfo (ServerInfoCallback cb)
        {
            pa_server_info_cb wrappedCallback = (_, info, __) => { cb (new ServerInfo (info)); };
            Operation opn;
            try {
                opn = new Operation (pa_context_get_server_info (context, wrappedCallback, IntPtr.Zero));
            } catch (ArgumentNullException) {
                throw new Exception (String.Format ("Error getting server info: {0}", LastError.Message));
            }
            return opn;
        }


        private EventHandler<RawSinkEventArgs> _rawSinkEventHandler;
        private readonly object eventHandlerLock = new object ();
        internal event EventHandler<RawSinkEventArgs> RawSinkEvent {
            add {
                lock (eventHandlerLock) {
                    if (_rawSinkEventHandler == null) {
                        pa_context_set_subscribe_callback (context, SubscriptionEventHandler, IntPtr.Zero);
                        Operation o = new Operation (pa_context_subscribe (context,
                                                                           SubscriptionMask.PA_SUBSCRIPTION_MASK_ALL,
                                                                           (a, b, c) => {;},
                                                                           IntPtr.Zero));
                        o.Dispose ();
                    }
                    _rawSinkEventHandler += value;
                }
            }
            remove {
                lock (eventHandlerLock) {
                    _rawSinkEventHandler -= value;
                    if (_rawSinkEventHandler == null) {
                        Operation o = new Operation (pa_context_subscribe (context,
                                                                           SubscriptionMask.PA_SUBSCRIPTION_MASK_NULL,
                                                                           (a,b,c) => {;},
                                                                           IntPtr.Zero));
                        o.Dispose ();
                    }
                }
            }
        }

        private void SubscriptionEventHandler (IntPtr context, SubscriptionEventMask e, UInt32 index, IntPtr userdata)
        {
            EventType action = EventType.Error;
            switch (e & SubscriptionEventMask.PA_SUBSCRIPTION_EVENT_TYPE_MASK) {
            case SubscriptionEventMask.PA_SUBSCRIPTION_EVENT_CHANGE:
                action = EventType.Changed;
                break;
            case SubscriptionEventMask.PA_SUBSCRIPTION_EVENT_NEW:
                action = EventType.New;
                break;
            case SubscriptionEventMask.PA_SUBSCRIPTION_EVENT_REMOVE:
                action = EventType.Removed;
                break;
            }
            if ((e & SubscriptionEventMask.PA_SUBSCRIPTION_EVENT_SINK) == 0x0000) {
                EventHandler<RawSinkEventArgs> handler;
                lock (eventHandlerLock) {
                    handler = _rawSinkEventHandler;
                }
                if (handler != null) {
                    handler (this, new RawSinkEventArgs { action = action, index = index });
                }
            }
        }

        private delegate void SubscriptionEventCB (IntPtr context, SubscriptionEventMask e, UInt32 index, IntPtr userdata);
        [DllImport ("pulse")]
        private static extern IntPtr pa_context_subscribe(HandleRef context,
                                                          SubscriptionMask m,
                                                          pa_context_success_cb cb,
                                                          IntPtr userdata);
        [DllImport ("pulse")]
        private static extern void pa_context_set_subscribe_callback (HandleRef context,
                                                                      SubscriptionEventCB cb,
                                                                      IntPtr userData);

        [Flags]
        private enum SubscriptionMask : uint {
            PA_SUBSCRIPTION_MASK_NULL = 0x0000U,/**< No events */
            PA_SUBSCRIPTION_MASK_SINK = 0x0001U,/**< Sink events */
            PA_SUBSCRIPTION_MASK_SOURCE = 0x0002U,/**< Source events */
            PA_SUBSCRIPTION_MASK_SINK_INPUT = 0x0004U,/**< Sink input events */
            PA_SUBSCRIPTION_MASK_SOURCE_OUTPUT = 0x0008U,/**< Source output events */
            PA_SUBSCRIPTION_MASK_MODULE = 0x0010U,/**< Module events */
            PA_SUBSCRIPTION_MASK_CLIENT = 0x0020U,/**< Client events */
            PA_SUBSCRIPTION_MASK_SAMPLE_CACHE = 0x0040U,/**< Sample cache events */
            PA_SUBSCRIPTION_MASK_SERVER = 0x0080U,/**< Other global server changes. */
            PA_SUBSCRIPTION_MASK_AUTOLOAD = 0x0100U,/**< \deprecated Autoload table events. */
            PA_SUBSCRIPTION_MASK_CARD = 0x0200U,/**< Card events. \since 0.9.15 */
            PA_SUBSCRIPTION_MASK_ALL = 0x02ffU/**< Catch all events */
        }
        [Flags]
        private enum SubscriptionEventMask : uint {
            PA_SUBSCRIPTION_EVENT_SINK = 0x0000U,/**< Event type: Sink */
            PA_SUBSCRIPTION_EVENT_SOURCE = 0x0001U,/**< Event type: Source */
            PA_SUBSCRIPTION_EVENT_SINK_INPUT = 0x0002U,/**< Event type: Sink input */
            PA_SUBSCRIPTION_EVENT_SOURCE_OUTPUT = 0x0003U,/**< Event type: Source output */
            PA_SUBSCRIPTION_EVENT_MODULE = 0x0004U,/**< Event type: Module */
            PA_SUBSCRIPTION_EVENT_CLIENT = 0x0005U,/**< Event type: Client */
            PA_SUBSCRIPTION_EVENT_SAMPLE_CACHE = 0x0006U,/**< Event type: Sample cache item */
            PA_SUBSCRIPTION_EVENT_SERVER = 0x0007U,/**< Event type: Global server change, only occurring with PA_SUBSCRIPTION_EVENT_CHANGE. */
            PA_SUBSCRIPTION_EVENT_AUTOLOAD = 0x0008U,/**< \deprecated Event type: Autoload table changes. */
            PA_SUBSCRIPTION_EVENT_CARD = 0x0009U,/**< Event type: Card \since 0.9.15 */
            PA_SUBSCRIPTION_EVENT_FACILITY_MASK = 0x000FU,/**< A mask to extract the event type from an event value */
            PA_SUBSCRIPTION_EVENT_NEW = 0x0000U,/**< A new object was created */
            PA_SUBSCRIPTION_EVENT_CHANGE = 0x0010U,/**< A property of the object was modified */
            PA_SUBSCRIPTION_EVENT_REMOVE = 0x0020U,/**< An object was removed */
            PA_SUBSCRIPTION_EVENT_TYPE_MASK = 0x0030U/**< A mask to extract the event operation from an event value */
        }
        public enum EventType {
            New,
            Changed,
            Removed,
            Error
        }
        public class RawSinkEventArgs : EventArgs {
            public EventType action { get; set;}
            public UInt32 index {get; set; }
        }
    }
}
