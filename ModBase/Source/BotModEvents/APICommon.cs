namespace Botman.Source.BotModEvents
{
  internal class APICommon
  {
    internal static bool GetOnlinePlayer(ClientInfo _clientInfo, out Player persistentPlayer, out EntityPlayer entityPlayer)
    {
      entityPlayer = null;

      if (!GetPersistentPlayer(_clientInfo, out persistentPlayer) || GameManager.Instance.World == null)
      {
        return false;
      }

      // Need to check if player is in dict first, if disconnected while still loading they will not be present
      return GameManager.Instance.World.Players.dict.TryGetValue(_clientInfo.entityId, out entityPlayer) && entityPlayer != null;
    }

    internal static bool GetPersistentPlayer(ClientInfo _clientInfo, out Player persistentPlayer)
    {
      persistentPlayer = null;

      if (_clientInfo == null) { return false; }

      // Load or create the persistent player, if _clientInfo.playerId is invalid then null is returned.
      persistentPlayer = PersistentContainer.Instance.Players[_clientInfo.CrossplatformId.CombinedString, create: true];

      return persistentPlayer != null;
    }
  }
}
