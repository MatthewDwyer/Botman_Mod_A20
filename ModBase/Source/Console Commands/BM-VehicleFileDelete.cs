using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMVehicleFileDelete : BMCmdAbstract
  {
    public static bool Enabled = false;

    public override string GetDescription() => "~Botman~ If enabled, deletes vehicles on reboot";

    public override string[] GetCommands() => new[] { "bm-vehiclefiledel" };

    public override string GetHelp() =>
      "If enabled, deletes vehicles on reboot:\n" +
      "Usage:\n" +
      "1. bm-vehiclefiledel enable/disable \n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count != 1)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      switch (_params[0].ToLower())
      {
        case "enable":
          Enabled = true;
          Config.UpdateXml();
          SdtdConsole.Instance.Output("~Botman~ Vehicles will be deleted after every reboot.");
          return;

        case "disable":
          Enabled = false;
          Config.UpdateXml();
          SdtdConsole.Instance.Output("~Botman~ No vehicles will be deleted.");
          return;

        default:
          SdtdConsole.Instance.Output(GetHelp());
          return;
      }
    }
  }
}
