//
//  Copyright © 2009 Christopher James Halse Rogers <raof@ubuntu.com>
//
//  PropList.cs is a part of Pulseaudio#
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
    public class PropList
    {
        private HandleRef handle;
        public PropList()
        {
            IntPtr listPtr = pa_proplist_new ();
            if (listPtr == IntPtr.Zero) {
                throw new NullReferenceException ();
            }
            handle = new HandleRef (this, listPtr);
        }

        internal PropList (IntPtr listPtr)
        {
            if (listPtr == IntPtr.Zero) {
                throw new NullReferenceException ();
            }
            handle = new HandleRef (this, listPtr);
        }

        public bool Empty {
            get {
                // TODO: Work out why pa_proplist_isempty doesn't return 0 when the list is empty.
                return (Count == 0);
            }
        }

        public uint Count {
            get {
                return pa_proplist_size (handle);
            }
        }

        public void Clear ()
        {
            pa_proplist_clear (handle);
        }

        [DllImport ("pulse")]
        private static extern int pa_proplist_isempty (HandleRef list);
        [DllImport ("pulse")]
        private static extern IntPtr pa_proplist_new ();
        [DllImport ("pulse")]
        private static extern void pa_proplist_clear (HandleRef list);
        [DllImport ("pulse")]
        private static extern uint pa_proplist_size (HandleRef list);
    }
}
