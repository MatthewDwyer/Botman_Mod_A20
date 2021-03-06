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
        PlatformUserIdentifierAbs platformId = null;

        var clientInfo = ConsoleHelper.ParseParamIdOrName(_params[0]);
        if (clientInfo != null)
        {
           platformId = clientInfo.CrossplatformId;
        }

        //if (platformId == null)
        //{
        //   platformId = _params[0];
        //}

        if (platformId == null)
        {
          SdtdConsole.Instance.Output("Player name, entity id or steam id not found.");

          return;
        }

        PrintFriendsOfPlayer(platformId);
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
          .Where(f => !string.IsNullOrEmpty(f.CombinedString))
          .Select(friendSteamId => $"{friendSteamId}/{PersistentContainer.Instance.Players[friendSteamId.CombinedString, false]?.PlayerName ?? ""}"))
        : string.Empty;

      SdtdConsole.Instance.Output($"Friends of id={persistentPlayer.UserIdentifier.CombinedString}/{PersistentContainer.Instance.Players[persistentPlayer.UserIdentifier.CombinedString, false]?.PlayerName ?? ""} Friends={friends}");
    }


    private static void PrintFriendsOfPlayer(PlatformUserIdentifierAbs platformId)
    {
      var persistentPlayer = GameManager.Instance.persistentPlayers.GetPlayerData(platformId);

      var friends = persistentPlayer?.ACL != null
        ? string.Join(",", persistentPlayer.ACL
          .Where(f => !string.IsNullOrEmpty(f.CombinedString))
          .Select(friendSteamId => $"{friendSteamId}/{PersistentContainer.Instance.Players[friendSteamId.CombinedString, false]?.PlayerName ?? ""}"))
        : string.Empty;

      SdtdConsole.Instance.Output($"Friends of id={platformId}/{PersistentContainer.Instance.Players[platformId.CombinedString, false]?.PlayerName ?? ""} Friends={friends}");
    }
  }
}
