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
using System.Threading;
using NUnit.Framework;

using g = GLib;
using Pulseaudio.GLib;

namespace Pulseaudio
{


    [TestFixture()]
    public class TestSink
    {
        private Helper helper;
        [SetUp]
        public void SetUp ()
        {
            helper = new Helper ();
        }
        [TearDown]
        public void TearDown ()
        {
            helper.Dispose ();
        }

        private void RunUntilEventSignal (Action action, EventWaitHandle until, string timeoutMessage)
        {
            var timeout = new EventWaitHandle (false, EventResetMode.AutoReset);
            g::Timeout.Add (1000, () =>
            {
                timeout.Set ();
                return false;
            });
            action ();
            while (!until.WaitOne (0, true)) {
                g::MainContext.Iteration (false);
                if (timeout.WaitOne (0, true)) {
                    Assert.Fail (timeoutMessage);
                }
            }
        }

        [Test()]
        public void TestGetName ()
        {
            string sinkDescription = "Just some description string";
            helper.AddSink ("Test sink", sinkDescription);
            Context c = new Context ();
            c.ConnectAndWait ();
            var sinks = new List<Sink> ();
            using (Operation o = c.EnumerateSinks ((Sink s) => sinks.Add (s))) {
                o.Wait ();
            }
            Assert.Contains (sinkDescription, (from sink in sinks select sink.Description).ToList ());
        }

        [Test()]
        public void TestVolumeChangedCallbackRuns ()
        {
            string testSinkName = "VolumeChangeTestSink";
            helper.AddSink (testSinkName);
            Context c = new Context ();
            c.ConnectAndWait ();

            ManualResetEvent callbackTriggered = new ManualResetEvent (false);

            var sinks = new List<Sink> ();
            using (Operation o = c.EnumerateSinks ((Sink sink) => sinks.Add (sink))) {
                o.Wait ();
            }
            Sink volumeTestSink = sinks.First ((Sink s) => s.Name == testSinkName);
            volumeTestSink.VolumeChanged += (_, __) => {
                callbackTriggered.Set ();
            };
            Volume vol = new Volume ();
            using (Operation o = volumeTestSink.GetVolume ((Volume v) => vol = v)) {
                o.Wait ();
            }
            vol.Modify (0.1);
            using (Operation o = volumeTestSink.SetVolume (vol, (_) => {;})) {
                o.Wait ();
            }
            RunUntilEventSignal (() => {;}, callbackTriggered, "Timeout waiting for VolumeChanged signal");
        }

        [Test]
        public void DisposeUnhooksVolumeCallbacks ()
        {
            Context c = new Context ();
            c.ConnectAndWait ();

            helper.AddSink ("dispose_test_sink");

            var sinks = new List<Sink> ();
            using (Operation o = c.EnumerateSinks ((Sink sink) => sinks.Add (sink))) {
                o.Wait ();
            }
            Sink testSink = sinks.Where ((Sink s) => s.Name == "dispose_test_sink").First ();

            testSink.VolumeChanged += delegate(object sender, Sink.VolumeChangedEventArgs e) {
                Assert.Fail ("VolumeChanged callback run after Sink was disposed");
            };

            while (g::MainContext.Iteration (false)) {}
            testSink.Dispose ();

            // Find the sink again...
            sinks.Clear ();
            using (Operation o = c.EnumerateSinks ((Sink sink) => sinks.Add (sink))) {
                o.Wait ();
            }
            testSink = sinks.Where ((Sink s) => s.Name == "dispose_test_sink").First ();
            Volume vol = new Volume ();
            using (Operation o = testSink.GetVolume (v => vol = v)) {
                o.Wait ();
            }
            vol.Modify (0.5);
            using (Operation o = testSink.SetVolume (vol, (o) => {;})) {
                o.Wait ();
            }
            //Flush the mainloop
            while (g::MainContext.Iteration (false)) {}
        }

        [Test]
        public void VolumePropertyUpdatedWithVolumeChange ()
        {
            string testSinkName = "VolumeChangeTestSink";
            helper.AddSink (testSinkName);
            Context c = new Context ();
            c.ConnectAndWait ();

            var sinks = new List<Sink> ();
            using (Operation o = c.EnumerateSinks ((Sink sink) => sinks.Add (sink))) {
                o.Wait ();
            }
            Sink volumeTestSink = sinks.First ((Sink s) => s.Name == testSinkName);

            Volume vol = volumeTestSink.Volume;
            vol.Modify (0.1);
            Assert.IsFalse (vol.Equals (volumeTestSink.Volume));
            using (Operation o = volumeTestSink.SetVolume (vol, (_) => {;})) {
                o.Wait ();
            }

            while (g::MainContext.Iteration (false)) {}
            // We need a little time to let the volume changed events bubble through.
            Thread.Sleep (1);
            while (g::MainContext.Iteration (false)) {}
            Assert.AreEqual (vol, volumeTestSink.Volume);
        }
    }
}
