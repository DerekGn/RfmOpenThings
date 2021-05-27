using Microsoft.Extensions.Logging;
using OpenThings;
using RfmUsb;
using System;
using System.Collections.Generic;
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
        private readonly IRfmUsb _rfmUsb;

        private CancellationTokenSource _cancellationTokenSource;
        private Task _task;

        public OpenThingsService(ILogger<OpenThingsService> logger, IRfmUsb rfmUsb, IOpenThingsDecoder openThingsDecoder, IOpenThingsEncoder openThingsEncoder)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _rfmUsb = rfmUsb ?? throw new ArgumentNullException(nameof(rfmUsb));
            _openThingsDecoder = openThingsDecoder ?? throw new ArgumentNullException(nameof(openThingsDecoder));
            _openThingsEncoder = openThingsEncoder ?? throw new ArgumentNullException(nameof(openThingsEncoder));
            InitaliseRfmUsb();
        }

        public void StartListen()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _task = Task.Run(() => ExecuteOperation(() =>
            {
                try
                {
                    Message message = _openThingsDecoder.Decode(_rfmUsb.Fifo.ToList(), new List<PidMap>() { new PidMap(4, 242) });
                    
                    _logger.LogInformation(message.ToString());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred decoding OpenThings message");
                }

                

            }, _cancellationTokenSource.Token), _cancellationTokenSource.Token);
        }

        private void ExecuteOperation(Action operation, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                _rfmUsb.DioInterruptMask = DioIrq.Dio0;
                _rfmUsb.Mode = Mode.Rx;

                do
                {
                    if(cancellationToken.IsCancellationRequested)
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
                        _logger.LogInformation($"[Packet Received]");
                        operation();
                    }
                } while (true);
            }
        }

        public void StopListen()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();

                Task.WaitAll(_task);

                _cancellationTokenSource = null;
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
    }
}