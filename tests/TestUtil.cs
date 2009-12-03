//
//  Copyright © 2009 Christopher James Halse Rogers <raof@ubuntu.com>
//
//  TestUtil.cs is a part of Pulseaudio#
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
    public class TestUtil
    {
        [Test()]
        public void TestSuccessErrorString ()
        {
            Assert.AreEqual ("OK", (new Error (Error.Code.OK)).Message);
        }

        [Test()]
        public void TestInvalidArgumentErrorString ()
        {
            Assert.AreEqual ("Invalid argument", (new Error (Error.Code.InvalidArgument)).Message);
        }

        [Test()]
        public void TestErrorToStringFormatting ()
        {
            Error e = new Error (Error.Code.OK);
            Assert.AreEqual ("[Error: OK]", e.ToString ());
        }

        [Test()]
        public void TestEqualCodesMeanEqualErrors ()
        {
            Error first = new Error (Error.Code.InvalidArgument);
            Error second = new Error (Error.Code.InvalidArgument);

            Assert.IsTrue (first.Equals (second));
        }

        [Test()]
        public void TestEqualityOperatorReturnsTrueForEqualErrors ()
        {
            Error first = new Error (Error.Code.BadState);
            Error second = new Error (Error.Code.BadState);

            Assert.IsTrue (first == second);
        }

        [Test()]
        public void TestEqualityOperatorErrorAndCode ()
        {
            Error e = new Error (Error.Code.ConnectionTerminated);

            Assert.IsTrue (e == Error.Code.ConnectionTerminated);
        }

        [Test()]
        public void TestEqualityOperatorReturnsFalseForUnequalErrors ()
        {
            Error first = new Error (Error.Code.AccessFailure);
            Error second = new Error (Error.Code.OK);

            Assert.IsFalse (first == second);
        }

        [Test()]
        public void TestEqualityOperatorErrorAndUnequalCode ()
        {
            Error e = new Error (Error.Code.EntityExists);

            Assert.IsFalse (e == Error.Code.Forked);
        }
    }
}
