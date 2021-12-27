using System;
using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMUnlockAll : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Unlock all secure loots, chests and doors for the current player.";

    public override string[] GetCommands() => new[] { "bm-unlockall" };

    public override string GetHelp() =>
      "Unlock all secure loots, chests and doors for the current player." +
      "1. bm-unlockall";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      var clientInfo = _senderInfo.RemoteClientInfo;

      if (null == clientInfo)
      {
        SdtdConsole.Instance.Output("Unable to apply this command. You need be a player to execute it.");

        return;
      }

      var playerId = clientInfo.CrossplatformId;
      var unlockedLoot = 0;
      var unlockedLootSigned = 0;
      var unlockedDoors = 0;
      var unlockedSigns = 0;
      var unlockedVendingMachines = 0;
      var totalLockedLoot = 0;
      var totalLockedLootSigned = 0;
      var totalLockedDoors = 0;
      var totalLockedSigns = 0;
      var totalLockedVendingMachines = 0;

      var chunks = new Dictionary<long, Chunk>();
      for (var i = 0; i < GameManager.Instance.World.ChunkClusters.Count; i++)
      {
        foreach (var chunk in GameManager.Instance.World.ChunkClusters[i].GetChunkArray())
        {
          foreach (var tileEntity in chunk.GetTileEntities().dict.Values)
          {
            try
            {
              switch (tileEntity.GetTileEntityType())
              {
                case TileEntityType.SecureLoot when tileEntity is TileEntitySecureLootContainer te:
                  {
                    totalLockedLoot++;

                    var users = te.GetUsers();
                    if (users.Contains(playerId)) { continue; }

                    users.Add(playerId);
                    unlockedLoot++;

                    if (!chunks.ContainsKey(chunk.Key))
                    {
                      chunks.Add(chunk.Key, chunk);
                    }

                    break;
                  }

                case TileEntityType.SecureDoor when tileEntity is TileEntitySecureDoor te:
                  {
                    totalLockedDoors++;

                    var users = te.GetUsers();
                    if (users.Contains(playerId)) { continue; }

                    users.Add(playerId);
                    unlockedDoors++;

                    if (!chunks.ContainsKey(chunk.Key))
                    {
                      chunks.Add(chunk.Key, chunk);
                    }

                    break;
                  }

                case TileEntityType.SecureLootSigned when tileEntity is TileEntitySecureLootContainerSigned te:
                  {
                    totalLockedLootSigned++;

                    var users = te.GetUsers();
                    if (users.Contains(playerId)) { continue; }

                    users.Add(playerId);
                    unlockedLootSigned++;

                    if (!chunks.ContainsKey(chunk.Key))
                    {
                      chunks.Add(chunk.Key, chunk);
                    }

                    break;
                  }

                case TileEntityType.Sign when tileEntity is TileEntitySign te:
                  {
                    totalLockedSigns++;

                    var users = te.GetUsers();
                    if (users.Contains(playerId)) { continue; }

                    users.Add(playerId);
                    unlockedSigns++;

                    if (!chunks.ContainsKey(chunk.Key))
                    {
                      chunks.Add(chunk.Key, chunk);
                    }

                    break;
                  }

                case TileEntityType.VendingMachine when tileEntity is TileEntityVendingMachine te:
                  {
                    totalLockedVendingMachines++;

                    var users = te.GetUsers();
                    if (users.Contains(playerId)) { continue; }

                    users.Add(playerId);
                    unlockedVendingMachines++;

                    if (!chunks.ContainsKey(chunk.Key))
                    {
                      chunks.Add(chunk.Key, chunk);
                    }

                    break;
                  }

                default:
                  break;
              }
            }
            catch (Exception e)
            {
              SdtdConsole.Instance.Output("~Botman Notice~ Error with bm-unlockall: " + e);
              return;
            }
          }
        }
      }

      if (chunks.Count > 0)
      {
        Reload(chunks, clientInfo);
      }

      if (totalLockedLoot > 0) { SdtdConsole.Instance.Output($"Secure loot containers unlocked: {unlockedLoot}/{totalLockedLoot}"); }
      if (totalLockedDoors > 0) { SdtdConsole.Instance.Output($"Secure doors unlocked: {unlockedDoors}/{totalLockedDoors}"); }
      if (totalLockedLootSigned > 0) { SdtdConsole.Instance.Output($"Secure signed loot containers unlocked: {unlockedLootSigned}/{totalLockedLootSigned}"); }
      if (totalLockedSigns > 0) { SdtdConsole.Instance.Output($"Secure signs unlocked: {unlockedSigns}/{totalLockedSigns}"); }
      if (totalLockedVendingMachines > 0) { SdtdConsole.Instance.Output($"Secure vending machines unlocked: {unlockedVendingMachines}/{totalLockedVendingMachines}"); }
    }

    public static void Reload(Dictionary<long, Chunk> chunks, ClientInfo clientInfo)
    {
      if (clientInfo == null) { return; }

      foreach (var key in chunks.Keys)
      {
        if (chunks.TryGetValue(key, out var chunk))
        {
          clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChunkRemove>().Setup(chunk.Key));
        }
      }

      foreach (var key in chunks.Keys)
      {
        if (chunks.TryGetValue(key, out var chunk2))
        {
          clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChunk>().Setup(chunk2));
        }
      }
    }
  }
}
