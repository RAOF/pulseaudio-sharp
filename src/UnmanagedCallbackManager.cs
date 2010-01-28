//
//  UnmanagedCallbackManager.cs
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
using System.Linq;
using System.Collections.Generic;

namespace Pulseaudio
{
    internal class UnmanagedCallbackManager
    {
        private Dictionary<int, Action> delegates;
        private List<int> pendingCookies;

        public UnmanagedCallbackManager ()
        {
            delegates = new Dictionary<int, Action> ();
            pendingCookies = new List<int> ();
        }

        public void AddDelegate (Action act, int cookie)
        {
            if (pendingCookies.Contains (cookie)) {
                delegates[cookie] = act;
                pendingCookies.Remove (cookie);
            } else {
                throw new Exception ();
            }
        }

        public void AddDelegate<T1, T2, T3, T4> (Action<T1, T2, T3, T4> act, int cookie)
        {
            AddDelegate (() => act(default(T1), default(T2), default(T3), default(T4)), cookie);
        }

        public void AddDelegate<T1, T2, T3> (Action<T1, T2, T3> act, int cookie)
        {
            AddDelegate (() => act(default (T1), default (T2), default (T3)), cookie);
        }

        public void RemoveDelegate (int cookie)
        {
            if (!delegates.ContainsKey (cookie)) {
                throw new Exception ();
            }
            delegates.Remove (cookie);
        }

        public int PendingCallbackCount {
            get {
                return delegates.Count ();
            }
        }

        public int NewCookie ()
        {
            int nextCookie, maxDelegateCookie, maxPendingCookie;
            maxDelegateCookie = delegates.Count () == 0 ? 0 : delegates.Keys.Max ();
            maxPendingCookie = pendingCookies.Count () == 0 ? 0 : pendingCookies.Max ();

            nextCookie = Math.Max (maxDelegateCookie, maxPendingCookie) + 1;
            pendingCookies.Add (nextCookie);
            return nextCookie;
        }
    }
}
