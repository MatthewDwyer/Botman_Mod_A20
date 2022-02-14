using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botman.Commands
{
  public class BMDroneFileDelete : BMCmdAbstract
  {
    public static bool Enabled = false;

    public override string GetDescription() => "~Botman~ If enabled, deletes drones on reboot";

    public override string[] GetCommands() => new[] { "bm-dronefiledel" };

    public override string GetHelp() =>
      "If enabled, deletes drones on reboot:\n" +
      "Usage:\n" +
      "1. bm-dronefiledel enable/disable \n";

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
          SdtdConsole.Instance.Output("~Botman~ Drones will be deleted after every reboot.");
          return;

        case "disable":
          Enabled = false;
          Config.UpdateXml();
          SdtdConsole.Instance.Output("~Botman~ No drones will be deleted.");
          return;

        default:
          SdtdConsole.Instance.Output(GetHelp());
          return;
      }
    }
  }
}
