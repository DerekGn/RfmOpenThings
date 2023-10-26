/*
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

// Ignore Spelling: Utils Rfm app

using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using OpenThings;
using RfmUsb.Net;
using System.ComponentModel.DataAnnotations;

namespace RfmUtils.Commands
{
    [Command(Description = "Send Interval command to a sensor")]
    internal class IntervalCommand : BaseRadioRxTxCommand
    {
        public IntervalCommand(
            IOpenThingsDecoder openThingsDecoder,
            IOpenThingsEncoder openThingsEncoder,
            IConfiguration configuration,
            IParameters parameters,
            IRfm69 rfm69)
            : base(openThingsDecoder, openThingsEncoder, configuration, parameters, rfm69)
        {
        }

        [Required]
        [Option(Templates.Interval, "The sensor update interval", CommandOptionType.SingleValue)]
        public uint? Interval { get; set; }

        protected override int OnExecute(CommandLineApplication app, IConsole console)
        {
            OperationResult result = OperationResult.Continue;

            return ExecuteRxTxCommand(console, (message) =>
            {
                if (message.Header.SensorId == SensorId && Interval.HasValue)
                {
                    console.WriteLine("Sending Interval Message");

                    Rfm69.Transmit(
                        EncodeMessage(
                            CreateMessage(
                                message.Header,
                                Parameters.GetParameter(DefaultCommands.ReportPeriodCommand),
                                new MessageRecordDataUInt(Interval.Value))));

                    return OperationResult.Complete;
                }

                return result;
            });
        }
    }
}