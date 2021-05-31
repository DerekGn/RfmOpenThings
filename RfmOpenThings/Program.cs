/*
* MIT License
*
* Copyright (c) 2021 Derek Goslin http://corememorydump.blogspot.ie/
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

using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenThings;
using RfmOta;
using RfmUsb;
using Serilog;
using System;
using System.IO;

namespace RfmOpenThings
{
    class Program
    {
        private static ServiceProvider _serviceProvider;
        private static IConfiguration _configuration;

        static int Main(string[] args)
        {
            _configuration = SetupConfiguration(args);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            _serviceProvider = BuildServiceProvider();

            var logger = _serviceProvider.GetService<ILogger<Program>>();

            return Parser.Default.ParseArguments<IdentifyOptions, ListenOptions, OtaOptions, IntervalOptions>(args)
                .MapResult(
                (ListenOptions options) => ExecuteOperation(options, (openThingsService) =>
                {
                    logger.LogInformation("Listening for OpenThings messages. Press any key to exit.");

                    openThingsService.StartListen();
                }),
                (IdentifyOptions options) => ExecuteOperation(options, (openThingsService) =>
                {
                    uint sensorId = options.SensorId.ConvertToUInt();

                    logger.LogInformation($"Listening for OpenThings message from SensorId [0x{sensorId:X}]. Press any key to exit.");

                    openThingsService.StartIdentify(sensorId);
                }),
                (IntervalOptions options) => ExecuteOperation(options, (openThingsService) =>
                {
                    options.ValidateOutputPower();

                    uint sensorId = options.SensorId.ConvertToUInt();
                    uint interval = options.Interval.ConvertToUInt();

                    logger.LogInformation($"Listening for an OpenThings message from SensorId [0x{sensorId:X}] Updating Interval To [0x{interval}:X]. Press any key to exit.");

                    openThingsService.StartIntervalUpdate(sensorId, options.OutputPower, interval);
                }),
                (OtaOptions options) => ExecuteOperation(options, (openThingsService) =>
                {
                    options.ValidateOutputPower();

                    uint sensorId = options.SensorId.ConvertToUInt();

                    logger.LogInformation($"Listening for an OpenThings message from SensorId [0x{sensorId:X}]. Press any key to exit.");

                    openThingsService.StartOtaUpdate(sensorId, options.OutputPower, options.HexFile);
                }),
                errs => -1);
        }

        private static int ExecuteOperation(BaseOptions options, Action<IOpenThingsService> operation)
        {
            int result = -1;

            var logger = _serviceProvider.GetService<ILogger<Program>>();

            var rfmUsb = _serviceProvider.GetService<IRfmUsb>();

            rfmUsb.Open(options.SerialPort, options.BaudRate);

            rfmUsb.Timeout = 10000;

            var openThingsService = _serviceProvider.GetService<IOpenThingsService>();

            logger.LogInformation(rfmUsb.Version);

            try
            {
                operation(openThingsService);

                Console.ReadKey(true);

                logger.LogInformation("Stopping listening for OpenThings messages");

                var stopResult = openThingsService.Stop();

                if (stopResult == OperationResult.Failed)
                {
                    result = -1;
                }
                else
                {
                    result = 0;
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "An unexpected exception occurred");

                result = -1;
            }
            finally
            {
                rfmUsb.Close();
            }

            return result;
        }

        private static ServiceProvider BuildServiceProvider()
        {
            var serviceCollection = new ServiceCollection()
                .AddLogging(builder => builder.AddSerilog())
                .AddSingleton(_configuration)
                .AddLogging()
                .AddOpenThings()
                .AddOpenThingsService()
                .AddOta();

            return serviceCollection.BuildServiceProvider();
        }

        private static IConfiguration SetupConfiguration(string[] args)
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
        }
    }
}
