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

// Ignore Spelling: Utils Rfm rssi dbm

using McMaster.Extensions.CommandLineUtils;
using RfmUsb.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace RfmUtils.Commands
{
    internal abstract class BaseRadioCommand : BaseCommand
    {
        protected readonly IRfm69 Rfm69;
        private readonly AutoResetEvent _consoleSignal;
        private readonly AutoResetEvent _irqSignal;
        private readonly AutoResetEvent[] _waitHandles;

        public BaseRadioCommand(IRfm69 rfm69)
        {
            Rfm69 = rfm69 ?? throw new ArgumentNullException(nameof(rfm69));

            _consoleSignal = new AutoResetEvent(false);
            _irqSignal = new AutoResetEvent(false);

            _waitHandles = new List<AutoResetEvent>() { _irqSignal, _consoleSignal }.ToArray();
        }

        [Option(Templates.BaudRate, "The baud rate for the serial port", CommandOptionType.SingleValue)]
        public int BaudRate { get; set; } = 230400;

        [Range(0, 102E+7)]
        [Option(Templates.Frequency, "The radio center frequency", CommandOptionType.SingleValue)]
        public uint Frequency { get; set; } = 433000000;

        [Range(-2, 20)]
        [Option(Templates.OutputPower, "The output power of the RfmUsb in dbm", CommandOptionType.SingleValue)]
        public sbyte OutputPower { get; set; } = 0;

        [Range(-115, 0)]
        [Option(Templates.RssiThreshold, "The RSSI threshold", CommandOptionType.SingleValue)]
        public sbyte RssiThreshold { get; set; } = -50;

        [Required]
        [Option(Templates.SerialPort, "The serial port that an RfmUsb device is connected", CommandOptionType.SingleValue)]
        public required string SerialPort { get; set; }

        internal void AttachEventHandlers(IConsole console)
        {
            console.CancelKeyPress += Console_CancelKeyPress;
            Rfm69.DioInterrupt += RfmDeviceDioInterrupt;
        }

        internal void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            _consoleSignal.Set();
        }

        internal void DettachEventHandlers(IConsole console)
        {
            console.CancelKeyPress -= Console_CancelKeyPress;
            Rfm69.DioInterrupt -= RfmDeviceDioInterrupt;
        }

        internal void InitaliseRadioOpenThings(string serialPort, int baudRate)
        {
            Rfm69.Open(serialPort, baudRate);
            Rfm69.ExecuteReset();
            Rfm69.Timeout = 5000;
            Rfm69.ModulationType = ModulationType.Fsk;
            Rfm69.FrequencyDeviation = 0x01EC;
            Rfm69.Frequency = 434300000;
            Rfm69.RxBw = 14;
            Rfm69.BitRate = 4800;
            Rfm69.Sync = new List<byte>() { 0x2D, 0xD4 };
            Rfm69.SyncSize = 1;
            Rfm69.SyncEnable = true;
            Rfm69.SyncBitErrors = 0;
            Rfm69.PacketFormat = true;
            Rfm69.DcFreeEncoding = DcFreeEncoding.Manchester;
            Rfm69.CrcOn = false;
            Rfm69.CrcAutoClearOff = false;
            Rfm69.AddressFiltering = AddressFilter.None;
            Rfm69.PayloadLength = 66;
            Rfm69.TxStartCondition = true;
            Rfm69.OutputPower = OutputPower;
            Rfm69.RssiThreshold = RssiThreshold;
        }

        internal void RfmDeviceDioInterrupt(object? sender, DioIrq e)
        {
            _irqSignal.Set();
        }

        internal SignalSource WaitForSignal(int timeout = -1)
        {
            return (SignalSource)AutoResetEvent.WaitAny(_waitHandles, timeout);
        }
    }
}