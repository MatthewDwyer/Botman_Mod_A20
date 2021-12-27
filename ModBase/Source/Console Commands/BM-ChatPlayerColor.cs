using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMChatPlayerColor : BMCmdAbstract
  {
    public override string GetDescription() => "Change default player chat and/or color.";

    public override string[] GetCommands() => new[] { "bm-chatplayercolor" };

    public override string GetHelp() =>
      "Change the players chat and/or response color.\n" +
      "Usage:\n" +
      "1. bm-chatplayercolor <steam id/player name/entity id> <color> <nameOnly>\n" +
      "the <color> must be a 6 hex characters. Example: FF00FF\n" +
      "the <nameOnly> must be a 1 to color only name and 0 to color all text\n" +
      "the default chat color is ffffff\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count != 3)
      {
        SdtdConsole.Instance.Output($"Wrong number of arguments, expected 3, found {_params.Count}.");
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      PlatformUserIdentifierAbs platformId = PersistentContainer.Instance.Players.GetSteamId(_params[0]);
      if (platformId == null)
      {
        SdtdConsole.Instance.Output($"Player name, entity id or steam id not found: {_params[0]}");

        return;
      }

      var persistentPlayer = PersistentContainer.Instance.Players[platformId, false];
      if (persistentPlayer == null)
      {
        SdtdConsole.Instance.Output($"Persistent player not found for steam id: {platformId}");

        return;
      }

      var chatName = _params[2];
      if (!chatName.Equals("0") && !chatName.Equals("1"))
      {
        SdtdConsole.Instance.Output("Invalid value for <nameOnly> parameter");

        return;
      }

      if (!CmdHelpers.GetColor(_params[1], out var chatColor)) { return; }

      persistentPlayer.ChatColor = !chatColor.ToLower().Equals("ffffff")
        ? $"[{chatColor}]"
        : null;

      persistentPlayer.ChatName = chatName.Equals("1");

      SdtdConsole.Instance.Output($"{persistentPlayer.PlayerName} chatColor={chatColor}");

      PersistentContainer.Instance.Save();
    }
  }
}
