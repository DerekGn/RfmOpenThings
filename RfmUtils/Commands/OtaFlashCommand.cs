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

// Ignore Spelling: ota rfm utils app crc

using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using RfmOta;
using RfmUsb.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace RfmUtils.Commands
{
    [Command(Name = "flash", Description = "Flash the device OTA")]
    internal class OtaFlashCommand : BaseRadioCommand
    {
        private readonly ILogger<OtaFlashCommand> _logger;
        private readonly IOtaService _otaService;

        public OtaFlashCommand(
            ILogger<OtaFlashCommand> logger,
            IOtaService otaService,
            IRfm69 rfm69) : base(rfm69)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _otaService = otaService ?? throw new ArgumentNullException(nameof(otaService));
        }

        [Required]
        [Option(Templates.HexFile, "The hex file to flash to the device", CommandOptionType.SingleValue)]
        public string HexFile { get; set; }

        protected override int OnExecute(CommandLineApplication app, IConsole console)
        {
            int result = -1;

            try
            {
                _logger.LogInformation("Flashing device");

                if (!File.Exists(HexFile))
                {
                    _logger.LogWarning("Hex file [{file}] not found", HexFile);
                }
                else
                {
                    using var stream = File.OpenRead(HexFile);

                    //ReconfigureRadio(
                    //    Rfm69,
                    //    BaudRate,
                    //    Frequency,
                    //    SerialPort,
                    //    OutputPower,
                    //    RssiThreshold);

                    InitialiseRadioOpenThings(SerialPort, BaudRate);

                    if (_otaService.OtaUpdate(2, stream, out uint crc))
                    {
                        _logger.LogInformation("OTA flash update completed. Crc: [0x{crc:X}]", crc);
                        result = 0;
                    }
                    else
                    {
                        _logger.LogWarning("OTA flash update failed");
                        result = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
            }
            finally
            {
                Rfm69?.Close();
                Rfm69?.Dispose();
            }

            return result;
        }

        private static void ReconfigureRadio(
            IRfm69 rfm69,
            int baudRate,
            uint frequency,
            string serialPort,
            sbyte outputPower,
            sbyte rssiThreshold)
        {
            rfm69.Open(serialPort, baudRate);
            rfm69.ExecuteReset();

            rfm69.RxBw = 9;
            rfm69.RxBwAfc = 9;
            rfm69.CrcOn = true;
            rfm69.Timeout = 5000;
            rfm69.BitRate = 4800;
            rfm69.PacketFormat = true;
            rfm69.Frequency = frequency;
            rfm69.TxStartCondition = true;
            rfm69.OutputPower = outputPower;
            rfm69.FrequencyDeviation = 0x01EC;
            rfm69.RssiThreshold = rssiThreshold;
            rfm69.DccFreq = DccFreq.FreqPercent0_125;
            rfm69.ModulationType = ModulationType.Ook;
            rfm69.DccFreqAfc = DccFreq.FreqPercent0_125;
            rfm69.Sync = new List<byte>() { 0x55, 0x55 };
            rfm69.DcFreeEncoding = DcFreeEncoding.Whitening;
            rfm69.FskModulationShaping = FskModulationShaping.None;
            
            // Enable DIO3 to capture rssi
            rfm69.SetDioMapping(Dio.Dio3, DioMapping.DioMapping1);
            rfm69.DioInterruptMask = DioIrq.Dio3;
        }
    }
}