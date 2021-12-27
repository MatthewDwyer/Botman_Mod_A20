using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMMutePlayerChat : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Mute a player in public chat";

    public override string[] GetCommands() => new[] { "bm-muteplayer" };

    public override string GetHelp() =>
      "Mutes a player in public chat.\n" +
      "Usage:\n" +
      "1. bm-muteplayer <steam id/player name/entity id> [true/false]\n" +
      "If the optional parameter is not given the command will show the current status.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params.Count < 1 || _params.Count > 2)
      {
        SdtdConsole.Instance.Output($"Wrong number of arguments, expected 1 or 2, found {_params.Count}.");
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

      if (_params.Count == 2)
      {
        switch (_params[1].ToLower())
        {
          case "true":
            persistentPlayer.IsChatMuted = true;
            break;

          case "false":
            persistentPlayer.IsChatMuted = false;
            break;

          default:
            SdtdConsole.Instance.Output("Wrong param 2. It must be \"true\" or \"false\"");
            return;
        }
      }

      SdtdConsole.Instance.Output($"{persistentPlayer.PlayerName} {(persistentPlayer.IsChatMuted ? "muted" : "unmuted")}");

      PersistentContainer.Instance.Save();
    }
  }
}
