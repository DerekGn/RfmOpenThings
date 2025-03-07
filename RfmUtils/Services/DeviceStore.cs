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

// Ignore Spelling: Utils Rfm

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace RfmUtils.Services
{
    internal class SensorStore : ISensorStore
    {
        private const string FileName = "discovered-sensors.json";

        public void ClearSensors()
        {
            if(File.Exists(FileName))
            {
                File.Delete(FileName);
            }
        }

        public IEnumerable<Sensor> ReadSensors()
        {
            IList<Sensor> sensors = new List<Sensor>();

            if (File.Exists(FileName))
            {
                using StreamReader inputFile = new StreamReader(FileName);

                sensors = (IList<Sensor>)JsonSerializer.Deserialize<IEnumerable<Sensor>>(inputFile.ReadToEnd());
            }

            return sensors;
        }

        public void WriteSensors(IEnumerable<Sensor> sensors)
        {
            if (sensors.Any())
            {
                using StreamWriter outputFile = new StreamWriter(FileName);

                outputFile.Write(JsonSerializer.Serialize(sensors));
            }
        }
    }
}