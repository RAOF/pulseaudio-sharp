//
//  Helper.cs
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
using System.Collections.Generic;
using System.Diagnostics;

namespace Pulseaudio
{
    public class Helper : IDisposable
    {
        private bool disposed = false;
        private List<int> modulesLoaded;

        public Helper ()
        {
            modulesLoaded = new List<int> ();
        }

        /// <summary>
        /// Add a null-sink to the running PulseAudio daemon
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/>.  Name of the sink that will be added.
        /// </param>
        /// <param name="description">
        /// A <see cref="System.String"/>.  Description of the sink that will be added.
        /// </param>
        public void AddSink (string name, string description)
        {
            string cmdLineOpts = String.Format ("load-module module-null-sink sink_name=\\\"{0}\\\" sink_properties=\\\"device.description=\\\\\\\"{1}\\\\\\\"\\\"", name, description);
            ProcessStartInfo pactlInfo = new ProcessStartInfo ("/usr/bin/pactl", cmdLineOpts);
            pactlInfo.RedirectStandardOutput = true;
            pactlInfo.UseShellExecute = false;
            var pactl = Process.Start (pactlInfo);

            pactl.WaitForExit ();

            if (pactl.ExitCode != 0) {
                throw new Exception (String.Format ("Unable to add sink to pulseaudio instance.  Return code: {0}", pactl.ExitCode));
            }
            modulesLoaded.Add (int.Parse (pactl.StandardOutput.ReadLine ()));
        }

        public void AddSink (string name)
        {
            AddSink (name, "Null sink, no description added");
        }

        public void Dispose ()
        {
            if (!disposed) {
                UnloadModules ();
                GC.SuppressFinalize (this);

                modulesLoaded = null;
                disposed = true;
            }
        }

        private void UnloadModules ()
        {
            foreach (int moduleIndex in modulesLoaded) {
                string cmdLineOpts = String.Format ("unload-module {0}", moduleIndex);
                Process.Start ("/usr/bin/pactl", cmdLineOpts).WaitForExit ();
            }
        }

        ~Helper ()
        {
            if (modulesLoaded != null) {
                UnloadModules ();
            }
        }
    }
}
