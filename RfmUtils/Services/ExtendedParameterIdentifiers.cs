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
        public const byte BatteryVoltage = 0x01;

        /// <summary>
        /// The Indoor air quality
        /// </summary>
        public const byte Iaq = 0x02;

        /// <summary>
        /// The total volatile organic compounds
        /// </summary>
        public const byte TVOC = 0x03;

        /// <summary>
        /// The equivalent ethanol level
        /// </summary>
        public const byte EtOH = 0x04;

        /// <summary>
        /// The estimated carbon dioxide level
        /// </summary>
        public const byte eCo2 = 0x05;

        /// <summary>
        /// The current of the live phase
        /// </summary>
        public const byte CurrentLive = 0x06;

        /// <summary>
        /// The phase angle for the live phase
        /// </summary>
        public const byte PhaseAngleLive = 0x07;

        /// <summary>
        /// The active power of the live phase
        /// </summary>
        public const byte ActivePowerLive = 0x08;

        /// <summary>
        /// The power factor of the live phase
        /// </summary>
        public const byte PowerFactorLive = 0x09;

        /// <summary>
        /// The reactive power of the live phase
        /// </summary>
        public const byte ReactivePowerLive = 0x0A;

        /// <summary>
        /// The apparent power for the live phase
        /// </summary>
        public const byte ApparentPowerLive = 0x0B;

        /// <summary>
        /// The current of the neutral phase
        /// </summary>
        public const byte CurrentNeutral = 0x0C;

        /// <summary>
        /// The phase angle of the neutral phase
        /// </summary>
        public const byte PhaseAngleNeutral = 0x0D;

        /// <summary>
        /// The active power of the neutral phase
        /// </summary>
        public const byte ActivePowerNeutral = 0x0E;

        /// <summary>
        /// The power factor of the neutral phase
        /// </summary>
        public const byte PowerFactorNeutral = 0x0F;

        /// <summary>
        /// The reactive power of the neutral phase
        /// </summary>
        public const byte ReactivePowerNeutral = 0x10;

        /// <summary>
        /// The apparent power of the neutral phase
        /// </summary>
        public const byte ApparentPowerNeutral = 0x11;

        /// <summary>
        /// The absolute active energy
        /// </summary>
        public const byte AbsoluteActiveEnergy = 0x12;

        /// <summary>
        /// The absolute reactive energy
        /// </summary>
        public const byte AbsoluteReactiveEnergy = 0x13;

        /// <summary>
        /// The forward active energy
        /// </summary>
        public const byte ForwardActiveEnergy = 0x14;

        /// <summary>
        /// The forward reactive energy
        /// </summary>
        public const byte ForwardReactiveEnergy = 0x15;

        /// <summary>
        /// The reverse active energy
        /// </summary>
        public const byte ReverseActiveEnergy = 0x16;

        /// <summary>
        /// The reverse active energy
        /// </summary>
        public const byte ReverseReactiveEnergy = 0x17;
    }
}