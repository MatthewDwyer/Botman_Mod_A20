using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMPlayerChatMaxLength : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Set the maximum number of characters a player can write in a single message";

    public override string[] GetCommands() => new[] { "bm-playerchatmaxlength" };

    public override string GetHelp() =>
      "Set the maximum number of characters a player can write in a single message.\n" +
      "Usage:\n" +
      "1. bm-playerchatmaxlength <steam id/player name/entity id> <chat length>\n";

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
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      var steamId = PersistentContainer.Instance.Players.GetSteamId(_params[0]);
      if (steamId == null)
      {
        SdtdConsole.Instance.Output("Player name, entity id or steam id not found.");

        return;
      }

      var persistentPlayer = PersistentContainer.Instance.Players[steamId, false];
      if (persistentPlayer == null)
      {
        SdtdConsole.Instance.Output("Player not found.");

        return;
      }

      if (!int.TryParse(_params[1], out var maxLength) || maxLength < 1)
      {
        SdtdConsole.Instance.Output("The parameter \"chat length\" must be an integer equal or greater than 1");

        return;
      }

      persistentPlayer.MaxChatLength = maxLength;

      SdtdConsole.Instance.Output($"{persistentPlayer.PlayerName}: max chat length changed to {maxLength}");

      PersistentContainer.Instance.Save();
    }
  }
}
