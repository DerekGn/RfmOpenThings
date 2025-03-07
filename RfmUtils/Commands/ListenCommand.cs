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

// Ignore Spelling: Utils Rfm app ctrl

using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenThings;
using RfmUsb.Net;
using RfmUtils.Services;
using System;
using System.Linq;

namespace RfmUtils.Commands
{
    [Command(Description = "Listen for openthings sensor messages")]
    internal class ListenCommand : BaseRadioRxCommand
    {
        private readonly ISensorStore _deviceStore;

        public ListenCommand(
           IOpenThingsDecoder openThingsDecoder,
           ILogger<ListenCommand> logger,
           IConfiguration configuration,
           ISensorStore deviceStore,
           IParameters parameters,
           IRfm69 rfm69) 
            : base(openThingsDecoder, logger, configuration, parameters, rfm69)
        {
            _deviceStore = deviceStore ?? throw new ArgumentNullException(nameof(deviceStore));
        }

        protected override int OnExecute(CommandLineApplication app, IConsole console)
        {
            return ExecuteRxCommand(console, (message) =>
            {
                var sensors = _deviceStore.ReadSensors().ToList();

                if (!sensors.Any(s => s.SensorId == message.Header.SensorId))
                {
                    Logger.LogInformation("New sensor detected [0x{SensorId:X}]", message.Header.SensorId);

                    sensors.Add(new Sensor(
                        message.Header.SensorId,
                        message.Header.ProductId,
                        (Manufacturer)message.Header.ManufacturerId,
                        DateTime.UtcNow));
                }
                else
                {
                    Logger.LogInformation("Existing sensor detected [0x{SensorId:X}]", message.Header.SensorId);
                }

                _deviceStore.WriteSensors(sensors);

                return 0;
            });
        }
    }
}