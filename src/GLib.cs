//
//  Copyright © 2009 Christopher James Halse Rogers <raof@ubuntu.com>
//
//  GLib.cs is a part of Pulseaudio#
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
using System.Threading;
using Pulseaudio;
using GLib;

namespace Pulseaudio.GLib
{
    public static class GLibExtensions
    {
        public static void Wait (this Operation opn)
        {
            while (opn.State == Operation.Status.Running) {
                MainContext.Iteration ();
            }
        }

        public static void ConnectAndWait (this Context context)
        {
            ManualResetEvent ready = new ManualResetEvent (false);
            context.Ready += delegate {
                ready.Set ();
            };
            context.Connect ();
            while (!ready.WaitOne (0, true)) {
                MainContext.Iteration ();
            }
        }
    }
}
