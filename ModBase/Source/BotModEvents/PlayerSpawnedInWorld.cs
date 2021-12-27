using System;
using UnityEngine;

namespace Botman.Source.BotModEvents
{
  internal class PlayerSpawnedInWorld
  {
    public static void Handler(ClientInfo _clientInfo, RespawnType _respawnType, Vector3i _position)
    {
      switch (_respawnType)
      {
        case RespawnType.JoinMultiplayer:
          // When a player first joins a seed
          HandleLogin(_clientInfo);
          break;

        case RespawnType.EnterMultiplayer:
          // When a player returns
          HandleLogin(_clientInfo);
          break;

        case RespawnType.NewGame:
          break;
        case RespawnType.LoadedGame:
          break;
        case RespawnType.Died:
          break;
        case RespawnType.Teleport:
          break;
        case RespawnType.Unknown:
          break;
        default:
          break;
      }
    }

    public static void HandleLogin(ClientInfo _clientInfo)
    {
      try
      {
        if (!APICommon.GetPersistentPlayer(_clientInfo, out var persistentPlayer)) { return; }

        UpdatePersistentPlayer(_clientInfo, persistentPlayer);

        // If an clan invite has not expired remind the player
        TryToRemindOfClanInvite(_clientInfo, persistentPlayer);

        // If a players profile was reset and the admin chose to return them.
        TryToReturnToMarkedPosition(_clientInfo, persistentPlayer);

        PersistentContainer.Instance.Save();
      }
      catch (Exception exception)
      {
        Log.Warning($"~Botman Notice~ Error with PlayerSpawnedInWorld.HandleLogin: {exception}");
      }
    }

    private static void UpdatePersistentPlayer(ClientInfo _clientInfo, Player persistentPlayer)
    {
      persistentPlayer.PlayerName = _clientInfo.playerName;
      persistentPlayer.EntityId = _clientInfo.entityId;
    }

    public static void TryToRemindOfClanInvite(ClientInfo _clientInfo, Player persistentPlayer)
    {
      if (persistentPlayer.ClanInvite == null) { return; }

      // Invite expires after 1 day from invite time
      if (persistentPlayer.InviteDate.AddDays(1) < DateTime.Now)
      {
        persistentPlayer.ClanInvite = null;

        return;
      }

      SendMessage.Private(_clientInfo, "You have a clan invite pending for " + persistentPlayer.ClanInvite);
      SendMessage.Private(_clientInfo, "Type \"/accept invite\" to accept or \"/decline invite\" to deny.");
    }

    private static void TryToReturnToMarkedPosition(ClientInfo _clientInfo, Player persistentPlayer)
    {
      if (persistentPlayer.MarkedForReturn == null) { return; }

      try
      {
        var returnPosition = persistentPlayer.MarkedForReturn.Split(' ');
        if (returnPosition.Length < 3 || !int.TryParse(returnPosition[0], out var x) || !int.TryParse(returnPosition[1], out var y) || !int.TryParse(returnPosition[2], out var z))
        {
          // Invalid integers for position
          Log.Out($"~Botman Notice~ Invalid integers for position: {persistentPlayer.MarkedForReturn}");

          return;
        }

        // todo: add a check for sensible values on position

        // Clear return position
        PersistentContainer.Instance.Players[_clientInfo.CrossplatformId, true].MarkedForReturn = null;

        // Teleport player to position
        _clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(new Vector3(x, y, z)));

        SendMessage.Private(_clientInfo, "You have been returned to where you were.");
      }
      catch (Exception)
      {
        Log.Warning("~Botman Notice~ Error with PlayerSpawnedInWorld.HandleLogin.ReturnToMarkedPosition on " + _clientInfo.playerName);
      }
    }
  }
}
