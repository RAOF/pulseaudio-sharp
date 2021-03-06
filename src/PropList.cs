//
//  Copyright © 2009 Christopher James Halse Rogers <raof@ubuntu.com>
//
//  PropList.cs is a part of Pulseaudio#
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
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Pulseaudio
{
    public enum Properties {
        /// <summary>
        /// For streams: localized media name, formatted as UTF-8. e.g. "Guns'N'Roses: Civil War".
        /// </summary>
        MediaName,
        /// <summary>
        /// For streams: localized media title if applicable, formatted as UTF-8. e.g. "Civil War"
        /// </summary>
        MediaTitle,
        /// <summary>
        /// For streams: localized media artist if applicable, formatted as UTF-8. e.g. "Guns'N'Roses"
        /// </summary>
        MediaArtist,
        /// <summary>
        /// For streams: localized media copyright string if applicable, formatted as UTF-8. e.g. "Evil Record Corp."
        /// </summary>
        MediaCopyright,
        /// <summary>
        /// For streams: localized media generator software string if applicable, formatted as UTF-8. e.g. "Foocrop AudioFrobnicator"
        /// </summary>
        MediaSoftware,
        /// <summary>
        /// For streams: media language if applicable, in standard POSIX format. e.g. "de_DE"
        /// </summary>
        MediaLanguage,
        /// <summary>
        /// For streams: source filename if applicable, in URI format or local path. e.g. "/home/lennart/music/foobar.ogg"
        /// </summary>
        MediaFilename,
        /// <summary>
        /// For streams: icon for the media. A binary blob containing PNG image data
        /// </summary>
        MediaIcon,
        /// <summary>
        /// For streams: an XDG icon name for the media. e.g. "audio-x-mp3"
        /// </summary>
        MediaIconName,
        /// <summary>
        /// For streams: logic role of this media. One of the strings "video", "music", "game", "event", "phone", "animation", "production", "a11y"
        /// </summary>
        MediaRole,
        /// <summary>
        /// For event sound streams: XDG event sound name. e.g. "message-new-email" (Event sound streams are those with media.role set to "event")
        /// </summary>
        EventID,
        /// <summary>
        /// For event sound streams: localized human readable one-line description of the event, formatted as UTF-8. e.g. "Email from lennart@example.com received."
        /// </summary>
        EventDescription,
        /// <summary>
        /// For event sound streams: absolute horizontal mouse position on the screen if the event sound was triggered by a mouse click, integer formatted as text string. e.g. "865"
        /// </summary>
        EventMouseX,
        /// <summary>
        /// For event sound streams: absolute vertical mouse position on the screen if the event sound was triggered by a mouse click, integer formatted as text string. e.g. "432"
        /// </summary>
        EventMouseY,
        /// <summary>
        /// For event sound streams: relative horizontal mouse position on the screen if the event sound was triggered by a mouse click, float formatted as text string, ranging from 0.0 (left side of the screen) to 1.0 (right side of the screen). e.g. "0.65"
        /// </summary>
        EventMouseHPos,
        /// <summary>
        /// For event sound streams: relative vertical mouse position on the screen if the event sound was triggered by a mouse click, float formatted as text string, ranging from 0.0 (top of the screen) to 1.0 (bottom of the screen). e.g. "0.43"
        /// </summary>
        EventMouseVPos,
        /// <summary>
        /// For event sound streams: mouse button that triggered the event if applicable, integer formatted as string with 0=left, 1=middle, 2=right. e.g. "0"
        /// </summary>
        EventMouseButton,
        /// <summary>
        /// For streams that belong to a window on the screen: localized window title. e.g. "Totem Music Player"
        /// </summary>
        WindowName,
        /// <summary>
        /// For streams that belong to a window on the screen: a textual id for identifying a window logically. e.g. "org.gnome.Totem.MainWindow"
        /// </summary>
        WindowID,
        /// <summary>
        /// For streams that belong to a window on the screen: window icon. A binary blob containing PNG image data
        /// </summary>
        WindowIcon,
        /// <summary>
        /// For streams that belong to a window on the screen: an XDG icon name for the window. e.g. "totem"
        /// </summary>
        WindowIconName,
        /// <summary>
        /// For streams that belong to a window on the screen: absolute horizontal window position on the screen, integer formatted as text string. e.g. "865". \since 0.9.17
        /// </summary>
        WindowX,
        /// <summary>
        /// For streams that belong to a window on the screen: absolute vertical window position on the screen, integer formatted as text string. e.g. "343". \since 0.9.17
        /// </summary>
        WindowY,
        /// <summary>
        /// For streams that belong to a window on the screen: window width on the screen, integer formatted as text string. e.g. "365". \since 0.9.17
        /// </summary>
        WindowWidth,
        /// <summary>
        /// For streams that belong to a window on the screen: window height on the screen, integer formatted as text string. e.g. "643". \since 0.9.17
        /// </summary>
        WindowHeight,
        /// <summary>
        /// For streams that belong to a window on the screen: relative position of the window center on the screen, float formatted as text string, ranging from 0.0 (left side of the screen) to 1.0 (right side of the screen). e.g. "0.65". \since 0.9.17
        /// </summary>
        WindowHPos,
        /// <summary>
        /// For streams that belong to a window on the screen: relative position of the window center on the screen, float formatted as text string, ranging from 0.0 (top of the screen) to 1.0 (bottom of the screen). e.g. "0.43". \since 0.9.17
        /// </summary>
        WindowVPos,
        /// <summary>
        /// For streams that belong to a window on the screen: if the windowing system supports multiple desktops, a comma seperated list of indexes of the desktops this window is visible on. If this property is an empty string, it is visible on all desktops (i.e. 'sticky'). The first desktop is 0. e.g. "0,2,3" \since 0.9.18
        /// </summary>
        WindowDesktop,
        /// <summary>
        /// For streams that belong to an X11 window on the screen: the X11 display string. e.g. ":0.0"
        /// </summary>
        WindowX11Display,
        /// <summary>
        /// For streams that belong to an X11 window on the screen: the X11 screen the window is on, an integer formatted as string. e.g. "0"
        /// </summary>
        WindowX11Screen,
        /// <summary>
        /// For streams that belong to an X11 window on the screen: the X11 monitor the window is on, an integer formatted as string. e.g. "0"
        /// </summary>
        WindowX11Monitor,
        /// <summary>
        /// For streams that belong to an X11 window on the screen: the window XID, an integer formatted as string. e.g. "25632"
        /// </summary>
        WindowX11XID,
        /// <summary>
        /// For clients/streams: localized human readable application name. e.g. "Totem Music Player"
        /// </summary>
        ApplicationName,
        /// <summary>
        /// For clients/streams: a textual id for identifying an application logically. e.g. "org.gnome.Totem"
        /// </summary>
        ApplicationID,
        /// <summary>
        /// For clients/streams: a version string e.g. "0.6.88"
        /// </summary>
        ApplicationVersion,
        /// <summary>
        /// For clients/streams: an XDG icon name for the application. e.g. "totem"
        /// </summary>
        ApplicationIconName,
        /// <summary>
        /// For clients/streams: application language if applicable, in standard POSIX format. e.g. "de_DE"
        /// </summary>
        ApplicationLanguage,
        /// <summary>
        /// For clients/streams on UNIX: application process PID, an integer formatted as string. e.g. "4711"
        /// </summary>
        ApplicationPID,
        /// <summary>
        /// For clients/streams: application process name. e.g. "totem"
        /// </summary>
        ApplicationProcessBinary,
        /// <summary>
        /// For clients/streams: application user name. e.g. "lennart"
        /// </summary>
        ApplicationProcessUser,
        /// <summary>
        /// For clients/streams: host name the application runs on. e.g. "omega"
        /// </summary>
        ApplicationProcessHost,
        /// <summary>
        /// For clients/streams: the D-Bus host id the application runs on. e.g. "543679e7b01393ed3e3e650047d78f6e"
        /// </summary>
        ApplicationProcessMachineID,
        /// <summary>
        /// For clients/streams: an id for the login session the application runs in. On Unix the value of $XDG_SESSION_COOKIE. e.g. "543679e7b01393ed3e3e650047d78f6e-1235159798.76193-190367717"
        /// </summary>
        ApplicationProcessSessionID,
        /// <summary>
        /// For devices: device string in the underlying audio layer's format. e.g. "surround51:0"
        /// </summary>
        DeviceString,
        /// <summary>
        /// For devices: API this device is access with. e.g. "alsa"
        /// </summary>
        DeviceAPI,
        /// <summary>
        /// For devices: localized human readable device one-line description, e.g. "Foobar Industries USB Headset 2000+ Ultra"
        /// </summary>
        DeviceDescription,
        /// <summary>
        /// For devices: bus path to the device in the OS' format. e.g. "/sys/bus/pci/devices/0000:00:1f.2"
        /// </summary>
        DeviceBusPath,
        /// <summary>
        /// For devices: serial number if applicable. e.g. "4711-0815-1234"
        /// </summary>
        DeviceSerial,
        /// <summary>
        /// For devices: vendor ID if applicable. e.g. 1274
        /// </summary>
        DeviceVendorID,
        /// <summary>
        /// For devices: vendor name if applicable. e.g. "Foocorp Heavy Industries"
        /// </summary>
        DeviceVendorName,
        /// <summary>
        /// For devices: product ID if applicable. e.g. 4565
        /// </summary>
        DeviceProductID,
        /// <summary>
        /// For devices: product name if applicable. e.g. "SuperSpeakers 2000 Pro"
        /// </summary>
        DeviceProductName,
        /// <summary>
        /// For devices: device class. One of "sound", "modem", "monitor", "filter"
        /// </summary>
        DeviceClass,
        /// <summary>
        /// For devices: form factor if applicable. One of "internal", "speaker", "handset", "tv", "webcam", "microphone", "headset", "headphone", "hands-free", "car", "hifi", "computer", "portable"
        /// </summary>
        DeviceFormFactor,
        /// <summary>
        /// For devices: bus of the device if applicable. One of "isa", "pci", "usb", "firewire", "bluetooth"
        /// </summary>
        DeviceBus,
        /// <summary>
        /// For devices: an XDG icon name for the device. e.g. "sound-card-speakers-usb"
        /// </summary>
        DeviceIconName,
        /// <summary>
        /// For devices: access mode of the device if applicable. One of "mmap", "mmap_rewrite", "serial"
        /// </summary>
        DeviceAccessMode,
        /// <summary>
        /// For filter devices: master device id if applicable.
        /// </summary>
        DeviceMasterDevice,
        /// <summary>
        /// For devices: buffer size in bytes, integer formatted as string.
        /// </summary>
        DeviceBufferSize,
        /// <summary>
        /// For devices: fragment size in bytes, integer formatted as string.
        /// </summary>
        DeviceFragmentSize,
        /// <summary>
        /// For devices: profile identifier for the profile this devices is in. e.g. "analog-stereo", "analog-surround-40", "iec958-stereo", ...
        /// </summary>
        DeviceProfileName,
        /// <summary>
        /// For devices: human readable one-line description of the profile this device is in. e.g. "Analog Stereo", ...
        /// </summary>
        DeviceProfileDescription,
        /// <summary>
        /// For devices: intended use. A comma seperated list of roles (see PA_PROP_MEDIA_ROLE) this device is particularly well suited for, due to latency, quality or form factor. \since 0.9.16
        /// </summary>
        DeviceIntendedRoles,
        /// <summary>
        /// For modules: the author's name, formatted as UTF-8 string. e.g. "Lennart Poettering"
        /// </summary>
        ModuleAuthor,
        /// <summary>
        /// For modules: a human readable one-line description of the module's purpose formatted as UTF-8. e.g. "Frobnicate sounds with a flux compensator"
        /// </summary>
        ModuleDescription,
        /// <summary>
        /// For modules: a human readable usage description of the module's arguments formatted as UTF-8
        /// </summary>
        ModuleUsage,
        /// <summary>
        /// For modules: a version string for the module. e.g. "0.9.15"
        /// </summary>
        ModuleVersion
    }

    public class PropList : IDisposable
    {
        private static readonly Dictionary<Properties, string> propertyTable = new Dictionary<Properties, string> {
            {Properties.MediaName, "media.name"},
            {Properties.MediaTitle, "media.title"},
            {Properties.MediaArtist, "media.artist"},
            {Properties.MediaCopyright, "media.copyright"},
            {Properties.MediaSoftware, "media.software"},
            {Properties.MediaLanguage, "media.language"},
            {Properties.MediaFilename, "media.filename"},
            {Properties.MediaIconName, "media.icon_name"},
            {Properties.MediaRole, "media.role"},
            {Properties.EventID, "event.id"},
            {Properties.EventDescription, "event.description"},
            {Properties.EventMouseX, "event.mouse.x"},
            {Properties.EventMouseY, "event.mouse.y"},
            {Properties.EventMouseHPos, "event.mouse.hpos"},
            {Properties.EventMouseVPos, "event.mouse.vpos"},
            {Properties.EventMouseButton, "event.mouse.button"},
            {Properties.WindowName, "window.name"},
            {Properties.WindowID, "window.id"},
            {Properties.WindowIconName, "window.icon_name"},
            {Properties.WindowX, "window.x"},
            {Properties.WindowY, "window.y"},
            {Properties.WindowWidth, "window.width"},
            {Properties.WindowHeight, "window.height"},
            {Properties.WindowHPos, "window.hpos"},
            {Properties.WindowVPos, "window.vpos"},
            {Properties.WindowDesktop, "window.desktop"},
            {Properties.WindowX11Display, "window.x11.display"},
            {Properties.WindowX11Screen, "window.x11.screen"},
            {Properties.WindowX11Monitor, "window.x11.monitor"},
            {Properties.WindowX11XID, "window.x11.xid"},
            {Properties.ApplicationName, "application.name"},
            {Properties.ApplicationID, "application.id"},
            {Properties.ApplicationVersion, "application.version"},
            {Properties.ApplicationIconName, "application.icon_name"},
            {Properties.ApplicationLanguage, "application.language"},
            {Properties.ApplicationPID, "application.process.id"},
            {Properties.ApplicationProcessBinary, "application.process.binary"},
            {Properties.ApplicationProcessUser, "application.process.user"},
            {Properties.ApplicationProcessHost, "application.process.host"},
            {Properties.ApplicationProcessMachineID, "application.process.machine_id"},
            {Properties.ApplicationProcessSessionID, "application.process.session_id"},
            {Properties.DeviceString, "device.string"},
            {Properties.DeviceAPI, "device.api"},
            {Properties.DeviceDescription, "device.description"},
            {Properties.DeviceBusPath, "device.bus_path"},
            {Properties.DeviceSerial, "device.serial"},
            {Properties.DeviceVendorID, "device.vendor.id"},
            {Properties.DeviceVendorName, "device.vendor.name"},
            {Properties.DeviceProductID, "device.product.id"},
            {Properties.DeviceProductName, "device.product.name"},
            {Properties.DeviceClass, "device.class"},
            {Properties.DeviceFormFactor, "device.form_factor"},
            {Properties.DeviceBus, "device.bus"},
            {Properties.DeviceIconName, "device.icon_name"},
            {Properties.DeviceAccessMode, "device.access_mode"},
            {Properties.DeviceMasterDevice, "device.master_device"},
            {Properties.DeviceBufferSize, "device.buffering.buffer_size"},
            {Properties.DeviceFragmentSize, "device.buffering.fragment_size"},
            {Properties.DeviceProfileName, "device.profile.name"},
            {Properties.DeviceProfileDescription, "device.profile.description"},
            {Properties.DeviceIntendedRoles, "device.intended_roles"},
            {Properties.ModuleAuthor, "module.author"},
            {Properties.ModuleDescription, "module.description"},
            {Properties.ModuleUsage, "module.version"},
            {Properties.ModuleVersion, "module.version"}
        };
        private HandleRef handle;
        private bool needDispose;

        public PropList ()
        {
            IntPtr listPtr = pa_proplist_new ();
            if (listPtr == IntPtr.Zero) {
                throw new NullReferenceException ();
            }
            handle = new HandleRef (this, listPtr);
            needDispose = true;
        }

        /// <summary>
        /// Internal-only PropList constructor taking an unmanaged pointer.
        /// </summary>
        /// <param name="listPtr">
        /// A <see cref="IntPtr"/>.  Unmanaged pointer to a pa_proplist.
        /// </param>
        /// <remarks>
        /// It is assumed that native code owns the pa_proplist pointer.  As such, it will not be free'd on Dispose ().
        /// If the managed code should own this pointer, set needDispose to true after calling this constructor.
        /// </remarks>
        internal PropList (IntPtr listPtr)
        {
            if (listPtr == IntPtr.Zero) {
                throw new NullReferenceException ();
            }
            handle = new HandleRef (this, listPtr);
            needDispose = false;
            GC.SuppressFinalize (this);
        }

        ~PropList ()
        {
            if (needDispose) {
                pa_proplist_free (handle);
            }
        }

        public void Dispose ()
        {
            if (needDispose) {
                pa_proplist_free (handle);
            }
            handle = new HandleRef (this, IntPtr.Zero);
            needDispose = false;

            GC.SuppressFinalize (this);
        }

        public byte[] this[string key]
        {
            get {
                IntPtr data = new IntPtr ();
                IntPtr size = new IntPtr ();
                pa_proplist_get (handle, key, ref data, ref size);

                byte[] retVal = new byte[size.ToInt32 ()];
                Marshal.Copy (data, retVal, 0, size.ToInt32 ());

                return retVal;
            }
            set {
                if (value == null) {
                    throw new ArgumentNullException ("value", "Setting a PropList key to 'null' is unsupported");
                }
                pa_proplist_set (handle, key, value, new IntPtr (Marshal.SizeOf (value[0]) * value.Length));
            }
        }

        public string this[Properties prop]
        {
            get {
                return Marshal.PtrToStringAnsi (pa_proplist_gets (handle, propertyTable[prop]));
            }
            set {
                pa_proplist_sets (handle, propertyTable[prop], value);
            }
        }

        public bool Empty {
            get {
                // TODO: Work out why pa_proplist_isempty doesn't return 0 when the list is empty.
                return (Count == 0);
            }
        }

        public uint Count {
            get {
                return pa_proplist_size (handle);
            }
        }

        public void Clear ()
        {
            pa_proplist_clear (handle);
        }

        public PropList Copy ()
        {
            PropList clone = new PropList (pa_proplist_copy (handle));
            clone.needDispose = true;
            GC.ReRegisterForFinalize (clone);
            return clone;
        }

        public IEnumerable<string> Keys {
            get {
                IntPtr cookie = new IntPtr (0);
                IntPtr key;
                key = pa_proplist_iterate (handle, ref cookie);
                while (key != IntPtr.Zero) {
                    yield return Marshal.PtrToStringAnsi (key);
                    key = pa_proplist_iterate (handle, ref cookie);
                }
                yield break;
            }
        }

        [DllImport ("pulse")]
        private static extern int pa_proplist_isempty (HandleRef list);
        [DllImport ("pulse")]
        private static extern IntPtr pa_proplist_new ();
        [DllImport ("pulse")]
        private static extern void pa_proplist_clear (HandleRef list);
        [DllImport ("pulse")]
        private static extern uint pa_proplist_size (HandleRef list);
        [DllImport ("pulse")]
        private static extern int pa_proplist_sets (HandleRef list, string key, string value);
        [DllImport ("pulse")]
        private static extern int pa_proplist_set (HandleRef list, string key, byte[] data, IntPtr size);
        [DllImport ("pulse")]
        private static extern IntPtr pa_proplist_gets (HandleRef list, string key);
        [DllImport ("pulse")]
        private static extern int pa_proplist_get (HandleRef list, string key, ref IntPtr data, ref IntPtr size);
        [DllImport ("pulse")]
        private static extern IntPtr pa_proplist_iterate (HandleRef list, ref IntPtr cookie);
        [DllImport ("pulse")]
        private static extern IntPtr pa_proplist_copy (HandleRef list);
        [DllImport ("pulse")]
        private static extern void pa_proplist_free (HandleRef list);
    }
}
