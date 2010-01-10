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
            Volume oldVol = sinkInputs[0].Vol.Copy ();
            Volume v = sinkInputs[0].Vol.Copy ();
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
            using (Operation opn = c.EnumerateSinkInputs ((SinkInput input, int eol) => inputs.Add (input))) {
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

            Volume vol = aplay.Vol;
            vol.Modify (0.1);
            Assert.AreNotEqual (vol, aplay.Vol);
            using (Operation opn = aplay.SetVolume (vol, (_) => {;})) {
                opn.Wait ();
            }

            while (g::MainContext.Iteration (false)) {}
            // Wait for volume change events to percolate
            Thread.Sleep (1);
            while (g::MainContext.Iteration (false)) {}

            try {
                Assert.AreEqual (vol, aplay.Vol);
            } finally {
                //Reset the volume to 100%
                vol.Reset ();
                using (Operation opn = aplay.SetVolume (vol, (_) => {;})) {
                    opn.Wait ();
                }
            }
        }
    }
}
