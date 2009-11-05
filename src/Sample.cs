//  
//  Copyright © 2009 Christopher James Halse Rogers <raof@ubuntu.com>
// 
//  Sample.cs is a part of Pulseaudio#
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
    public enum SampleFormat
    {
        U8,
        /**< Unsigned 8 Bit PCM */
        
        ALAW,
        /**< 8 Bit a-Law */
        
        ULAW,
        /**< 8 Bit mu-Law */
        
        S16LE,
        /**< Signed 16 Bit PCM, little endian (PC) */
        
        S16BE,
        /**< Signed 16 Bit PCM, big endian */
        
        Float32LE,
        /**< 32 Bit IEEE floating point, little endian (PC), range -1.0 to 1.0 */
        
        Float32BE,
        /**< 32 Bit IEEE floating point, big endian, range -1.0 to 1.0 */
        
        S32LE,
        /**< Signed 32 Bit PCM, little endian (PC) */
        
        S32BE,
        /**< Signed 32 Bit PCM, big endian */
        
        S24LE,
        /**< Signed 24 Bit PCM packed, little endian (PC). \since 0.9.15 */

        S24BE,
        /**< Signed 24 Bit PCM packed, big endian. \since 0.9.15 */
        
        S24in32LE,
        /**< Signed 24 Bit PCM in LSB of 32 Bit words, little endian (PC). \since 0.9.15 */
        
        S24in32BE,
        /**< Signed 24 Bit PCM in LSB of 32 Bit words, big endian. \since 0.9.15 */
        
        Max,
        /**< Upper limit of valid sample types */
        
        Invalid = -1
        /**< An invalid value */    
    }
    
    [StructLayout (LayoutKind.Sequential)]
    public struct SampleSpec
    {
        public SampleFormat format;
        public UInt32 rate;
        public Byte channels;

        public UInt64 BytesPerSecond {
            get {
                return (UInt64) pa_bytes_per_second (ref this);
            }
        }

        [DllImport ("pulse")]
        private static extern UIntPtr pa_bytes_per_second (ref SampleSpec spec);
    }
}
