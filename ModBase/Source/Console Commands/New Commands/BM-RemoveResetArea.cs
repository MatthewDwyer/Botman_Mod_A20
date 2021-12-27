using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMRemoveResetArea : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Removes areas to reset during reboots.";

    public override string[] GetCommands() => new[] { "bm-removeresetarea" };

    public override string GetHelp() =>
      "Removes areas to reset.\n" +
      "Usage:\n" +
      "1. bm-removeresetarea [name]\n" +
      "  1. removes reset areas by name.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {

      if (_params.Count != 1)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (!BMAreaReset.ResetAreas.ContainsKey(_params[0]))
      {
        SdtdConsole.Instance.Output($"Could not find area called {_params[0]} on reset list.");

        return;
      }

      BMAreaReset.ResetAreas.Remove(_params[0]);
      Config.MapUpdateRequired = true;
      Config.UpdateXml();
      SdtdConsole.Instance.Output($"Removed {_params[0]} from reset area list.");
    }
  }
}
