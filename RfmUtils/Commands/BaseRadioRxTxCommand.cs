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

// Ignore Spelling: Utils Rfm pip ctrl

using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenThings;
using RfmUsb.Net;
using RfmUtils.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RfmUtils.Commands
{
    internal abstract class BaseRadioRxTxCommand : BaseRadioRxCommand
    {
        protected readonly IOpenThingsEncoder? OpenThingsEncoder;
        protected readonly List<PipMap>? PipMap;

        private readonly Random _random;

        public BaseRadioRxTxCommand(
            IOpenThingsDecoder openThingsDecoder,
            IOpenThingsEncoder openThingsEncoder,
            ILogger<BaseRadioRxTxCommand> logger,
            IConfiguration configuration,
            IParameters parameters,
            IRfm69 rfm69)
            : base(openThingsDecoder, logger, configuration, parameters, rfm69)
        {
            OpenThingsEncoder = openThingsEncoder ?? throw new ArgumentNullException(nameof(openThingsEncoder));

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            PipMap = configuration.GetSection(nameof(PipMap)).Get<List<PipMap>>();

            _random = new Random();
        }

        [Required]
        [Option(Templates.SensorId, "The OpenThings sensor id", CommandOptionType.SingleValue)]
        public uint? SensorId { get; set; }

        internal static Message CreateMessage(
            MessageHeader messageHeader,
            Parameter parameter,
            BaseMessageRecordData messageData)
        {
            var identifyHeader = new MessageHeader(
                        messageHeader.ManufacturerId,
                        messageHeader.ProductId,
                        0,
                        messageHeader.SensorId);

            var message = new Message(identifyHeader);

            var record = new MessageRecord(parameter, messageData);
            message.Records.Add(record);

            return message;
        }

        internal IList<byte> EncodeMessage(Message requestMessage)
        {
            var pip = PipMap?.FirstOrDefault(_ => _.ManufacturerId == requestMessage.Header.ManufacturerId);

            List<byte> encodedMessage;

            if (pip == null)
            {
                encodedMessage = OpenThingsEncoder.Encode(requestMessage, pip.Pip, (ushort)_random.Next(ushort.MaxValue)).ToList();
            }
            else
            {
                encodedMessage = OpenThingsEncoder.Encode(requestMessage).ToList();
            }

            return encodedMessage;
        }

        internal int ExecuteRxTxCommand(IConsole console, Func<Message, OperationResult> action)
        {
            OperationResult operationResult = OperationResult.Continue;
            int result = -1;

            try
            {
                Logger.LogInformation("Waiting for sensor messages. Press ctrl + c to quit");

                InitaliseRadio(SerialPort, BaudRate);

                Logger.LogInformation("Initialized Radio");

                AttachEventHandlers(console);

                Rfm69.SetDioMapping(Dio.Dio0, DioMapping.DioMapping1);
                Rfm69.DioInterruptMask = DioIrq.Dio0;
                Rfm69.Mode = Mode.Rx;

                do
                {
                    var source = WaitForSignal();

                    if (source == SignalSource.Irq)
                    {
                        if ((Rfm69.IrqFlags & Rfm69IrqFlags.PayloadReady) == Rfm69IrqFlags.PayloadReady)
                        {
                            Rfm69.Mode = Mode.Standby;

                            try
                            {
                                var payload = Rfm69.Fifo.ToList();

                                //console.WriteLine("Received Packet: [{message}]",
                                //    BitConverter.ToString(payload.ToArray()));

                                Message message = OpenThingsDecoder.Decode(payload, PidMap);

                                Logger.LogInformation("Message Decoded: {message}", message);

                                operationResult = action(message);
                            }
                            catch (Exception ex)
                            {
                                console.Error.WriteLine("Decoding OpenThings Message Failed. [{message}]",
                                    ex.Message);
                            }

                            Rfm69.Mode = Mode.Rx;
                        }
                    }
                    else if (source == SignalSource.Stop)
                    {
                        Logger.LogInformation("Finished listening for messages.");
                        break;
                    }
                } while (operationResult == OperationResult.Continue);

                if (operationResult == OperationResult.Complete)
                {
                    result = 0;
                }
            }
            finally
            {
                DettachEventHandlers(console);

                Rfm69.DioInterruptMask = DioIrq.None;
                Rfm69.Mode = Mode.Sleep;
                Rfm69?.Close();
                Rfm69?.Dispose();
            }

            return result;
        }
    }
}