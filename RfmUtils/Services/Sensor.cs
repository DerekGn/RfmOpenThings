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

// Ignore Spelling: Utils Rfm Energenie

using System;
using System.Collections.Generic;

namespace RfmUtils.Services
{
    public record Sensor
    {
        public Sensor(
            uint sensorId,
            int productId,
            Manufacturer manufacturer,
            DateTime discovered)
        {
            SensorId = sensorId;
            ProductId = productId;
            Manufacturer = manufacturer;
            Discovered = discovered;

            ProductType = MapProductType(Manufacturer, ProductId);
        }

        public uint SensorId { get; }

        public int ProductId { get; }

        public string ProductType { get; }

        public Manufacturer Manufacturer { get; }

        public DateTime Discovered { get; }

        public override string ToString()
        {
            return $"SensorId:\t[0x{SensorId:X}]\r\nProductId:\t[0x{ProductId:X2}]\r\n" +
                $"ProductType:\t[{ProductType}]\r\nManufacture:\t[{Manufacturer}]\r\n" +
                $"Discovered:\t[{Discovered}]";
        }

        private static string? MapProductType(Manufacturer manufacturer, int productId)
        {
            if (manufacturer == Manufacturer.Energenie)
            {
                return MapEnergenieProductType(productId);
            }
            else if(manufacturer == Manufacturer.Alt)
            {
                return MapAltProductType(productId);
            }
            else
            {
                return "Unknown";
            }
        }

        private static string? MapAltProductType(int productId)
        {
            return productId switch
            {
                0x01 => "Temperature Humidity Sensor",
                0x02 => "AQS Sensor",
                0x03 => "Energy Meter",
                _ => "Unknown",
            };
        }

        private static string? MapEnergenieProductType(int productId)
        {
            return productId switch
            {
                0x01 => "MIHO004 Home Monitor",
                0x02 => "MIHO005 Home Smart Plug",
                0x03 => "MIHO013 eTrv",
                0x05 => "MIHO006 Smart Power Monitor",
                0x0C => "MIHO032 Smart Motion Sensor",
                0x0D => "MIHO032 Smart Door/Window Open Sensor",
                _ => "Unknown",
            };
        }
    }
}