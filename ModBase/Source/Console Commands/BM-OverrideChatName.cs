using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMOverrideChatName : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Change players displayed chat name.";

    public override string[] GetCommands() => new[] { "bm-overridechatname" };

    public override string GetHelp() =>
      "Change the player`s chat name.\n" +
      "Usage:\n" +
      "1. bm-overridechatname <steam id/player name/entity id> <newname>\n";

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
        SdtdConsole.Instance.Output("Playername or entity/steamid id not found.");

        return;
      }

      var player = PersistentContainer.Instance.Players[steamId, true];
      SdtdConsole.Instance.Output($"{player.PlayerName}'s chat name has been changed to {_params[1]}");
      PersistentContainer.Instance.Players[steamId, true].PlayerName = _params[1];
      PersistentContainer.Instance.Save();
    }
  }
}
