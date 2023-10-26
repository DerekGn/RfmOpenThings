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

// Ignore Spelling: Utils Rfm app ota

using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using OpenThings;
using RfmUsb.Net;
using System.ComponentModel.DataAnnotations;

namespace RfmUtils.Commands
{
    [Command(Description = "OTA a sensor")]
    internal class OtaCommand : BaseRadioRxCommand
    {
        public OtaCommand(
            IOpenThingsDecoder openThingsDecoder,
            IConfiguration configuration,
            IParameters parameters,
            IRfm69 rfm69) 
            : base(openThingsDecoder, configuration, parameters, rfm69)
        {
        }

        [Required]
        [Option(Templates.HexFile, "The hex file to flash to the device", CommandOptionType.SingleValue)]
        public string? HexFile { get; set; }

        protected override int OnExecute(CommandLineApplication app, IConsole console)
        {
            return base.OnExecute(app, console);
        }
    }
}
