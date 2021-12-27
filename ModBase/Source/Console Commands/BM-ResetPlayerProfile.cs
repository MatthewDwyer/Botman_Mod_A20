using System.Collections.Generic;
using System.IO;

namespace Botman.Commands
{
  public class BMResetPlayer : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Reset a players profile. Warning, can not be undone";

    public override string[] GetCommands() => new[] { "bm-resetplayer" };

    public override string GetHelp() =>
      "Resets a players profile. Warning, can not be undone\n" +
      "Usage:\n" +
      "1. bm-resetplayer <steamId/entityId> <optional=(true)> \n" +
      "   1. Resets a players profile. If the player is offline, they will log in to new profile. \n" +
      "*If true is used in optional parameter, the players location will be marked and they will return on login.";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params.Count != 1 && _params.Count != 2)
      {
        SdtdConsole.Instance.Output($"Wrong number of arguments, expected 1 or 2, found {_params.Count}.");

        return;
      }

      var markForReturn = false;
      if (_params.Count == 2)
      {
        if (_params[1].EqualsCaseInsensitive("true"))
        {
          markForReturn = true;
        }
      }

      PlatformUserIdentifierAbs steamId = PersistentContainer.Instance.Players.GetSteamId(_params[0]);
      if (steamId == null)
      {
        SdtdConsole.Instance.Output($"Can not reset Id: Invalid Id {_params[0]}");

        return;
      }

      ResetPlayer(steamId, markForReturn);
    }

    private static void ResetPlayer(PlatformUserIdentifierAbs steamId, bool markForReturn)
    {
      // Note: If world is still loading this wont give the correct location as GameWorld and GameName prefs wont be set
      var saveGamePath = GameIO.GetSaveGameDir();

      var playerMapPath = $"{saveGamePath}/Player/{steamId}.map";
      var playerTtpPath = $"{saveGamePath}/Player/{steamId}.ttp";

      Log.Out($"~Botman Notice~ Searching for {playerMapPath}");
      Log.Out($"~Botman Notice~ Searching for {playerTtpPath}");

      var persistentPlayer = PersistentContainer.Instance.Players[steamId, false];
      if (persistentPlayer == null)
      {
        SdtdConsole.Instance.Output($"Player file {steamId}.ttp does not exist");

        return;
      }

      var clientInfo = ConsoleHelper.ParseParamIdOrName(steamId.CombinedString, true, false);
      if (clientInfo == null)
      {
        if (markForReturn)
        {
          persistentPlayer.MarkedForReturn = persistentPlayer.LastPosition;

          PersistentContainer.Instance.Save();
        }

        DeleteFiles(steamId.CombinedString, playerMapPath, playerTtpPath);

        SdtdConsole.Instance.Output($"{persistentPlayer.PlayerName} is offline but their profile has been reset.");

        return;
      }

      if (markForReturn)
      {
        var entityPlayer = GameManager.Instance.World.Players.dict[clientInfo.entityId];
        if (entityPlayer == null)
        {
          SdtdConsole.Instance.Output("Player not found for position, using last known position.");

          persistentPlayer.MarkedForReturn = persistentPlayer.LastPosition;
        }
        else
        {
          persistentPlayer.MarkedForReturn = $"{(int)entityPlayer.position.x} {(int)entityPlayer.position.y} {entityPlayer.position.z}";
        }

        PersistentContainer.Instance.Save();
      }

      SdtdConsole.Instance.ExecuteSync($"kick {steamId} \"Your player profile has been reset by an admin\"", null);

      DeleteFiles(steamId.CombinedString, playerMapPath, playerTtpPath);

      SdtdConsole.Instance.Output($"You have reset the profile for {persistentPlayer.PlayerName}");
    }

    private static void DeleteFiles(string steamId, string playerMapPath, string playerTtpPath)
    {
      if (!File.Exists(playerMapPath))
      {
        SdtdConsole.Instance.Output($"Could not find file {steamId}.map");
      }
      else
      {
        File.Delete(playerMapPath);
      }

      if (!File.Exists(playerTtpPath))
      {
        SdtdConsole.Instance.Output($"Could not find file {steamId}.ttp");
      }
      else
      {
        File.Delete(playerTtpPath);
      }
    }
  }
}
