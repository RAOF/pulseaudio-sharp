//
//  Copyright © 2009 Christopher James Halse Rogers <raof@ubuntu.com>
//
//  MainLoop.cs is a part of Pulseaudio#
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
    public interface MainLoop : IDisposable
    {
        //TODO: Make this into an actual managed implementation of a Pulseaudio main loop
        //For the moment, just require that we can get a pa_mainloop_api pointer out.
        IntPtr GetAPI ();
    }

    public class GLibMainLoop : MainLoop
    {
        IntPtr pa_mainloop = IntPtr.Zero;
        bool disposed = false;

        public GLibMainLoop ()
        {
            pa_mainloop = pa_glib_mainloop_new (IntPtr.Zero);
        }

        public IntPtr GetAPI ()
        {
            if (disposed) {
                throw new ObjectDisposedException ("GLibMainLoop", "GLibMainLoop used after being disposed!");
            }
            return pa_glib_mainloop_get_api (pa_mainloop);
        }

        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        protected virtual void Dispose (bool explicitlyCalled)
        {
            if (!disposed) {
                pa_glib_mainloop_free (pa_mainloop);
                disposed = true;
            }
        }

        ~GLibMainLoop ()
        {
            Dispose (false);
        }

        [DllImport ("pulse-mainloop-glib")]
        private static extern IntPtr pa_glib_mainloop_new (IntPtr main_context);
        [DllImport ("pulse-mainloop-glib")]
        private static extern IntPtr pa_glib_mainloop_get_api (IntPtr pa_mainloop);
        [DllImport ("pulse-mainloop-glib")]
        private static extern void pa_glib_mainloop_free (IntPtr pa_mainloop);
    }
}
