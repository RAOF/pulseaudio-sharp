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
using System.Threading;
using g = GLib;
using NUnit.Framework;

using Pulseaudio.GLib;

namespace Pulseaudio
{
    public class Helper : IDisposable
    {
        private bool disposed = false;
        private List<int> modulesLoaded;
        private List<Process> processesSpawned;
        private Context ctx;

        public Helper ()
        {
            modulesLoaded = new List<int> ();
            processesSpawned = new List<Process> ();
            ctx = new Context ("PulseAudio testing helper");
            ctx.ConnectAndWait ();
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
        /// <returns>
        /// The <see cref="PulseAudio.Sink"/> matching the added sink.
        /// </returns>
        public Sink AddSink (Context constructOn, string name, string description)
        {
            int moduleNumber = AddSink (name, description);

            Sink addedSink = null;
            using (Operation o = constructOn.EnumerateSinks ((Sink s) => { if (s.OwnerModule == moduleNumber) addedSink = s; })) {
                o.Wait ();
            }
            Assert.IsNotNull (addedSink, "Failed to find added test sink.");
            return addedSink;
        }

        public Sink AddSink (Context constructOn, string name)
        {
            return AddSink (constructOn, name, "Null sink, no description added");
        }

        public int AddSink (string name)
        {
            return AddSink (name, "Null sink, no description added");
        }

        public int AddSink (string name, string description)
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
            int moduleNumber = int.Parse (pactl.StandardOutput.ReadLine ());
            modulesLoaded.Add (moduleNumber);
            return moduleNumber;
        }

        public void SpawnAplaySinkInput ()
        {
            var inputAdded = new ManualResetEvent (false);
            EventHandler<ServerEventArgs> eventHandler = delegate (object sender, ServerEventArgs args) {
                if (args.Type == EventType.Added) {
                    inputAdded.Set ();
                }
            };
            ctx.SinkInputEvent += eventHandler;
            Helper.DrainEventLoop ();

            ProcessStartInfo p = new ProcessStartInfo ("/usr/bin/aplay", System.IO.Path.Combine (BuildConfiguration.TestDataDir, "15seconds.wav"));
            p.RedirectStandardOutput = true;
            p.RedirectStandardError = true;
            p.UseShellExecute = false;
            processesSpawned.Add (Process.Start (p));

            //Wait until we get a new SinkInput event.
            while (!inputAdded.WaitOne (0)) {
                Helper.DrainEventLoop ();
            }

            //And unregister our handler
            ctx.SinkInputEvent -= eventHandler;
        }

        public static void RunUntilEventSignal (Action action, EventWaitHandle until, string timeoutMessage)
        {
            var timeout = new EventWaitHandle (false, EventResetMode.AutoReset);
            g::Timeout.Add (1000, () =>
            {
                timeout.Set ();
                return false;
            });
            action ();
            while (!until.WaitOne (0, true)) {
                DrainEventLoop ();
                if (timeout.WaitOne (0, true)) {
                    Assert.Fail (timeoutMessage);
                }
            }
        }

        public static void DrainEventLoop ()
        {
            while (g::MainContext.Iteration (false)) {}
        }

        public void Dispose ()
        {
            if (!disposed) {
                UnloadModules ();
                KillProcesses ();
                ctx.Dispose ();
                GC.SuppressFinalize (this);

                processesSpawned = null;
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

        private void KillProcesses ()
        {
            foreach (Process p in processesSpawned) {
                p.Kill ();
            }
        }
    }
}
