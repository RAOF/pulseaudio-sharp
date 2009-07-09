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
    public interface MainLoop
    {
        //TODO: Make this into an actual managed implementation of a Pulseaudio main loop
        //For the moment, just require that we can get a pa_mainloop_api pointer out.
        IntPtr GetAPI ();
    }

    public class GLibMainLoop : MainLoop, IDisposable
    {
        IntPtr pa_mainloop = new IntPtr (0);

        public GLibMainLoop ()
        {
            pa_mainloop = pa_glib_mainloop_new (g_main_context_default ());
        }
        
        public IntPtr GetAPI ()
        {
            if (pa_mainloop == new IntPtr (0)) {
                throw new Exception ("Foo");
            }
            return pa_glib_mainloop_get_api (pa_mainloop);
        }

        public void Dispose ()
        {
            pa_glib_mainloop_free (pa_mainloop);
            pa_mainloop = new IntPtr (0);
        }
        
        [DllImport ("pulse-mainloop-glib")]
        private static extern IntPtr pa_glib_mainloop_new (IntPtr main_context);
        [DllImport ("pulse-mainloop-glib")]
        private static extern IntPtr pa_glib_mainloop_get_api (IntPtr pa_mainloop);
        [DllImport ("pulse-mainloop-glib")]
        private static extern void pa_glib_mainloop_free (IntPtr pa_mainloop);        

        [DllImport ("glib-2.0")]
        private static extern IntPtr g_main_context_default ();
    }
}
