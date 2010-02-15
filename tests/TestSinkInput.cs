//
//  TestSinkInput.cs
//
//  Author:
//       Christopher James Halse Rogers <raof@ubuntu.com>
//
//  Copyright (c) 2009 Christopher James Halse Rogers <raof@ubuntu.com>
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using Pulseaudio.GLib;
using g = GLib;

namespace Pulseaudio
{
    [TestFixture]
    public class TestSinkInput
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
            GC.Collect ();
        }

        /*
         * This test requires an active SinkInput; ie: something needs to be playing sound
        */
        [Test]
        public void TestVolumeSuccessCallbackRuns ()
        {
            Context c = new Context ();
            var cbCalled = new ManualResetEvent (false);
            c.ConnectAndWait ();

            helper.SpawnAplaySinkInput ();

            List<SinkInput> sinkInputs = new List<SinkInput> ();
            using (Operation opn = c.EnumerateSinkInputs ((SinkInput input, int eol) => sinkInputs.Add (input))) {
                opn.Wait ();
            }
            Volume oldVol = sinkInputs[0].Volume.Copy ();
            Volume v = sinkInputs[0].Volume.Copy ();
            v.Modify (1.0);
            using (Operation opn = sinkInputs[0].SetVolume (v, (_) => cbCalled.Set ())) {
                opn.Wait ();
            }
            sinkInputs[0].SetVolume (oldVol, (_) => {}).Dispose ();
            Assert.IsTrue (cbCalled.WaitOne (0));
        }

        [Test]
        public void TestAplaySinkInputHasCorrectApplicationBinaryProperty ()
        {
            Context c = new Context ();
            c.ConnectAndWait ();

            helper.SpawnAplaySinkInput ();

            List<SinkInput> inputs = new List<SinkInput> ();
            using (Operation opn = c.EnumerateSinkInputs ((SinkInput input, int eol) => {if (eol == 0) {inputs.Add (input);}})) {
                opn.Wait ();
            }
            SinkInput aplayInput = inputs.FirstOrDefault ((SinkInput input) => input.Properties[Properties.ApplicationProcessBinary] == "aplay");
            Assert.IsNotNull (aplayInput);
        }

        [Test]
        public void VolumeFieldIsUpdated ()
        {
            Context c = new Context ();
            c.ConnectAndWait ();

            helper.SpawnAplaySinkInput ();

            SinkInput aplay = null;
            using (Operation opn = c.EnumerateSinkInputs ((SinkInput input, int eol) => {
                if (eol == 0) {
                    if (input.Properties[Properties.ApplicationProcessBinary] == "aplay") {
                        aplay = input;
                    }
                }
            })) {
                opn.Wait ();
            }

            Volume vol = aplay.Volume;
            vol.Modify (0.1);
            Assert.AreNotEqual (vol, aplay.Volume);
            using (Operation opn = aplay.SetVolume (vol, (_) => {;})) {
                opn.Wait ();
            }

            Helper.DrainEventLoop ();
            // Wait for volume change events to percolate
            Thread.Sleep (200);
            Helper.DrainEventLoop ();
            Thread.Sleep (10);
            Helper.DrainEventLoop ();

            try {
                Assert.AreEqual (vol, aplay.Volume);
            } finally {
                //Reset the volume to 100%
                vol.Reset ();
                using (Operation opn = aplay.SetVolume (vol, (_) => {;})) {
                    opn.Wait ();
                }
            }
        }

        [Test]
        public void VolumeChangedEventFires ()
        {
            Context c = new Context ();
            c.ConnectAndWait ();

            helper.SpawnAplaySinkInput ();

            SinkInput aplay = null;
            using (Operation opn = c.EnumerateSinkInputs ((SinkInput input, int eol) => {
                if (eol == 0) {
                    if (input.Properties[Properties.ApplicationProcessBinary] == "aplay") {
                        aplay = input;
                    }
                }
            })) {
                opn.Wait ();
            }
            Assert.IsNotNull (aplay, "Testing error: aplay SinkInput not found");

            var volumeChangedEventTriggered = new ManualResetEvent (false);
            aplay.VolumeChanged += delegate(object sender, SinkInput.VolumeChangedEventArgs e) {
                volumeChangedEventTriggered.Set ();
            };
            // Ensure the volume changed callback is hooked up.
            Helper.DrainEventLoop ();

            Volume vol = aplay.Volume;
            vol.Modify (0.1);

            using (Operation o = aplay.SetVolume (vol, (_)=>{;})) {
                o.Wait ();
            }

            Helper.RunUntilEventSignal (() => {;}, volumeChangedEventTriggered, "Timeout waiting for VolumeChanged signal");
        }

        [Test]
        public void MoveToSinkChangesSink ()
        {
            string sinkNameA = "TestSinkDestinationA";
            string sinkNameB = "TestSinkDestinationB";
            helper.AddSink (sinkNameA);
            helper.AddSink (sinkNameB);

            Context c = new Context ();
            c.ConnectAndWait ();

            Sink destinationA = null;
            Sink destinationB = null;
            using (Operation o = c.EnumerateSinks ((Sink s) => {
                if (s.Name == sinkNameA) {
                    destinationA = s;
                } else if (s.Name == sinkNameB) {
                    destinationB = s;
                }
            })) {
                o.Wait ();
            }
            Assert.IsNotNull (destinationA);
            Assert.IsNotNull (destinationB);

            helper.SpawnAplaySinkInput ();

            SinkInput aplay = null;
            using (Operation opn = c.EnumerateSinkInputs ((SinkInput input, int eol) => {
                if (eol == 0) {
                    if (input.Properties[Properties.ApplicationProcessBinary] == "aplay") {
                        aplay = input;
                    }
                }
            })) {
                opn.Wait ();
            }
            Assert.IsNotNull (aplay, "Testing error: aplay SinkInput not found");

            AutoResetEvent propertiesChanged = new AutoResetEvent (false);
            aplay.PropertiesChanged += delegate(object sender, SinkInput.PropertyChangedEventArgs e) {
                propertiesChanged.Set ();
            };

            using (Operation o = aplay.MoveTo (destinationA, (_) => {;})) {
                o.Wait ();
            }
            Helper.RunUntilEventSignal (() => {;}, propertiesChanged, "Timeout waiting for first sink change");
            Assert.AreEqual (destinationA.Index, aplay.Sink, "Failed to move stream to first sink");

            using (Operation o = aplay.MoveTo (destinationB, (_) => {;})) {
                o.Wait ();
            }
            Helper.RunUntilEventSignal (() => {;}, propertiesChanged, "Timeout waiting for second sink change");
            Assert.AreEqual (destinationB.Index, aplay.Sink, "Failed to move stream to second sink");
        }
    }
}
