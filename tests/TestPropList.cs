//
//  Copyright © 2009 Christopher James Halse Rogers <raof@ubuntu.com>
//
//  TestPropList.cs is a part of Pulseaudio#
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
    public class TestPropList
    {
        [Test()]
        public void NewlyCreatedPropListIsEmpty ()
        {
            PropList l = new PropList ();
            Assert.IsTrue (l.Empty);
        }

        [Test()]
        public void NewlyCreatedPropListHasZeroEntries ()
        {
            PropList l = new PropList ();
            Assert.AreEqual (0, l.Count);
        }

        [Test]
        public void AddingEntryIncreasesCount ()
        {
            PropList l = new PropList ();
            l[Properties.ApplicationIconName] = "value";
            Assert.AreEqual (1, l.Count);
        }

        [Test]
        public void AddedNonStandardEntryHasCorrectValue ()
        {
            PropList l = new PropList ();
            byte[] data = {
                0,
                1,
                2,
                3,
                4
            };
            l["key"] = data;
            Assert.AreEqual (data, l["key"]);
        }

        [Test]
        public void AddMultipleEntriesAndCheckForMemoryCorruption ()
        {
            PropList l = new PropList ();
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding ();
            string first = "mydata";
            byte[] second = {
                0,
                5,
                10,
                25
            };
            byte[] third = {
                255,
                125,
                5
            };
            l["first"] = encoder.GetBytes (first);
            l["second"] = second;
            l["third"] = third;

            Assert.AreEqual (third, l["third"]);
            Assert.AreEqual (second, l["second"]);
            Assert.AreEqual (first, encoder.GetString (l["first"]));
        }

        [Test]
        public void AddStandardEntry ()
        {
            PropList l = new PropList ();
            l[Properties.ApplicationPID] = "1234";
            Assert.AreEqual ("1234", l[Properties.ApplicationPID]);
        }
    }
}
