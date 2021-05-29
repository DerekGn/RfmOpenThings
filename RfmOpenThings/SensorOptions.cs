using CommandLine;

namespace RfmOpenThings
{
    internal class SensorOptions : BaseOptions
    {
        [Option('i', "sensorId", Required = true, HelpText = "The OpenThings sensor id. Can be decimal, octal or hex formatted value.")]
        public string SensorId { get; set; }
    }
}
