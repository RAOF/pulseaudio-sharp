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
        private g.Process aplay;
        [SetUp]
        public void SetUp ()
        {
            g.Process.SpawnAsync (null, new string[] {"/usr/bin/aplay", "tests/15seconds.wav"}, null, g.SpawnFlags.StdoutToDevNull | g.SpawnFlags.StderrToDevNull, null, out aplay);
            System.Threading.Thread.Sleep (200);
        }

        [TearDown]
        public void TearDown ()
        {
            aplay.Close ();
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
            List<SinkInput> inputs = new List<SinkInput> ();
            using (Operation opn = c.EnumerateSinkInputs ((SinkInput input, int eol) => inputs.Add (input))) {
                opn.Wait ();
            }
            SinkInput aplayInput = inputs.FirstOrDefault ((SinkInput input) => input.Properties[Properties.ApplicationProcessBinary] == "aplay");
            Assert.IsNotNull (aplayInput);
        }
    }
}
