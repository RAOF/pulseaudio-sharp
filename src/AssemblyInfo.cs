//
//  AssemblyInfo.cs
//
//  Author:
//       Christopher James Halse Rogers <raof@ubuntu.com>
//
//  Copyright © 2012 Christopher James Halse Rogers <raof@ubuntu.com>
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

using System.Reflection;
using System.Runtime.CompilerServices;

// Information about this assembly is defined by the following attributes. 
// Change them to the values specific to your project.

[assembly: AssemblyTitle("PulseAudio#")]
[assembly: AssemblyDescription("CLI Bindings for the PulseAudio sound server")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("cooperteam.net")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// The assembly version has the format "{Major}.{Minor}.{Build}.{Revision}".
// If the build and revision are set to '*' they will be updated automatically.

[assembly: AssemblyVersion("0.1.0.0")]

// The following attributes are used to specify the signing key for the assembly, 
// if desired. See the Mono documentation for more information about signing.

[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]

// Allow our Test assembly access...
[assembly:InternalsVisibleTo("Test")]
