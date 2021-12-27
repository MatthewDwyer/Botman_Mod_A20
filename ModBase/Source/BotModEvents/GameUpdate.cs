using System;
using System.Collections.Generic;
using Botman.Commands;

namespace Botman.Source.BotModEvents
{
  internal class GameUpdate
  {
    public static bool IsAlive;

    private static long nextTick;
    private const long TickLimiter = TimeSpan.TicksPerSecond;

        internal static void Handler()
    {
      if (!IsAlive) { return; }

      if (DateTime.UtcNow.Ticks > nextTick)
      {
        BMSanctuaries.Update();
        EntityWatch.Update();
        LCBPlacement.Update();
        LevelSystem.Update();
        BMZombieFreeTime.Update();

        nextTick = DateTime.UtcNow.Ticks + TickLimiter;
      }

      var currentTick = DateTime.UtcNow.Ticks;
      var removeList = new List<Vector3i>();
      foreach (var item in API.rlpQueue)
      {
        if (item.Value < currentTick) {
          removeList.Add(item.Key);
        }
      }

      foreach (var item in removeList)
      {
        
        var itm = new BlockChangeInfo(item, new BlockValue(0u), true, false);
        var list = new List<BlockChangeInfo>();
        list.Add(itm);

        GameManager.Instance.SetBlocksRPC(list);
        GameManager.Instance.persistentPlayers.RemoveLandProtectionBlock(item);
        API.rlpQueue.Remove(item);
      }

      while (BMSanctuaries.DespawnQueue.Count > 0)
      {
        GameManager.Instance.World.RemoveEntity(BMSanctuaries.DespawnQueue.Dequeue(), EnumRemoveEntityReason.Despawned);
      }

      while (BMZombieFreeTime.DespawnQueue.Count > 0)
      {
        GameManager.Instance.World.RemoveEntity(BMZombieFreeTime.DespawnQueue.Dequeue(), EnumRemoveEntityReason.Despawned);
      }
    }
  }
}
