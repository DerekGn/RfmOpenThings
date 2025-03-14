﻿/*
* MIT License
*
* Copyright (c) 2023 Derek Goslin http://corememorydump.blogspot.ie/
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

using OpenThings;

namespace RfmUtils.Services
{
    internal class ExtendedParameters : DefaultParameters
    {
        public ExtendedParameters()
        {
            Add(new Parameter(
                ExtendedParameterIdentifiers.BatteryVoltage,
                nameof(ExtendedParameterIdentifiers.BatteryVoltage), "V"));

            Add(new Parameter(
                ExtendedParameterIdentifiers.Iaq,
                nameof(ExtendedParameterIdentifiers.Iaq), string.Empty));

            Add(new Parameter(
                ExtendedParameterIdentifiers.eCo2,
                nameof(ExtendedParameterIdentifiers.eCo2), "ppm"));

            Add(new Parameter(
                ExtendedParameterIdentifiers.EtOH,
                nameof(ExtendedParameterIdentifiers.EtOH), "ppm"));

            Add(new Parameter(
                ExtendedParameterIdentifiers.TVOC,
                nameof(ExtendedParameterIdentifiers.TVOC), "mg/m^3"));

            Add(new Parameter(
                ExtendedParameterIdentifiers.CurrentLive,
                nameof(ExtendedParameterIdentifiers.CurrentLive), "A"));

            Add(new Parameter(
                ExtendedParameterIdentifiers.PhaseAngleLive,
                nameof(ExtendedParameterIdentifiers.PhaseAngleLive), "°"));

            Add(new Parameter(
                ExtendedParameterIdentifiers.ActivePowerLive,
                nameof(ExtendedParameterIdentifiers.ActivePowerLive), "kW"));

            Add(new Parameter(
                ExtendedParameterIdentifiers.PowerFactorLive,
                nameof(ExtendedParameterIdentifiers.PowerFactorLive), string.Empty));

            Add(new Parameter(
                ExtendedParameterIdentifiers.ReactivePowerLive,
                nameof(ExtendedParameterIdentifiers.ReactivePowerLive), "kvar"));

            Add(new Parameter(
                ExtendedParameterIdentifiers.ApparentPowerLive,
                nameof(ExtendedParameterIdentifiers.ApparentPowerLive), "kVA"));

            Add(new Parameter(
                ExtendedParameterIdentifiers.CurrentNeutral,
                nameof(ExtendedParameterIdentifiers.CurrentNeutral), "A"));

            Add(new Parameter(
                ExtendedParameterIdentifiers.PhaseAngleNeutral,
                nameof(ExtendedParameterIdentifiers.PhaseAngleNeutral), "°"));

            Add(new Parameter(
                ExtendedParameterIdentifiers.ActivePowerNeutral,
                nameof(ExtendedParameterIdentifiers.ActivePowerNeutral), "kW"));

            Add(new Parameter(
                ExtendedParameterIdentifiers.PowerFactorNeutral,
                nameof(ExtendedParameterIdentifiers.PowerFactorNeutral), string.Empty));

            Add(new Parameter(
                ExtendedParameterIdentifiers.ReactivePowerNeutral,
                nameof(ExtendedParameterIdentifiers.ReactivePowerNeutral), "kvar"));

            Add(new Parameter(
                ExtendedParameterIdentifiers.ApparentPowerNeutral,
                nameof(ExtendedParameterIdentifiers.ApparentPowerNeutral), "kVA"));

            Add(new Parameter(
                ExtendedParameterIdentifiers.AbsoluteActiveEnergy,
                nameof(ExtendedParameterIdentifiers.AbsoluteActiveEnergy), "kW"));

            Add(new Parameter(
                ExtendedParameterIdentifiers.AbsoluteReactiveEnergy,
                nameof(ExtendedParameterIdentifiers.ReverseReactiveEnergy), "kW"));

            Add(new Parameter(
                ExtendedParameterIdentifiers.ForwardActiveEnergy,
                nameof(ExtendedParameterIdentifiers.ForwardActiveEnergy), "kW"));

            Add(new Parameter(
                ExtendedParameterIdentifiers.ForwardReactiveEnergy,
                nameof(ExtendedParameterIdentifiers.ForwardReactiveEnergy), "kW"));

            Add(new Parameter(
                ExtendedParameterIdentifiers.ReverseActiveEnergy,
                nameof(ExtendedParameterIdentifiers.ReverseActiveEnergy), "kW"));

            Add(new Parameter(
                ExtendedParameterIdentifiers.ReverseReactiveEnergy,
                nameof(ExtendedParameterIdentifiers.ReverseReactiveEnergy), "kW"));

            Add(new Parameter(
                ExtendedCommands.ExecuteBootLoaderCommand,
                nameof(ExtendedCommands.ExecuteBootLoaderCommand), string.Empty));
        }
    }
}