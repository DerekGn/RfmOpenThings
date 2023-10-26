/*
* MIT License
*
* Copyright (c) 2021 Derek Goslin http://corememorydump.blogspot.ie/
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/

// Ignore Spelling: Iaq TVOC

using OpenThings;

namespace RfmUtils.Services
{
    internal class ExtendedParameterIdentifiers : ParameterIdentifiers
    {
        /// <summary>
        /// The devices battery voltage
        /// </summary>
        public const byte BatteryVoltage = 0x7D;

        /// <summary>
        /// The Indoor air quality
        /// </summary>
        public const byte Iaq = 0x7E;

        /// <summary>
        /// The estimated carbon dioxide level
        /// </summary>
        public const byte eCo2 = 0x7F;

        /// <summary>
        /// The equivalent ethanol level
        /// </summary>
        public const byte EtOH = 0x80;

        /// <summary>
        /// The total volatile organic compounds
        /// </summary>
        public const byte TVOC = 0x81;
    }
}
