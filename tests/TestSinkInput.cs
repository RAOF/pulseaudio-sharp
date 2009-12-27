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
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using Pulseaudio.GLib;

namespace Pulseaudio
{
    [TestFixture]
    public class TestSinkInput
    {
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
            Assert.IsTrue (cbCalled.WaitOne (9));
        }
    }
}
