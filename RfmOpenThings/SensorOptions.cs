using CommandLine;

namespace RfmOpenThings
{
    internal class SensorOptions : BaseOptions
    {
        [Option('i', "sensorId", Required = true, HelpText = "The OpenThings sensor id")]
        public string SensorId { get; set; }
    }
}
