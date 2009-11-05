//  
//  Copyright © 2009 Christopher James Halse Rogers <raof@ubuntu.com>
// 
//  TestMainLoop.cs is a part of Pulseaudio#
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

using NUnit.Framework;

namespace Pulseaudio
{
    [TestFixture()]
    public class TestMainLoop
    {
        [Test()]
        public void TestGLibMainLoopAPIIsStable ()
        {
            GLibMainLoop loop = new GLibMainLoop ();
            Assert.AreEqual (loop.GetAPI (), loop.GetAPI ());
        }

        [Test()]
        [ExpectedException()]
        public void TestGLibMainLoopCatchesNullMainLoop ()
        {
            GLibMainLoop loop = new GLibMainLoop ();
            loop.Dispose ();
            loop.GetAPI ();
        }
    }
}
