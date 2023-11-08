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

// Ignore Spelling: Utils Rfm app ota

using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenThings;
using RfmUsb.Net;
using RfmUtils.Services;

namespace RfmUtils.Commands
{
    [Command(Name = "bootloader", Description = "Enter the OTA bootloader")]
    internal class OtaEnterCommand : BaseRadioRxTxCommand
    {
        public OtaEnterCommand(
            IOpenThingsDecoder openThingsDecoder,
            IOpenThingsEncoder openThingsEncoder,
            IConfiguration configuration,
            ILogger<OtaEnterCommand> logger,
            IParameters parameters,
            IRfm69 rfm69)
            : base(openThingsDecoder, openThingsEncoder, logger, configuration, parameters, rfm69)
        {
        }

        protected override int OnExecute(CommandLineApplication app, IConsole console)
        {
            return ExecuteRxTxCommand(console, (message) =>
            {
                OperationResult result = OperationResult.Continue;

                if (message.Header.SensorId == SensorId)
                {
                    Logger.LogInformation("Sending execute boot loader message");

                    Rfm69.Transmit(
                        EncodeMessage(
                            CreateMessage(
                                message.Header,
                                Parameters.GetParameter(ExtendedCommands.ExecuteBootLoaderCommand),
                                new MessageRecordDataNull())));

                    result = OperationResult.Complete;
                }

                return result;
            });
        }
    }
}