//
//  TestUnmanagedCallbackManager.cs
//
//  Author:
//       Christopher James Halse Rogers <raof@ubuntu.com>
//
//  Copyright © 2010 Christopher James Halse Rogers <raof@ubuntu.com>
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
using NUnit.Framework;

namespace Pulseaudio
{
    [TestFixture]
    public class TestUnmanagedCallbackManager
    {
        [Test]
        public void AddDelegateSucceeds ()
        {
            UnmanagedCallbackManager manager = new UnmanagedCallbackManager ();

            manager.AddDelegate (() => {}, manager.NewCookie ());
        }

        [Test]
        public void SuccessiveCookieCallsReturnDifferentCookies ()
        {
            UnmanagedCallbackManager manager = new UnmanagedCallbackManager ();

            Assert.AreNotEqual (manager.NewCookie (), manager.NewCookie ());
        }

        [Test]
        [ExpectedException (ExceptionType = typeof(System.Exception))]
        public void AddingTwoCallbacksWithTheSameCookieIsAnError ()
        {
            UnmanagedCallbackManager manager = new UnmanagedCallbackManager ();

            int cookie = manager.NewCookie ();
            manager.AddDelegate (() => {}, cookie);
            manager.AddDelegate (() => {}, cookie);
        }

        [Test]
        [ExpectedException (ExceptionType = typeof (System.Exception))]
        public void RemovingCallbackTwiceIsAnError ()
        {
            UnmanagedCallbackManager manager = new UnmanagedCallbackManager ();

            int cookie = manager.NewCookie ();
            manager.AddDelegate (() => {}, cookie);
            manager.RemoveDelegate (cookie);
            manager.RemoveDelegate (cookie);
        }
    }
}
