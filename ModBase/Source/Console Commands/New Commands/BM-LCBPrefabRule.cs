using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMLCBPrefabRule : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Enables/Disables LCB allowance near prefabs..";

    public override string[] GetCommands() => new[] { "bm-lcbprefabrule" };

    public override string GetHelp() =>
      "Enables/Disables LCB allowance near prefabs..\n" +
      "Usage:\n" +
      "1. bm-lcbprefabrule enable/disable \n" +
      "2. bm-lcbprefabrule distance <number> \n" +
      "  1. Turns tool on/off\n" +
      "  2. Distance from a prefab lcbs are allowed to be placed.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      switch (_params[0].ToLower())
      {
        case "enable":
          LCBPlacement.PrefabRangeEnabled = true;
          Config.UpdateXml();
          SdtdConsole.Instance.Output($"Players will not be allowed to place LCBs within {LCBPlacement.PrefabRangeSearch} blocks of a prefab.");
          return;

        case "disable":
          LCBPlacement.PrefabRangeEnabled = false;
          Config.UpdateXml();
          SdtdConsole.Instance.Output("No longer searching for prefabs nearby.");
          return;

        case "distance":
          {
            if (!int.TryParse(_params[1], out var distance) || distance < 0)
            {
              SdtdConsole.Instance.Output("Distance must be a positive number");

              return;
            }

            LCBPlacement.PrefabRangeSearch = distance;
            Config.UpdateXml();
            SdtdConsole.Instance.Output($"Players will not be allowed to place LCBs within { LCBPlacement.PrefabRangeSearch} blocks of a prefab.");

            return;
          }

        default:
          SdtdConsole.Instance.Output(GetHelp());
          return;
      }
    }
  }
}
