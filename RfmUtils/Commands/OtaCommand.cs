
using McMaster.Extensions.CommandLineUtils;

namespace RfmUtils.Commands
{
    [Command(Description = "Perform OTA of device")]
    [Subcommand(typeof(OtaEnterCommand))]
    [Subcommand(typeof(OtaFlashCommand))]
    internal class OtaCommand : BaseCommand
    {
    }
}
