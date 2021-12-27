using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMGiveXP : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Give XP to a player";

    public override string[] GetCommands() => new[] { "bm-givexp" };

    public override string GetHelp() =>
      "Gives XP to a player\n" +
      "Usage: \n" +
      "1. bm-givexp <name/entity id> <amount xp>\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params.Count != 2)
      {
        SdtdConsole.Instance.Output($"Wrong number of arguments, expected 2, found {_params.Count}.");

        return;
      }

      if (!CmdHelpers.GetClientInfo(_params[0], out var clientInfo)) { return; }

      if (!int.TryParse(_params[1], out var xpValue) || xpValue < 1)
      {
        SdtdConsole.Instance.Output("The amount of XP is not a valid positive number.");

        return;
      }

      clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageConsoleCmdClient>().Setup($"giveselfxp {xpValue}", true));

      SdtdConsole.Instance.Output($"{xpValue} xp was given to player {clientInfo.playerName}");
    }
  }
}
