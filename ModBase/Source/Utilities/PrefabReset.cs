using System;
using System.Collections.Generic;
using System.Linq;
using Botman.Patches;
using HarmonyLib;
using UnityEngine;

namespace Botman
{
  class PrefabReset
  {
    public static List<string> ExemptList = new List<string>();
    public static bool ResettingPrefabs;
    public static void ResetAll()
    {
      ResettingPrefabs = true;
      var count = 0;
      var max = GameManager.Instance.GetDynamicPrefabDecorator().GetDynamicPrefabs().Count;
      var skipped = 0;
      var lcbReach = GamePrefs.GetInt(EnumGamePrefs.LandClaimSize) / 2;
      var announced = new List<int>();
      foreach (var prefab in GameManager.Instance.GetDynamicPrefabDecorator().GetDynamicPrefabs())
      {
        if (ExemptList.ContainsWithComparer(prefab.name, StringComparer.InvariantCultureIgnoreCase))
        {
          skipped++;

          continue;
        }

        var x1 = prefab.boundingBoxPosition.x;
        var x2 = prefab.boundingBoxPosition.x + prefab.boundingBoxSize.x;
        var z1 = prefab.boundingBoxPosition.z;
        var z2 = prefab.boundingBoxPosition.z + prefab.boundingBoxSize.z;
        ResetChunks(x1, z1, x2, z2);
        count++;
        var percentageDone = Math.Round((double)count / max * 100);
        if (percentageDone % 10 != 0) continue;
        if (announced.Contains((int)percentageDone)) continue;
        Log.Out($"~Botman~ Reset Status: {percentageDone} %");
        announced.Add((int)percentageDone);

      }
      SdtdConsole.Instance.Output(
        $"~Botman~ Live reset complete. {count} prefabs were reset and {skipped} prefabs were skipped.");
      ResettingPrefabs = false;
    }

    public static void ResetAtCoords(int x, int y, int z)
    {
      var lcbsToRemove = new List<Vector3i>();
      var lcbReach = GamePrefs.GetInt(EnumGamePrefs.LandClaimSize) / 2;
      var dynamicPrefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();
      PrefabInstance prefab;
      //var chunks = new Dictionary<long, Chunk>();
      //var hash = new HashSetLong();

      if (dynamicPrefabDecorator != null &&
          (prefab = dynamicPrefabDecorator.GetPrefabFromWorldPosInside(x, y, z)) != null)
      {
        lcbsToRemove.AddRange(from lcb in GameManager.Instance.persistentPlayers.m_lpBlockMap
          where lcb.Key.x >= prefab.boundingBoxPosition.x - lcbReach &&
                lcb.Key.z >= prefab.boundingBoxPosition.z - lcbReach &&
                lcb.Key.x <= (prefab.boundingBoxPosition.x + prefab.boundingBoxSize.x) + lcbReach &&
                lcb.Key.z <= prefab.boundingBoxPosition.z + prefab.boundingBoxSize.z + lcbReach
          select lcb.Key);
        ResetChunks(prefab.boundingBoxPosition.x, prefab.boundingBoxPosition.z, prefab.boundingBoxPosition.x + prefab.boundingBoxSize.x, prefab.boundingBoxPosition.z + prefab.boundingBoxSize.z);
        //prefab.Reset(GameManager.Instance.World);

        SdtdConsole.Instance.Output($"Reset prefab {prefab.name}");
      }

      //if (GameManager.Instance.World.GetChunkFromWorldPos(x, y, z) is Chunk chunk && !chunks.ContainsKey(chunk.Key))
      //{
      //  //chunks.Add(chunk.Key, chunk);
      //}
      if (lcbsToRemove.Count >= 1)
      {
        foreach (var lcb in lcbsToRemove)
        {
          PlaceLcbPatch.RemoveLCB(lcb);
        }
      }
      GameManager.Instance.World.m_ChunkManager.SendChunksToClients();

      //Reload(chunks);
    }

    public static void Check(int x, int y, int z)
    {
      var dynamicPrefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();
      PrefabInstance prefabFromWorldPosInside;

      if (dynamicPrefabDecorator != null &&
          (prefabFromWorldPosInside = dynamicPrefabDecorator.GetPrefabFromWorldPosInside(x, y, z)) != null)
      {
        SdtdConsole.Instance.Output(
          $"POI {prefabFromWorldPosInside.name} contains player home: {prefabFromWorldPosInside.CheckForAnyPlayerHome(GameManager.Instance.World) != GameUtils.EPlayerHomeType.None} Location: {Vectors.PrefabCenter(prefabFromWorldPosInside)} Region: {Vectors.GetPrefabRegion(Vectors.PrefabCenter(prefabFromWorldPosInside))} Center: {Vectors.PrefabCenter(prefabFromWorldPosInside)}");
      }
    }

    private static void Reload(Dictionary<long, Chunk> modifiedChunks)
    {
      //BMReload.ReloadForClients(modifiedChunks);
      //modifiedChunks.Clear();
    }

    public static void ResetChunks(int x1, int z1, int x2, int z2)
    {
      var lcbReach = GamePrefs.GetInt(EnumGamePrefs.LandClaimSize) / 2;
      if (x2 < x1)
      {
        var val = x1;
        x1 = x2;
        x2 = val;
      }

      if (z2 < z1)
      {
        var val = z1;
        z1 = z2;
        z2 = val;
      }

      HarmonyServerShutdown.LcbsToRemove.AddRange(from lcb in GameManager.Instance.persistentPlayers.m_lpBlockMap
        where lcb.Key.x >= x1 - lcbReach &&
              lcb.Key.z >= z1 - lcbReach &&
              lcb.Key.x <= x2 + lcbReach &&
              lcb.Key.z <= z2 + lcbReach
        select lcb.Key);

      var chunkKeys = new HashSetLong();

      for (var k = x1; k <= x2; k++)
      {
        for (var l = z1; l <= z2; l++)
        {
          var chunkX = World.toChunkXZ(k);
          var chunkZ = World.toChunkXZ(l);
          var chunkKey = WorldChunkCache.MakeChunkKey(chunkX, chunkZ);

          if (chunkKeys.Contains(chunkKey))
          {
            continue;
          }

          chunkKeys.Add(chunkKey);
        }
      }

      if (GameManager.Instance.World.ChunkCache.ChunkProvider is ChunkProviderGenerateWorld
        chunkProviderGenerateWorld)
      {
        chunkProviderGenerateWorld.RemoveChunks(chunkKeys);

       foreach (var key in chunkKeys)
        {
          if (!chunkProviderGenerateWorld.GenerateSingleChunk(GameManager.Instance.World.ChunkCache, key, true))
          {
            Log.Error(
              $"Failed regenerating chunk at position {WorldChunkCache.extractX(key) << 4}/{WorldChunkCache.extractZ(key) << 4}");
          }
        }

      }

      GameManager.Instance.World.m_ChunkManager.ResendChunksToClients(chunkKeys);
    }
  }

}
