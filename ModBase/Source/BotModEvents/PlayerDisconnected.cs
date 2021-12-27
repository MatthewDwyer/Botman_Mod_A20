using System;

namespace Botman.Source.BotModEvents
{
  internal class PlayerDisconnected
  {
    public static void Handler(ClientInfo _cInfo, bool _isShutdown)
    {
            Log.Out($"~Botman~ PlayerDisconnected");

            if (!APICommon.GetOnlinePlayer(_cInfo, out var persistentPlayer, out var entityPlayer)) { return; }

            Log.Out($"~Botman~ PlayerDisconnected");

            int adminLevel = GameManager.Instance.adminTools.GetUserPermissionLevel(_cInfo);

            Log.Out($"~Botman~ PlayerDisconnected");

            if (AntiCheat.Spectators.Count > 0 && AntiCheat.Spectators.Contains(_cInfo.CrossplatformId.CombinedString))
      {
        AntiCheat.Spectators.Remove(_cInfo.CrossplatformId.CombinedString);
      }
            Log.Out($"~Botman~ PlayerDisconnected");
            if (AntiCheat.GodMode.Count > 0 && AntiCheat.GodMode.Contains(_cInfo.CrossplatformId.CombinedString))
      {
                Log.Out($"~Botman~");
                for (int i= AntiCheat.GodMode.Count; i > -1; i--)
        {
                    Log.Out($"~Botman~");
                    if (AntiCheat.GodMode[i].Contains(_cInfo.CrossplatformId.CombinedString))
          {
                        Log.Out($"~Botman~ remove anticheat line");
                        AntiCheat.GodMode.RemoveAt(i);
          }
        }

//        AntiCheat.GodMode.Remove($"{_cInfo.playerId}");
      }

      UpdatePersistentPlayer(persistentPlayer, entityPlayer);

      PersistentContainer.Instance.Save();
    }

    public static void UpdatePersistentPlayer(Player persistentPlayer, Entity entityPlayer)
    {
      try
      {
        persistentPlayer.LastPosition = entityPlayer.GetBlockPosition().ToString();
        persistentPlayer.IsOnline = false;
      }
      catch (Exception exception)
      {
        Log.Warning($"~Botman Notice~ Error with PlayerDisconnected.UpdatePersistentPlayer: {exception}");
      }
    }
  }
}
