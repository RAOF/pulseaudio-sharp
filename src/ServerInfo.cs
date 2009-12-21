//
//  ServerInfo.cs
//
//  Author:
//       Christopher James Halse Rogers <raof@ubuntu.com>
//
//  Copyright © 2009 Christopher James Halse Rogers <raof@ubuntu.com>
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
using System.Runtime.InteropServices;

namespace Pulseaudio
{
    [StructLayout(LayoutKind.Sequential)]
    internal class NativeServerInfo
    {
        public IntPtr user_name;
        public IntPtr host_name;
        public IntPtr server_version;
        public IntPtr server_name;
        public SampleSpec sample_spec;
        public IntPtr default_sink_name;
        public IntPtr default_source_name;
        public UInt32  cookie;
        public ChannelMap channel_map;
    }

    public class ServerInfo
    {
        internal ServerInfo (NativeServerInfo info)
        {
            UserName = Marshal.PtrToStringAnsi (info.user_name);
            HostName = Marshal.PtrToStringAnsi (info.host_name);
            Version = Marshal.PtrToStringAnsi (info.server_version);
            Name = Marshal.PtrToStringAnsi (info.server_name);
            DefaultSampleSpec = info.sample_spec;
            DefaultSinkName = Marshal.PtrToStringAnsi (info.default_sink_name);
            DefaultSourceName = Marshal.PtrToStringAnsi (info.default_source_name);
            Cookie = info.cookie;
            DefaultChannelMap = info.channel_map;
        }

        /// <summary>
        /// Name of the user running the daemon process
        /// </summary>
        public string UserName {
            get;
            private set;
        }
        /// <summary>
        /// Host name of the machine the daemon is running on
        /// </summary>
        public string HostName {
            get;
            private set;
        }
        /// <summary>
        /// Version string of the daemon
        /// </summary>
        public string Version {
            get;
            private set;
        }
        /// <summary>
        /// Package name of the daemon (usually "pulseaudio")
        /// </summary>
        public string Name {
            get;
            private set;
        }
        /// <summary>
        /// Default sample specification for this daemon
        /// </summary>
        public SampleSpec DefaultSampleSpec {
            get;
            private set;
        }
        /// <summary>
        /// Name of the default sink for this daemon
        /// </summary>
        public string DefaultSinkName {
            get;
            private set;
        }
        /// <summary>
        /// Name of the default source for this daemon
        /// </summary>
        public string DefaultSourceName {
            get;
            private set;
        }
        /// <summary>
        /// A random cookie identifying this daemon process
        /// </summary>
        public UInt32 Cookie {
            get;
            private set;
        }
        /// <summary>
        /// Default channel map for this server
        /// </summary>
        public ChannelMap DefaultChannelMap {
            get;
            private set;
        }
    }
}
