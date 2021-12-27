using System;
using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMAntiCheatCmd : BMCmdAbstract
  {
    //Change 1.9// Added in all of the following.
    public static int AuthorizedSpectatorLevel = 0;
    public static int AuthorizedGodMode = 0;

    public override string GetDescription() => "~Botman~ Customize Anti Cheat Settings";

    public override string[] GetCommands() => new[] { "bm-anticheat" };

    public override string GetHelp() =>
      "Enable/Disable Anti Cheat:\n" +
      "Usage:\n" +
      "1. bm-anticheat enable/disable \n" +
      "2. bm-anticheat report \n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params.Count != 1)
      {
        SdtdConsole.Instance.Output("~Botman~ Invalid amount of parameters.");

        return;
      }

      if (_params[0].Equals("enable", StringComparison.InvariantCultureIgnoreCase))
      {
        AntiCheat.Enabled = true;
        SdtdConsole.Instance.Output("~Botman~ Anti Cheat Enabled");
        Config.UpdateXml();

        return;
      }

      if (_params[0].Equals("disable", StringComparison.InvariantCultureIgnoreCase))
      {
        AntiCheat.Enabled = false;
        SdtdConsole.Instance.Output("~Botman~ Anti Cheat disabled");
        Config.UpdateXml();

        return;
      }

      if (_params[0].Equals("report", StringComparison.InvariantCultureIgnoreCase))
      {
        AntiCheat.Report();

        return;
      }

      SdtdConsole.Instance.Output(GetHelp());
    }
  }
}
