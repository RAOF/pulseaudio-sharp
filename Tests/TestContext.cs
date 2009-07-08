//  
//  Copyright © 2009 Christopher James Halse Rogers <raof@ubuntu.com>
// 
//  TestContext.cs is a part of Pulseaudio#
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
using NUnit.Framework;
using GLib;

namespace Pulseaudio
{
    [TestFixture()]
    public class TestContext
    {
        private void MainLoopIterate ()
        {
            while (GLib.MainContext.Iteration (false))
            { }
        }
        
        [Test()]
        // TODO: running this test is going to break frequently, because it's relying on the system-wide pulseaudio
        // server.  It'd be good to spawn a known-version server of our very own.
        public void TestGetServerVersion()
        {
            Context s = new Context ();
            MainLoopIterate ();
            Assert.AreEqual ("pulseaudio 0.9.16-test2", s.Version);
        }

        [Test()]
        public void TestGetServerAPIVersion ()
        {
            Context s = new Context ();
            MainLoopIterate ();
            Assert.AreEqual (16, s.ServerAPI);
        }
    }
}
