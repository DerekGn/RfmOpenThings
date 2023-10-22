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

using McMaster.Extensions.CommandLineUtils;
using RfmUtils.Services;
using System;
using System.Linq;

namespace RfmUtils.Commands
{
    [Command(Description = "List all previously discovered sensors")]
    internal class ListCommand : BaseCommand
    {
        private readonly ISensorStore _deviceStore;

        public ListCommand(ISensorStore deviceStore)
        {
            _deviceStore = deviceStore ?? throw new ArgumentNullException(nameof(deviceStore));
        }

        protected override int OnExecute(CommandLineApplication app, IConsole console)
        {
            int result = -1;

            try
            {
                console.WriteLine("Listing discovered devices");

                var devices = _deviceStore.ReadSensors();

                console.WriteLine($"[{devices.Count()}] Stored devices");

                foreach (var device in devices)
                {
                    console.WriteLine(device);
                }

                result = 0;
            }
            catch (Exception ex)
            {
                console.Error.WriteLine("Unhandled exception occurred. {ex}", ex);
            }

            return result;
        }
    }
}