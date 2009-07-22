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

        private void ActWithMainLoop (Action action)
        {
            EventWaitHandle action_finished = new EventWaitHandle (false, EventResetMode.AutoReset);
            GLib.Timeout.Add (0, delegate {
                action ();
                action_finished.Set ();
                return false;
            });
            while (!action_finished.WaitOne (0, true)) {
                MainLoopIterate ();
            }
        }

        private void RunUntilEventSignal (Action action, EventWaitHandle until, string timeoutMessage)
        {
            var timeout = new EventWaitHandle (false, EventResetMode.AutoReset);
            GLib.Timeout.Add (1000, () => {timeout.Set (); return false;});
            action ();
            while (!until.WaitOne (0, true)) {
                MainLoopIterate ();
                if (timeout.WaitOne (0, true)) {
                    Assert.Fail (timeoutMessage);
                }
            }
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
                connection_finished.Set ();
            };
            RunUntilEventSignal (s.Connect, connection_finished, "Timeout waiting for Connect");
            Assert.AreEqual (16, s.ServerAPI);            
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
            RunUntilEventSignal (c.Connect, connection_finished, "Timeout waiting for Connect to finish");
            Assert.AreEqual (Context.ConnectionState.Ready, c.State);
        }

        [Test()]
        public void SinkInputCallbackIsCalled ()
        {
            var callback_called = new EventWaitHandle (false, EventResetMode.AutoReset);
            Context c = new Context ();
            c.Ready += delegate {
                c.EnumerateSinkInputs ((cb, eol) => {
                    callback_called.Set ();
                });
            };
            RunUntilEventSignal (c.Connect, callback_called, "Timeout waiting for EnumerateSinkInputs");
        }

        [Test()]
        public void SinkCallbackIsCalled ()
        {
            var callback_called = new EventWaitHandle (false, EventResetMode.AutoReset);

            Context c = new Context ();
            c.Ready += delegate {
                c.EnumerateSinks ((cb, eol) => {
                    callback_called.Set ();
                });
            };
            RunUntilEventSignal (c.Connect, callback_called, "Timeout waiting for EnumerateSinks");
        }

        [Test()]
        public void SinkInfoContainsNonEmptyName ()
        {
            var callback_called = new EventWaitHandle (false, EventResetMode.AutoReset);
            Context c = new Context ();
            c.Ready += delegate {
                c.EnumerateSinks ((SinkInfo info, int eol) => {
                    if (eol == 0) {
                        Assert.IsNotNull (info, "Sink info is null");
                        Assert.IsNotNull (info.Name, "info.Name is null");
                        Assert.IsNotEmpty (info.Name, "info.Name is empty");
                    } else {
                        callback_called.Set ();
                    }
                });
            };
            RunUntilEventSignal (c.Connect, callback_called, "Timeout waiting for EnumerateSinks");
        }
    }
}
