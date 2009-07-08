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
using GLib;

namespace Pulseaudio
{
    public class Context
    {
        IntPtr context;
        IntPtr pa_mainloop;

        public delegate void ConnectionStateHandler ();

        public event ConnectionStateHandler Ready;
        public event ConnectionStateHandler Connecting;

        public Context ()
        {
            pa_mainloop = pa_glib_mainloop_new (g_main_context_default ());
            context = pa_context_new (pa_glib_mainloop_get_api (pa_mainloop), "LibFoo");
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
            switch (pa_context_get_state (context)) {
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
            SettingName,   /**< The client is passing its application name to the daemon */
            Ready,          /**< The connection is established, the context is ready to execute operations */
            Failed,         /**< The connection failed or was disconnected */
            Terminated      /**< The connection was terminated cleanly */
        }   
        
        [DllImport ("glib-2.0")]
        private static extern IntPtr g_main_context_default ();

        [DllImport ("pulse")]
        private static extern IntPtr pa_context_new (IntPtr mainloop_api, string appName);
        [DllImport ("pulse")]
        private static extern int pa_context_connect (IntPtr context, string server, ContextConnectionFlags flags, 
                                                      IntPtr spawnApi);
        [DllImport ("pulse")]
        private static extern UInt32 pa_context_get_server_protocol_version (IntPtr context);
        [DllImport ("pulse")]
        private static extern void pa_context_set_state_callback (IntPtr context, 
                                                                  ContextNotifyCB cb, 
                                                                  IntPtr userdata);
        [DllImport ("pulse")]
        private static extern ConnectionState pa_context_get_state (IntPtr context);
        
        [DllImport ("pulse-mainloop-glib")]
        private static extern IntPtr pa_glib_mainloop_new (IntPtr main_context);
        [DllImport ("pulse-mainloop-glib")]
        private static extern IntPtr pa_glib_mainloop_get_api (IntPtr pa_mainloop);
        [DllImport ("pulse-mainloop-glib")]
        private static extern void pa_glib_mainloop_free (IntPtr pa_mainloop);
    }
}
