using Botman.Commands;
using System;
using System.Collections.Generic;

namespace Botman
{
  class LCBPlacement
  {
    private static int _tick = 0;
    private static int _time = 3;
    public static int PrefabRangeSearch = 25;

    public static bool PrefabRangeEnabled = false;
    public static Dictionary<Vector3i, ClientInfo> BlocksToRemove = new Dictionary<Vector3i, ClientInfo>();

    public static bool IsWithinRestrictedPrefab(PrefabInstance prefab, ClientInfo cInfo)
    {
      var player = GameManager.Instance.World.Players.dict[cInfo.entityId];
      if (null == player) { return false; }
      if (player.IsAdmin) { return false; }

      var southWest = prefab.boundingBoxPosition;
      var northEast = Vectors.PrefabBoundingBoxPlusSize(prefab);
      var swX = southWest.x - 25;
      var swZ = southWest.z - 25;
      var neX = northEast.x + 25;
      var neZ = northEast.z + 25;
      var pos = player.GetBlockPosition();
      var x = pos.x;
      var z = pos.z;

      return x > swX && x < neX && z > swZ && z < neZ;
    }

    public static bool IsWithinResetRegion(EntityPlayer player)
    {
      if (player.IsAdmin) { return false; }
      var region = BMResetRegions.OnlineChunk(player.GetBlockPosition());

      return BMResetRegions.ManualResetRegions.Contains(region);
    }

    public static bool IsWithinTraderZone(PrefabInstance prefab, ClientInfo cInfo)
    {
      var player = GameManager.Instance.World.Players.dict[cInfo.entityId];
      if (null == player) { return false; }
      if (player.IsAdmin) { return false; }

      var southWest = prefab.boundingBoxPosition;
      var northEast = Vectors.PrefabBoundingBoxPlusSize(prefab);
      var swX = southWest.x - 75;
      var swZ = southWest.z - 75;
      var neX = northEast.x + 75;
      var neZ = northEast.z + 75;
      var pos = player.GetBlockPosition();
      var x = pos.x;
      var z = pos.z;

      return x > swX && x < neX && z > swZ && z < neZ;
    }

    public static void Update()
    {
      if (BlocksToRemove.Count > 0)
      {
        _tick++;
        if (_tick == _time)
        {
          foreach (var lcb in BlocksToRemove)
          {
            RemoveLcb(lcb.Key, lcb.Value);
          }
          BlocksToRemove.Clear();
          _tick = 0;
        }
      }
      else
      {
        _tick = 0;
      }
    }

    public static void RemoveLcb(Vector3i pos, ClientInfo cInfo)
    {
      if (GameManager.Instance.adminTools.IsAdmin(cInfo)) { return; }

      try
      {
        var persistentPlayerList = GameManager.Instance.GetPersistentPlayerList();
        var lpBlockMap = persistentPlayerList.m_lpBlockMap;
        if (!lpBlockMap.ContainsKey(pos))
        {
          return;
        }
        API.rlpQueue.Add(pos, DateTime.UtcNow.Ticks + (TimeSpan.TicksPerSecond));
      }
      catch
      {
        Log.Out($"High-Level Alert: Error Removing LCB For {cInfo.playerName} at {pos}");
      }

      try
      {
        SdtdConsole.Instance.ExecuteSync($"bm-give {cInfo.entityId} keystoneBlock 1 1", null);
      }
      catch
      {
        Log.Out($"High-Level Alert: Error Returning LCB To {cInfo.playerName} From {pos}");
      }
    }

    public static bool WithinPrefabRange(int x, int z)
    {
      foreach (var prefab in GameManager.Instance.GetDynamicPrefabDecorator().GetDynamicPrefabs())
      {
        var southWest = prefab.boundingBoxPosition;
        var northEast = Vectors.PrefabBoundingBoxPlusSize(prefab);
        var swX = southWest.x - PrefabRangeSearch;
        var swZ = southWest.z - PrefabRangeSearch;
        var neX = northEast.x + PrefabRangeSearch;
        var neZ = northEast.z + PrefabRangeSearch;

        if (x > swX && x < neX && z > swZ && z < neZ) { return true; }
      }

      return false;
    }
  }
}
