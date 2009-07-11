//  
//  Copyright © 2009 Christopher James Halse Rogers <raof@ubuntu.com>
// 
//  Operation.cs is a part of Pulseaudio#
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
using System.Runtime.InteropServices;

namespace Pulseaudio
{
    public sealed class Operation : IDisposable
    {
        public enum Status
        {
            Running,
            Done,
            Cancelled
        }
        
        private HandleRef handle;
        internal Operation(HandleRef handle)
        {
            this.handle = handle;
        }

        ~Operation ()
        {
            pa_operation_unref (handle);
        }

        public void Dispose ()
        {
            pa_operation_unref (handle);

            GC.SuppressFinalize (this);
        }

        public Status State {
            get {
                return pa_operation_get_state (handle);
            }
        }

        [DllImport ("pulse")]
        private static extern void pa_operation_unref (HandleRef opn);
        [DllImport ("pulse")]
        private static extern Status pa_operation_get_state (HandleRef opn);
        [DllImport ("pulse")]
        private static extern void pa_operation_ref (HandleRef opn);
        [DllImport ("pulse")]
        private static extern void pa_operation_cancel (HandleRef opn);
    }
}
