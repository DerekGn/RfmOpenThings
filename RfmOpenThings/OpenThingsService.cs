using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenThings;
using RfmOta;
using RfmUsb;
using RfmUsb.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RfmOpenThings
{
    internal class OpenThingsService : IOpenThingsService
    {
        private readonly IOpenThingsDecoder _openThingsDecoder;
        private readonly IOpenThingsEncoder _openThingsEncoder;
        private readonly ILogger<OpenThingsService> _logger;
        private readonly IOtaService _otaService;
        private readonly List<PidMap> _pidMap;
        private readonly List<PipMap> _pipMap;
        private readonly IRfmUsb _rfmUsb;
        private readonly Random _random;

        private CancellationTokenSource _cancellationTokenSource;
        private Task _task;

        public OpenThingsService(
            ILogger<OpenThingsService> logger,
            IConfiguration configuration,
            IRfmUsb rfmUsb,
            IOtaService otaService,
            IOpenThingsDecoder openThingsDecoder,
            IOpenThingsEncoder openThingsEncoder)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _rfmUsb = rfmUsb ?? throw new ArgumentNullException(nameof(rfmUsb));
            _otaService = otaService ?? throw new ArgumentNullException(nameof(otaService));
            _openThingsDecoder = openThingsDecoder ?? throw new ArgumentNullException(nameof(openThingsDecoder));
            _openThingsEncoder = openThingsEncoder ?? throw new ArgumentNullException(nameof(openThingsEncoder));

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            InitaliseRfmUsb();

            _pidMap = configuration.GetSection(nameof(PidMap)).Get<List<PidMap>>();
            _pipMap = configuration.GetSection(nameof(PipMap)).Get<List<PipMap>>();
            _random = new Random();
        }

        public void StartListen()
        {
            StartOperation((message) =>
            {
                _logger.LogInformation($"Message Decoded {message}");
            });
        }

        public void StartIdentify(uint sensorId)
        {
            StartOperation((message) =>
            {
                if (message.Header.SensorId == sensorId)
                {
                    var pip = _pipMap.FirstOrDefault(_ => _.ManufacturerId == message.Header.ManufacturerId);

                    _logger.LogInformation("Sending Identify Message");

                    TransmitMessage(
                        EncodeMessage(
                            CreateMessage(
                                message.Header,
                                OpenThingsParameter.IdentifyCommand,
                                new MessageRecordDataInt(RecordType.SignedX0, 0, 0))));
                }
            });
        }

        public void StartIntervalUpdate(uint sensorId, uint interval, int outputPower)
        {
            StartOperation((message) =>
            {
                if (message.Header.SensorId == sensorId)
                {
                    _rfmUsb.OutputPower = outputPower;

                    TransmitMessage(
                        EncodeMessage(
                            CreateMessage(
                                message.Header,
                                (OpenThingsParameter)((int)OpenThingsParameter.ReportPeriod | 0x80),
                                new MessageRecordDataUInt(RecordType.SignedX0, sizeof(UInt32), interval))));
                }
            });
        }

        public void StartOtaUpdate(uint sensorId, int outputPower, string hexFile)
        {
            StartOperation((message) =>
            {
                if (File.Exists(hexFile) && (message.Header.SensorId == sensorId))
                {
                    _rfmUsb.OutputPower = outputPower;

                    TransmitMessage(
                        EncodeMessage(
                            CreateMessage(
                                message.Header,
                                (OpenThingsParameter)((int)OpenThingsParameter.ReportPeriod | 0x80),
                                new MessageRecordDataInt(RecordType.SignedX0, 0, 0))));

                    Thread.Sleep(1000);

                    OtaUpdate(hexFile);
                }
                else
                {
                    _logger.LogWarning($"Unable to find hex file [{hexFile}]");
                }
            });
        }

        public void Stop()
        {
            StopRunningTask();
        }

        private void StartOperation(Action<Message> operation)
        {
            if (_task == null)
            {
                _cancellationTokenSource = new CancellationTokenSource();

                _task = Task.Run(() => ExecuteOperation(operation, _cancellationTokenSource.Token), _cancellationTokenSource.Token);
            }
            else
            {
                throw new InvalidOperationException("Service busy");
            }
        }

        private void StopRunningTask()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();

                Task.WaitAll(_task);

                _cancellationTokenSource = null;

                _task = null;
            }
        }

        private void InitaliseRfmUsb()
        {
            _rfmUsb.Reset();

            _rfmUsb.Timeout = 5000;

            _rfmUsb.Modulation = Modulation.Fsk;
            _rfmUsb.FrequencyDeviation = 0x01EC;
            _rfmUsb.Frequency = 434300000;
            _rfmUsb.RxBw = 14;
            _rfmUsb.BitRate = 4800;
            _rfmUsb.Sync = new List<byte>() { 0x2D, 0xD4 };
            _rfmUsb.SyncSize = 1;
            _rfmUsb.SyncEnable = true;
            _rfmUsb.SyncBitErrors = 0;
            _rfmUsb.PacketFormat = true;
            _rfmUsb.DcFree = DcFree.Manchester;
            _rfmUsb.CrcOn = false;
            _rfmUsb.CrcAutoClear = false;
            _rfmUsb.AddressFiltering = AddressFilter.None;
            _rfmUsb.PayloadLength = 66;
        }

        private void ExecuteOperation(Action<Message> operation, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                _rfmUsb.DioInterruptMask = DioIrq.Dio0;
                _rfmUsb.Mode = Mode.Rx;

                do
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    try
                    {
                        _rfmUsb.WaitForIrq();
                    }
                    catch (TimeoutException)
                    {
                    }

                    if ((_rfmUsb.Irq & Irq.PayloadReady) == Irq.PayloadReady)
                    {

                        try
                        {
                            Message message = _openThingsDecoder.Decode(_rfmUsb.Fifo.ToList(), _pidMap);

                            _logger.LogInformation($"Message Decoded {message}");

                            operation(message);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Decoding OpenThings Message Failed. [{ex.Message}]");
                        }
                    }
                } while (true);
            }
        }

        private void TransmitMessage(IList<byte> bytes)
        {
            _rfmUsb.Mode = Mode.Standby;

            _rfmUsb.Transmit(bytes);

            _rfmUsb.Mode = Mode.Rx;
        }

        private Message CreateMessage(MessageHeader messageHeader, OpenThingsParameter openThingsParameter, BaseMessageRecordData messageData)
        {
            var identifyHeader = new MessageHeader(
                        messageHeader.ManufacturerId,
                        messageHeader.ProductId,
                        0,
                        messageHeader.SensorId);

            var message = new Message(identifyHeader);

            var parameter = new Parameter(openThingsParameter);
            var record = new MessageRecord(parameter, messageData);
            message.Records.Add(record);

            return message;
        }

        private IList<byte> EncodeMessage(Message requestMessage)
        {
            var pip = _pipMap.FirstOrDefault(_ => _.ManufacturerId == requestMessage.Header.ManufacturerId);

            List<Byte> encodedMessage;

            if (pip == null)
            {
                encodedMessage = _openThingsEncoder.Encode(requestMessage, pip.Pip, (ushort)_random.Next(ushort.MaxValue)).ToList();
            }
            else
            {
                encodedMessage = _openThingsEncoder.Encode(requestMessage).ToList();
            }

            return encodedMessage;
        }

        private void OtaUpdate(string hexFile)
        {
            try
            {
                using var stream = File.OpenRead(hexFile);

                if (_otaService.OtaUpdate(stream, out uint crc))
                {
                    _logger.LogInformation($"OTA flash update completed. Crc: [0x{crc:X}]");
                }
                else
                {
                    _logger.LogWarning("OTA flash update failed");
                }
            }
            catch (RfmUsbSerialPortNotFoundException ex)
            {
                _logger.LogError(ex.Message);
            }
            catch (RfmUsbCommandExecutionException ex)
            {
                _logger.LogError(ex.Message);
            }
            catch (FileNotFoundException)
            {
                _logger.LogError($"Unable to find file: [{hexFile}]");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
            }
        }
    }
}