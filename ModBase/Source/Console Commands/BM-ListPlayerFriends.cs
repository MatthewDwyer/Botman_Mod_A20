using System.Collections.Generic;
using System.Linq;

namespace Botman.Commands
{
  public class BMListPlayersFriends : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ List friends of a single player or all players";

    public override string[] GetCommands() => new[] { "bm-listplayerfriends" };

    public override string GetHelp() =>
      "Usage:\n" +
      "  1. bm-listplayerfriends\n" +
      "  2. bm-listplayerfriends <steam id / player name / entity id>\n" +
      "     1. List all online players friends\n" +
      "     2. List a players friends\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count > 1)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params.Count == 0)
      {
        //note: this lists friends of all players including offline
        //foreach (var steamId in GameManager.Instance.persistentPlayers.Players.Keys)
        //{
        //  PrintFriendsOfPlayer(steamId);
        //}

        foreach (var player in GameManager.Instance.World.Players.list)
        {
          if (!player.IsSpawned()) { return; }

          PrintFriendsOfPlayerById(player.entityId);
        }

        return;
      }

      {
        string steamId = null;

        var clientInfo = ConsoleHelper.ParseParamIdOrName(_params[0]);
        if (clientInfo != null)
        {
          steamId = clientInfo.playerId;
        }

        if (steamId == null)
        {
          if (ConsoleHelper.ParseParamSteamIdValid(_params[0]))
          {
            steamId = _params[0];
          }
        }

        if (steamId == null)
        {
          SdtdConsole.Instance.Output("Player name, entity id or steam id not found.");

          return;
        }

        PrintFriendsOfPlayer(steamId);
      }
    }

    private static void PrintFriendsOfPlayerById(int entityId)
    {
      var persistentPlayer = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(entityId);

      if (persistentPlayer == null)
      {
        SdtdConsole.Instance.Output($"Persistent player not found for entity id {entityId}");

        return;
      }

      var friends = persistentPlayer.ACL != null
        ? string.Join(",", persistentPlayer.ACL
          .Where(f => !string.IsNullOrEmpty(f))
          .Select(friendSteamId => $"{friendSteamId}/{PersistentContainer.Instance.Players[friendSteamId, false]?.PlayerName ?? ""}"))
        : string.Empty;

      SdtdConsole.Instance.Output($"Friends of id={persistentPlayer.PlayerId}/{PersistentContainer.Instance.Players[persistentPlayer.PlayerId, false]?.PlayerName ?? ""} Friends={friends}");
    }


    private static void PrintFriendsOfPlayer(string steamId)
    {
      var persistentPlayer = GameManager.Instance.persistentPlayers.GetPlayerData(steamId);

      var friends = persistentPlayer?.ACL != null
        ? string.Join(",", persistentPlayer.ACL
          .Where(f => !string.IsNullOrEmpty(f))
          .Select(friendSteamId => $"{friendSteamId}/{PersistentContainer.Instance.Players[friendSteamId, false]?.PlayerName ?? ""}"))
        : string.Empty;

      SdtdConsole.Instance.Output($"Friends of id={steamId}/{PersistentContainer.Instance.Players[steamId, false]?.PlayerName ?? ""} Friends={friends}");
    }
  }
}
