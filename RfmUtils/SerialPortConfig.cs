namespace RfmUtils
{
    internal class RfmUsbConfig
    {
        public RfmUsbConfig(string serialPort, int baudRate, byte outputPower)
        {
            SerialPort = serialPort;
            BaudRate = baudRate;
            OutputPower = outputPower;
        }

        public string SerialPort { get; }
        public int BaudRate { get; }
        public byte OutputPower { get; }
    }
}
