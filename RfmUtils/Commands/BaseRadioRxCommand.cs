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
using OpenThings;
using RfmUsb.Net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RfmUtils.Commands
{
    internal abstract class BaseRadioRxCommand : BaseRadioCommand
    {
        internal readonly IOpenThingsDecoder OpenThingsDecoder;
        internal readonly IParameters Parameters;
        protected readonly List<PidMap>? PidMap;
        private readonly IRfm69 _rfm69;

        public BaseRadioRxCommand(
            IOpenThingsDecoder openThingsDecoder,
            IConfiguration configuration,
            IParameters parameters,
            IRfm69 rfm69) : base(rfm69)
        {
            OpenThingsDecoder = openThingsDecoder ?? throw new ArgumentNullException(nameof(openThingsDecoder));
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
                InitaliseRadio(SerialPort, BaudRate);

                console.CancelKeyPress += Console_CancelKeyPress;
                _rfm69.DioInterrupt += RfmDeviceDioInterrupt;

                _rfm69.SetDioMapping(Dio.Dio0, DioMapping.DioMapping1);
                _rfm69.SetDioMapping(Dio.Dio3, DioMapping.DioMapping1);
                _rfm69.DioInterruptMask = DioIrq.Dio0 | DioIrq.Dio3;
                _rfm69.Mode = Mode.Rx;

                SignalSource signalSource = SignalSource.None;

                do
                {
                    signalSource = WaitForSignal();

                    if (signalSource == SignalSource.Irq)
                    {
                        console.WriteLine($"Radio irq: [{_rfm69.IrqFlags}]");

                        if ((_rfm69.IrqFlags & Rfm69IrqFlags.PayloadReady) == Rfm69IrqFlags.PayloadReady)
                        {
                            console.WriteLine("Processing received packet");

                            _rfm69.Mode = Mode.Standby;

                            try
                            {
                                console.WriteLine($"Message received Rssi: [{Rfm69.LastRssi}]");

                                result = action(OpenThingsDecoder.Decode(_rfm69.Fifo.ToList(), PidMap));
                            }
                            catch (Exception ex)
                            {
                                console.Error.WriteLine($"Decoding OpenThings Message Failed. [{ex.Message}]");
                            }

                            _rfm69.Mode = Mode.Rx;

                            console.WriteLine("Listening for device messages");
                        }
                    }
                    else if (signalSource == SignalSource.Stop)
                    {
                        console.WriteLine("Finished listening for messages.");
                    }
                } while (signalSource != SignalSource.Stop);
            }
            finally
            {
                _rfm69.Mode = Mode.Sleep;
                console.CancelKeyPress -= Console_CancelKeyPress;
                _rfm69.DioInterrupt -= RfmDeviceDioInterrupt;
                _rfm69.DioInterruptMask = DioIrq.None;
                _rfm69?.Close();
                _rfm69?.Dispose();
            }

            return result;
        }
    }
}