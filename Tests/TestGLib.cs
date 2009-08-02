//  
//  Copyright © 2009 Christopher James Halse Rogers <raof@ubuntu.com>
// 
//  TestGLib.cs is a part of Pulseaudio#
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
using Pulseaudio.GLib;
using g = GLib;

namespace Pulseaudio
{
    [TestFixture()]
    public class TestGLib
    {
        [Test()]
        public void WaitExtensionBlocksUntilOperationComplete()
        {
            ManualResetEvent connection_ready = new ManualResetEvent (false);
            Context c = new Context ();
            c.Ready += delegate {
                connection_ready.Set ();
            };
            c.Connect ();

            while (!connection_ready.WaitOne (0, true)) {
                g::MainContext.Iteration ();
            }
            
            Operation opn = c.EnumerateSinks (new Context.SinkInfoCallback ((a,b) => {System.Console.Write ("Hello");}));
            opn.Wait ();
            Assert.IsTrue (opn.State == Operation.Status.Done);
        }
    }
}
