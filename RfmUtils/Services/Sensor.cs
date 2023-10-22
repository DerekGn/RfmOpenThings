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

using System;
using System.Collections.Generic;

namespace RfmUtils.Services
{
    public record Sensor
    {
        private static readonly List<Tuple<int, string>> productTypeMap = new List<Tuple<int, string>>()
        {
            new Tuple<int, string>( 0x55, "Alt"),
            new Tuple<int, string>( 4, "Energenie")
        };

        public Sensor(
            uint sensorId,
            int productId,
            int manufacturer,
            DateTime discovered)
        {
            SensorId = sensorId;
            ProductId = productId;
            Manufacturer = (Manufacturer)manufacturer;
            Discovered = discovered;

            //ProductType = MapProductType(Manufacturer, ProductId);
        }

        public uint SensorId { get; }

        public int ProductId { get; }

        public string ProductType { get; }

        public Manufacturer Manufacturer { get; }

        public DateTime Discovered { get; }

        public override string ToString()
        {
            return $"SensorId:\t[0x{SensorId:X}]\r\nProductId:\t[0x{ProductId:X}]\r\n" +
                $"ProductType:\t[{ProductType}]\r\nManufacture:\t[{Manufacturer}]\r\n" +
                $"Discovered:\t[{Discovered}]";
        }

        //private static string? MapProductType(Manufacturer manufacturer, int productId)
        //{

        //}
    }
}