//
//  Copyright © 2009 Christopher James Halse Rogers <raof@ubuntu.com>
//
//  TestVolume.cs is a part of Pulseaudio#
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
using NUnit.Framework;

namespace Pulseaudio
{
    [TestFixture()]
    public class TestVolume
    {
        [Test()]
        public void TestResetSetsToNorm()
        {
            Volume v = new Volume ();
            Volume r = new Volume ();
            v.Reset ();
            r.Reset ();
            Assert.IsTrue (r == v);
        }

        [Test()]
        public void TestVolumeEquals()
        {
            Volume v = new Volume ();
            Volume r = new Volume ();
            v.Reset ();
            r.Reset ();
            Assert.IsTrue (v.Equals (r));
        }

        [Test()]
        public void SetSameVolume ()
        {
            Volume a = new Volume ();
            Volume b = new Volume ();
            a.Set (0.5);
            b.Set (0.5);
            Assert.IsTrue (a == b);
        }

        [Test()]
        public void SetDifferentVolume ()
        {
            Volume a = new Volume ();
            Volume b = new Volume ();
            a.Set (0.5);
            b.Set (0);
            Assert.IsTrue (a != b);
        }
    }
}
