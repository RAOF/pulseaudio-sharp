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
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using NUnit.Framework;
using g = GLib;
using Pulseaudio.GLib;

namespace Pulseaudio
{
    // TODO: These tests all run against the user's pulseaudio daemon, which we don't control.
    // Thus, some of these tests are expected to break on systems which aren't mine!
    // It would be good to set up an explicit test pulseaudio server which doesn't interact with the user.
    // This shall be left as an exercise for the reader.
    [TestFixture()]
    public class TestContext
    {
        private Helper helper;

        [SetUp]
        public void SetUp ()
        {
            helper = new Helper ();
            helper.AddSink ("Default Test Sink 1");
            helper.AddSink ("Default Test Sink 2");
        }

        [TearDown]
        public void TearDown ()
        {
            helper.Dispose ();
        }

        private void MainLoopIterate ()
        {
            while (g::MainContext.Iteration (false))
            { }
        }

        private void ActWithMainLoop (Action action)
        {
            EventWaitHandle action_finished = new EventWaitHandle (false, EventResetMode.AutoReset);
            g::Timeout.Add (0, delegate {
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
            g::Timeout.Add (1000, () => {timeout.Set (); return false;});
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

        /*
         * This test asserts the running pulseaudio server's API version.
         * This will fail if the running server isn't 0.9.16.
         */
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
            c.ConnectAndWait ();
            c.EnumerateSinks ((cb, eol) => {
                    callback_called.Set ();
            });
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

        [Test()]
        public void SinkInfoContainsNonEmptyDescription ()
        {
            var callback_called = new EventWaitHandle (false, EventResetMode.AutoReset);
            Context c = new Context ();
            c.Ready += delegate {
                c.EnumerateSinks ((SinkInfo info, int eol) => {
                    if (eol == 0) {
                        Assert.IsNotNull (info, "Sink info is null");
                        Assert.IsNotNull (info.Description, "info.Name is null");
                        Assert.IsNotEmpty (info.Description, "info.Name is empty");
                    } else {
                        callback_called.Set ();
                    }
                });
            };
            RunUntilEventSignal (c.Connect, callback_called, "Timeout waiting for EnumerateSinks");
        }

        [Test()]
        public void SinkInfoContainsKnownSink ()
        {
            helper.AddSink ("TestName");
            var callback_called = new EventWaitHandle (false, EventResetMode.AutoReset);
            Context c = new Context ();
            List<string> sink_names = new List<string> ();
            c.Ready += delegate {
                c.EnumerateSinks ((SinkInfo info, int eol) => {
                    if (eol == 0) {
                        sink_names.Add (info.Name);
                    } else {
                        Assert.Contains ("TestName", sink_names);
                        callback_called.Set ();
                    }
                });
            };
            RunUntilEventSignal (c.Connect, callback_called, "Timeout waiting for EnumerateSinks");
        }

        [Test()]
        public void SinkInfoContainsKnownDescription ()
        {
            helper.AddSink ("Test Sink", "Test Description");
            var callback_called = new EventWaitHandle (false, EventResetMode.AutoReset);
            Context c = new Context ();
            List<string> sink_descriptions = new List<string> ();
            c.Ready += delegate {
                c.EnumerateSinks ((SinkInfo info, int eol) => {
                    if (eol == 0) {
                        sink_descriptions.Add (info.Description);
                    } else {
                        Assert.Contains ("Test Description", sink_descriptions);
                        callback_called.Set ();
                    }
                });
            };
            RunUntilEventSignal (c.Connect, callback_called, "Timeout waiting for EnumerateSinks");
        }

        [Test ()]
        public void SinksAreValidOutsideEnumerateCallback ()
        {
            helper.AddSink ("Test_Sink_1", "Description1");

            Context c = new Context ();
            List<Sink> sinks = new List<Sink> ();
            c.ConnectAndWait ();
            Context.SinkCallback AppendToListCB = (Sink s) => {
                sinks.Add (s);
            };
            using (Operation o = c.EnumerateSinks (AppendToListCB)) {
                o.Wait ();
            }
            Assert.Contains ("Description1", (from sink in sinks select sink.Description).ToArray ());
            Assert.Contains ("Test_Sink_1", (from sink in sinks select sink.Name).ToArray ());
        }

        [Test()]
        public void VolumeChangeCallbackRuns ()
        {
            var volume_set = new EventWaitHandle (false, EventResetMode.AutoReset);
            Context c = new Context ();
            Volume vol = new Volume ();    //Dummy, because compiler can't see the set in the delegate.
            c.ConnectAndWait ();
            Operation o;
            using (o = c.GetSinkInfoByIndex (1, (SinkInfo info, int eol) => {if (eol == 0) {vol = info.volume;}})) {
                o.Wait ();
            }
            vol.Set (1);
            c.SetSinkVolume (1, vol, (int success) => {volume_set.Set ();});
            RunUntilEventSignal (c.Connect, volume_set, "Timeout waiting for volume set");
        }

        [Test()]
        [ExpectedException (typeof (Exception), ExpectedMessage = "Error setting sink volume: Invalid argument")]
        public void VolumeChangeThrowsOnInvalidArgument ()
        {
            Context c = new Context ();
            Volume invalidVol = new Volume ();
            c.ConnectAndWait ();
            using (Operation o = c.SetSinkVolume (1, invalidVol, (_) => {})) {
                o.Wait ();
            }
        }

        [Test()]
        public void SuccessfulConnectionHasErrorOK ()
        {
            Context c = new Context ();
            c.ConnectAndWait ();
            Assert.AreEqual (Error.Code.OK, c.LastError.ErrorCode);
            Assert.AreEqual ("OK", c.LastError.Message);
        }

        [Test()]
        public void GetSinkInfoByIndexReturns ()
        {
            Context c = new Context ();
            c.ConnectAndWait ();
            ManualResetEvent callbackCalled = new ManualResetEvent (false);
            using (Operation o = c.GetSinkInfoByIndex (0,
                                                       (_, __) => callbackCalled.Set ())) {
                o.Wait ();
            }

            Assert.IsTrue (callbackCalled.WaitOne (0));
        }

        [Test()]
        public void GetServerInfoReturns ()
        {
            Context c = new Context ();
            c.ConnectAndWait ();
            ManualResetEvent callbackCalled = new ManualResetEvent (false);
            using (Operation o = c.GetServerInfo ((ServerInfo info) => {callbackCalled.Set ();})) {
                o.Wait ();
            }
            Assert.IsTrue (callbackCalled.WaitOne (0));
        }

        [Test()]
        public void ServerNameIsPulseaudio ()
        {
            Context c = new Context ();
            c.ConnectAndWait ();
            ServerInfo info = new ServerInfo (new NativeServerInfo ());
            using (Operation o = c.GetServerInfo ((ServerInfo i) => info = i)) {
                o.Wait ();
            }
            Assert.AreEqual ("pulseaudio", info.Name);
        }

        [Test()]
        public void DefaultChannelFormatIsStereo ()
        {
            Context c = new Context ();
            c.ConnectAndWait ();
            ServerInfo info = new ServerInfo (new NativeServerInfo ());
            using (Operation o = c.GetServerInfo ((ServerInfo i) => info = i)) {
                o.Wait ();
            }
            Assert.AreEqual (ChannelMap.StereoMapping ().channels, info.DefaultChannelMap.channels);
        }
    }
}
