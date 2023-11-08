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

// Ignore Spelling: Utils Rfm pid rssi

using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenThings;
using OpenThings.Exceptions;
using RfmUsb.Net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RfmUtils.Commands
{
    internal abstract class BaseRadioRxCommand : BaseRadioCommand
    {
        internal readonly ILogger<BaseRadioRxCommand> Logger;
        internal readonly IOpenThingsDecoder OpenThingsDecoder;
        internal readonly IParameters Parameters;
        protected readonly List<PidMap>? PidMap;
        private readonly IRfm69 _rfm69;

        public BaseRadioRxCommand(
            IOpenThingsDecoder openThingsDecoder,
            ILogger<BaseRadioRxCommand> logger,
            IConfiguration configuration,
            IParameters parameters,
            IRfm69 rfm69) : base(rfm69)
        {
            OpenThingsDecoder = openThingsDecoder ?? throw new ArgumentNullException(nameof(openThingsDecoder));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _rfm69 = rfm69 ?? throw new ArgumentNullException(nameof(rfm69));

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            PidMap = configuration.GetSection(nameof(PidMap)).Get<List<PidMap>>();
        }

        internal int ExecuteRxCommand(IConsole console, Func<Message, int> action)
        {
            int result = -1;

            try
            {
                Logger.LogInformation("Waiting for sensor messages. Press ctrl + c to quit");

                InitaliseRadioOpenThings(SerialPort, BaudRate);
                Logger.LogInformation("Radio Initialized");

                AttachEventHandlers(console);

                // Enable DIO3 to capture rssi
                _rfm69.SetDioMapping(Dio.Dio0, DioMapping.DioMapping1);
                _rfm69.SetDioMapping(Dio.Dio3, DioMapping.DioMapping1);
                _rfm69.DioInterruptMask = DioIrq.Dio0 | DioIrq.Dio3;
                _rfm69.Mode = Mode.Rx;

                SignalSource signalSource = SignalSource.None;

                Logger.LogInformation("Listening for device packets");

                do
                {
                    signalSource = WaitForSignal();

                    if (signalSource == SignalSource.Irq)
                    {
                        Logger.LogDebug("Radio irq: [{IrqFlags}]", _rfm69.IrqFlags);

                        if ((_rfm69.IrqFlags & Rfm69IrqFlags.PayloadReady) == Rfm69IrqFlags.PayloadReady)
                        {
                            Logger.LogInformation("Processing received packet");

                            _rfm69.Mode = Mode.Standby;

                            try
                            {
                                Logger.LogInformation("Message received Rssi: [{LastRssi}]", Rfm69.LastRssi);

                                result = action(OpenThingsDecoder.Decode(_rfm69.Fifo.ToList(), PidMap));
                            }
                            catch (OpenThingsException ex)
                            {
                                Logger.LogError("Decoding OpenThings Message Failed. [{Message}]", ex.Message);
                            }
                            catch(Exception ex)
                            {
                                Logger.LogError("Unhandled exception occurred [{Message}]", ex.Message);
                            }

                            _rfm69.Mode = Mode.Rx;
                        }
                    }
                    else if (signalSource == SignalSource.Stop)
                    {
                        Logger.LogInformation("Finished listening for messages.");
                    }
                } while (signalSource != SignalSource.Stop);
            }
            finally
            {
                DettachEventHandlers(console);

                _rfm69.DioInterruptMask = DioIrq.None;
                _rfm69.Mode = Mode.Sleep;
                _rfm69?.Close();
                _rfm69?.Dispose();
            }

            return result;
        }
    }
}