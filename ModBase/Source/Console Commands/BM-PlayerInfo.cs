using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMPlayerInfo : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Player Info";

    public override string[] GetCommands() => new[] { "bm-playerinfo" };

    public override string GetHelp() =>
      "Usage:\n" +
      "1. bm-playerinfo [playername]\n" +
      "   2. bm-playerinfo [playername]\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count < 1)
      {
        SdtdConsole.Instance.Output("Player name is required");

        return;
      }

      string id = PersistentContainer.Instance.Players.GetId(_params[0]);

      if (id == null)
      {
        SdtdConsole.Instance.Output($"Could not locate {_params[0]}");

        return;
      }

      var player = PersistentContainer.Instance.Players[id, true];

      SdtdConsole.Instance.Output(
        $"PlayerName {player.PlayerName}\n" +
        $"Id :{id}\n" +
        $"IsOnline :{player.IsOnline}\n" +
        $"ChatName :{player.ChatName}\n" +
        $"ChatColor :{player.ChatColor}\n");
    }
  }
}
