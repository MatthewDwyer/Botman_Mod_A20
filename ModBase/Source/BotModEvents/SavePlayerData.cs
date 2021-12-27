namespace Botman.Source.BotModEvents
{
  internal class SavePlayerData
  {
    public static void Handler(ClientInfo _clientInfo, PlayerDataFile _playerDataFile)
    {
      if (!APICommon.GetOnlinePlayer(_clientInfo, out var persistentPlayer, out var entityPlayer)) { return; }
      UpdatePersistentPlayer(persistentPlayer, entityPlayer);
      PersistentContainer.Instance.Save();

      if (AntiCheat.Enabled)
      {
        AntiCheat.Check(_clientInfo);
      }
    }

    private static void UpdatePersistentPlayer(Player persistentPlayer, Entity entityPlayer)
    {
      var pos = entityPlayer.GetBlockPosition();

      persistentPlayer.LastPosition = $"{pos.x} {pos.y} {pos.z}";
    }
  }
}
