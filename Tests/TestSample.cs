//  
//  Copyright © 2009 Christopher James Halse Rogers <raof@ubuntu.com>
// 
//  TestSample.cs is a part of Pulseaudio#
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
using NUnit.Framework;

namespace Pulseaudio
{
    
    
    [TestFixture()]
    public class TestSample
    {
        
        [Test()]
        public void BytesPerSecondForStereo44100HzS16 ()
        {
            SampleSpec spec = new SampleSpec ();
            spec.channels = 2;
            spec.format = SampleFormat.S16LE;
            spec.rate = 44100;
            Assert.AreEqual (44100 * 2 * 2, spec.BytesPerSecond);
        }

        [Test()]
        public void BytesPerSecondForMono44100HzS16 ()
        {
            SampleSpec spec = new SampleSpec ();
            spec.channels = 1;
            spec.format = SampleFormat.S16LE;
            spec.rate = 44100;
            Assert.AreEqual (44100 * 1 * 2, spec.BytesPerSecond);
        }

        [Test()]
        public void BytesPerSecondForStereo48000HzFloat32 ()
        {
            SampleSpec spec = new SampleSpec ()
            { 
                channels = 2,
                format = SampleFormat.Float32LE,
                rate = 48000
            };
            Assert.AreEqual (48000 * 4 * 2, spec.BytesPerSecond);
        }  
    }
}
