//  
//  Copyright © 2009 Christopher James Halse Rogers <raof@ubuntu.com>
// 
//  Volume.cs is a part of Pulseaudio#
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
    public class Volume
    {
        public static readonly Volume Norm = new Volume ();
        
        public Volume()
        {
        }

        public void Reset ()
        {
        }

        public override bool Equals (object obj)
        {
            Volume v = obj as Volume;
            if (v != null) {
                return true;
            }
            return base.Equals (obj);
        }

        public override int GetHashCode ()
        {
            return base.GetHashCode ();
        }

/*
        public static bool operator== (Volume a, Volume b)
        {
            return true;
        }
/*
        public static bool operator!= (Volume a, Volume b)
        {
            return !(a==b);
        }
*/
    }
}
