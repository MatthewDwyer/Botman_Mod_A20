using System;
using System.Collections.Generic;
using System.Threading;

namespace Botman
{
  public static class BMReload
  {
    public static int ReloadForClients(Dictionary<long, Chunk> chunks, PlatformUserIdentifierAbs CrossplatformId)
    {
      var world = GameManager.Instance.World;
      if (world == null) { return 0; }

      ResetStability(world, chunks);

      var clientChunks = new Dictionary<ClientInfo, List<long>>();

      foreach (var clientInfo in SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.List)
      {
        try
        {
          if (CrossplatformId != null && CrossplatformId != clientInfo.CrossplatformId) { continue; }

          if (!world.Entities.dict.ContainsKey(clientInfo.entityId)) { continue; }

          var entityPlayer = world.Entities.dict[clientInfo.entityId] as EntityPlayer;
          if (entityPlayer == null) { continue; }

          var chunksLoaded = entityPlayer.ChunkObserver.chunksLoaded;
          if (chunksLoaded == null) { continue; }

          foreach (var chunkKey in chunksLoaded)
          {
            if (!chunks.ContainsKey(chunkKey)) { continue; }

            if (clientChunks.ContainsKey(clientInfo))
            {
              clientChunks[clientInfo].Add(chunkKey);
            }
            else
            {
              clientChunks.Add(clientInfo, new List<long>
              {
                chunkKey
              });
            }

            clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChunkRemove>().Setup(chunkKey));
          }

          if (clientChunks.ContainsKey(clientInfo))
          {
            Log.Out($"~Botman~ Reloading Chunks: Reloading {clientChunks[clientInfo].Count}/{entityPlayer.ChunkObserver.chunksLoaded.Count} chunks for {clientInfo.playerName}");
          }
        }
        catch (Exception arg)
        {
          Log.Out($"~Botman~ Error removing chunks for {clientInfo.playerName}:\n{arg}");
        }
      }

      Thread.Sleep(50);

      foreach (var clientInfo in clientChunks.Keys)
      {
        try
        {
          if (clientChunks[clientInfo] == null) { continue; }

          var chunkCluster = world.ChunkClusters[0];
          if (chunkCluster == null) { continue; }

          var entityPlayer2 = world.Entities.dict[clientInfo.entityId] as EntityPlayer;
          if (entityPlayer2 == null) { continue; }

          var chunkKeysCopySync = chunkCluster.GetChunkKeysCopySync();
          foreach (var chunkKey in clientChunks[clientInfo])
          {
            if (!chunkKeysCopySync.Contains(chunkKey) || !entityPlayer2.ChunkObserver.chunksLoaded.Contains(chunkKey)) { continue; }

            var chunkSync = chunkCluster.GetChunkSync(chunkKey);
            if (chunkSync == null) { continue; }

            try
            {
              clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChunk>().Setup(chunkSync, false));
            }
            catch (Exception ex)
            {
              Log.Out($"~Botman~ Reloading Chunks: Error reloading chunk {chunkKey} for {clientInfo.playerName}:\n{ex}");
            }
          }
        }
        catch (Exception arg2)
        {
          Log.Out($"~Botman~ Error resending chunks for {clientInfo.playerName}:\n{arg2}");
        }
      }

      return clientChunks.Count;
    }

    public static void ResetStability(World world, Dictionary<long, Chunk> chunks)
    {
      var stabilityInitializer = new StabilityInitializer(world);
      foreach (var chunk in chunks.Values)
      {
        chunk?.ResetStability();
      }

      foreach (var chunk2 in chunks.Values)
      {
        if (chunk2 == null) { continue; }

        stabilityInitializer.DistributeStability(chunk2);
        chunk2.NeedsRegeneration = true;
        chunk2.NeedsLightCalculation = true;
      }
    }
  }
}
