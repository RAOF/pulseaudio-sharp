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
using System.Threading;
using NUnit.Framework;
using GLib;

namespace Pulseaudio
{
    // TODO: These tests all run against the user's pulseaudio daemon, which we don't control.
    // Thus, some of these tests are expected to break on systems which aren't mine!
    // It would be good to set up an explicit test pulseaudio server which doesn't interact with the user.
    // This shall be left as an exercise for the reader.
    [TestFixture()]
    public class TestContext
    {
        private void MainLoopIterate ()
        {
            while (GLib.MainContext.Iteration (false))
            { }
        }
        
        [Test()]
        public void TestGetServerVersion()
        {
            Context s = new Context ();
            MainLoopIterate ();
            Assert.AreEqual ("pulseaudio 0.9.16-test2", s.Version);
        }

        [Test()]
        public void TestGetServerAPIVersion ()
        {
            EventWaitHandle connection_finished = new EventWaitHandle (false, EventResetMode.AutoReset);
            Context s = new Context ();
            s.Ready += delegate {
                Assert.AreEqual (16, s.ServerAPI);
                connection_finished.Set ();
            };
            s.Connect ();
            while (!connection_finished.WaitOne (0, true)) {
                MainLoopIterate ();
            }
        }

        [Test()]
        public void TestConnectedEventTriggers ()
        {
            bool flag = false;
            Context c = new Context ();
            c.Ready += delegate { flag = true; };
            c.Connect ();
            while (!flag) {
                MainLoopIterate ();
            }
            Assert.IsTrue (flag);
        }

        [Test()]
        public void TestConnectingEventTriggers ()
        {
            bool flag = false;
            Context c = new Context ();
            c.Connecting += delegate { flag = true; };
            c.Connect ();
            MainLoopIterate ();
            Assert.IsTrue (flag);            
        }

        [Test()]
        public void TestStatusIsDisconnectedBeforeConnect ()
        {
            Context c = new Context ();
            Assert.AreEqual (Context.ConnectionState.Unconnected, c.State);
        }

        [Test()]
        public void TestStatusIsReadyAfterConnect ()
        {
            EventWaitHandle connection_finished = new EventWaitHandle (false, EventResetMode.AutoReset);
            Context c = new Context ();
            c.Ready += delegate {
                connection_finished.Set ();
            };
            c.Connect ();
            while (!connection_finished.WaitOne (0, true)) {
                MainLoopIterate ();
            }
            Assert.AreEqual (Context.ConnectionState.Ready, c.State);
        }

        [Test()]
        public void SinkInputCallbackIsCalled ()
        {
        }
    }
}
