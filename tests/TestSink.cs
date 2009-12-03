//
//  Copyright © 2009 Christopher James Halse Rogers <raof@ubuntu.com>
//
//  TestSink.cs is a part of Pulseaudio#
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
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

using GLib;
using Pulseaudio.GLib;

namespace Pulseaudio
{


    [TestFixture()]
    public class TestSink
    {

        [Test()]
        public void TestGetName ()
        {
            Context c = new Context ();
            c.ConnectAndWait ();
            var sinks = new List<Sink> ();
            using (Operation o = c.EnumerateSinks ((Sink s) => sinks.Add (s))) {
                o.Wait ();
            }
            Assert.Contains ("Internal Audio Analog Stereo", (from sink in sinks select sink.Description).ToList ());
        }

    }
}
