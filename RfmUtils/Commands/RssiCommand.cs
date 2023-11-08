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

using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using RfmUsb.Net;
using System;

namespace RfmUtils.Commands
{
    [Command(Description = "Listen for device transmissions and capture the rssi level")]
    internal class RssiCommand : BaseRadioCommand
    {
        private readonly ILogger<RssiCommand> _logger;

        public RssiCommand(ILogger<RssiCommand> logger, IRfm69 rfm69) : base(rfm69)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override int OnExecute(CommandLineApplication app, IConsole console)
        {
            int result = -1;
            try
            {
                _logger.LogInformation("Waiting for rssi updates. Press ctrl + c to quit");

                InitaliseRadioOpenThings(SerialPort, BaudRate);

                AttachEventHandlers(console);

                Rfm69.SetDioMapping(Dio.Dio0, DioMapping.DioMapping3);
                Rfm69.DioInterruptMask = DioIrq.Dio0;
                
                SignalSource signalSource = SignalSource.None;

                do
                {
                    Rfm69.Mode = Mode.Rx;

                    signalSource = WaitForSignal();

                    if (signalSource == SignalSource.Irq)
                    {
                        _logger.LogDebug("Radio irq: [{IrqFlags}]", Rfm69.IrqFlags);

                        if ((Rfm69.IrqFlags & Rfm69IrqFlags.Rssi) == Rfm69IrqFlags.Rssi)
                        {
                            _logger.LogInformation("Rssi: [{rssi}]", Rfm69.Rssi);

                            Rfm69.Mode = Mode.Standby;
                        }
                    }
                    else if (signalSource == SignalSource.Stop)
                    {
                        _logger.LogInformation("Finished listening for messages.");
                    }
                } while (signalSource != SignalSource.Stop);

                result = 0;
            }
            finally
            {
                DettachEventHandlers(console);
            }

            return result;
        }
    }
}
