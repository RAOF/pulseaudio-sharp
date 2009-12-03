//
//  Copyright © 2009 Christopher James Halse Rogers <raof@ubuntu.com>
//
//  Util.cs is a part of Pulseaudio#
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
    public enum ErrorCode {
        OK = 0,                 /**< No error */
        AccessFailure,          /**< Access failure */
        UnknownCommand,         /**< Unknown command */
        InvalidArgument,        /**< Invalid argument */
        EntityExists,           /**< Entity exists */
        NoSuchEntity,           /**< No such entity */
        ConnectionRefused,      /**< Connection refused */
        Protocol,               /**< Protocol error */
        Timeout,                /**< Timeout */
        NoAuthKey,              /**< No authorization key */
        Internal,               /**< Internal error */
        ConnectionTerminated,   /**< Connection terminated */
        Killed,                 /**< Entity killed */
        InvalidServer,          /**< Invalid server */
        ModuleInitFailed,       /**< Module initialization failed */
        BadState,               /**< Bad state */
        NoData,                 /**< No data */
        Version,                /**< Incompatible protocol version */
        TooLarge,               /**< Data too large */
        NotSupported,           /**< Operation not supported \since 0.9.5 */
        Unknown,                /**< The error code was unknown to the client */
        NoExtension,            /**< Extension does not exist. \since 0.9.12 */
        Obsolete,               /**< Obsolete functionality. \since 0.9.15 */
        NotImplemented,         /**< Missing implementation. \since 0.9.15 */
        Forked,                 /**< The caller forked without calling execve() and tried to reuse the context. \since 0.9.15 */
        IO,                     /**< An IO error happened. \since 0.9.16 */
        PA_ERR_MAX              /**< Not really an error but the first invalid error code */
    }

    public class Error
    {
        public enum Code {
            OK = 0,                 /**< No error */
            AccessFailure,          /**< Access failure */
            UnknownCommand,         /**< Unknown command */
            InvalidArgument,        /**< Invalid argument */
            EntityExists,           /**< Entity exists */
            NoSuchEntity,           /**< No such entity */
            ConnectionRefused,      /**< Connection refused */
            Protocol,               /**< Protocol error */
            Timeout,                /**< Timeout */
            NoAuthKey,              /**< No authorization key */
            Internal,               /**< Internal error */
            ConnectionTerminated,   /**< Connection terminated */
            Killed,                 /**< Entity killed */
            InvalidServer,          /**< Invalid server */
            ModuleInitFailed,       /**< Module initialization failed */
            BadState,               /**< Bad state */
            NoData,                 /**< No data */
            Version,                /**< Incompatible protocol version */
            TooLarge,               /**< Data too large */
            NotSupported,           /**< Operation not supported \since 0.9.5 */
            Unknown,                /**< The error code was unknown to the client */
            NoExtension,            /**< Extension does not exist. \since 0.9.12 */
            Obsolete,               /**< Obsolete functionality. \since 0.9.15 */
            NotImplemented,         /**< Missing implementation. \since 0.9.15 */
            Forked,                 /**< The caller forked without calling execve() and tried to reuse the context. \since 0.9.15 */
            IO,                     /**< An IO error happened. \since 0.9.16 */
            PA_ERR_MAX              /**< Not really an error but the first invalid error code */
        }

        private Code error;
        private string error_string;

        internal Error (Code error)
        {
            this.error = error;
        }

        public Code ErrorCode {
            get {
                return error;
            }
        }

        public string Message {
            get {
                if (string.IsNullOrEmpty (error_string)) {
                    error_string = Marshal.PtrToStringAnsi (pa_strerror (Convert.ToInt32 (error)));
                }
                return error_string;
            }
        }

        public override string ToString ()
        {
            return string.Format("[Error: {0}]", Message);
        }

        public override bool Equals (object obj)
        {
            Error other = obj as Error;
            if (other != null) {
                return (error == other.error);
            }
            if (obj is Code) {
                return error == (Code)obj;
            }
            return base.Equals (obj);
        }

        public bool Equals (Error b)
        {
            return error == b.error;
        }

        public override int GetHashCode ()
        {
            return error.GetHashCode ();
        }

        public static bool operator==(Error a, Code b)
        {
            return a.error == b;
        }

        public static bool operator!=(Error a, Code b)
        {
            return !(a==b);
        }

        public static bool operator==(Code a, Error b)
        {
            return b == a;
        }

        public static bool operator!=(Code a, Error b)
        {
            return !(a==b);
        }

        public static bool operator==(Error a, Error b)
        {
            if (Object.ReferenceEquals (a, b)) {
                return true;
            }
            return a.error == b.error;
        }

        public static bool operator!=(Error a, Error b)
        {
            return !(a==b);
        }

        [DllImport ("pulse")]
        private static extern IntPtr pa_strerror (int error);
    }

    public static class Util
    {
        public static string ErrorStringFromErrno (ErrorCode error)
        {
            return Marshal.PtrToStringAnsi (pa_strerror (Convert.ToInt32 (error)));
        }

        [DllImport ("pulse")]
        private static extern IntPtr pa_strerror (int error);
    }
}
