using System;
using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMChunkReset : BMCmdAbstract
  {
    private static readonly Dictionary<int, Vector3i> StoredLocations = new Dictionary<int, Vector3i>();

    public override string GetDescription() => "~Botman~ Resets Chunk around a player/coords.";

    public override string[] GetCommands() => new[] { "bm-chunkreset" };

    public override string GetHelp() =>
      "Resets chunks around a player.\n" +
      "Usage:\n" +
      "1. bm-chunkreset p1 \n" +
      "2. bm-chunkreset p2 \n" +
      "3. bm-chunkreset [playername/id] \n" +
      "4. bm-chunkreset xstart zstart xend zend \n" +
      "  1. Stores current position as start point. \n" +
      "  2. Uses stored position from p1 and reset to this area. \n" +
      "  3. Resets current chunk player is standing in. \n" +
      "  4. Resets terrain of the given world coordinate range.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count != 1 && _params.Count != 4)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params.Count == 1)
      {
        if (_params[0].Equals("p1", StringComparison.InvariantCultureIgnoreCase))
        {
          var cInfo = _senderInfo.RemoteClientInfo;
          if (cInfo == null)
          {
            SdtdConsole.Instance.Output("Unable to get your position");

            return;
          }

          var player = GameManager.Instance.World.Players.dict[cInfo.entityId];
          if (player == null)
          {
            SdtdConsole.Instance.Output("Unable to get your position");

            return;
          }

          if (StoredLocations.ContainsKey(cInfo.entityId))
          {
            StoredLocations.Remove(cInfo.entityId);
          }

          var pos = player.GetBlockPosition();
          StoredLocations.Add(cInfo.entityId, pos);
          SdtdConsole.Instance.Output($"Stored position: {pos.x} {pos.y} {pos.z}");

          return;
        }
        else if (_params[0].Equals("p2", StringComparison.InvariantCultureIgnoreCase))
        {
          var cInfo = _senderInfo.RemoteClientInfo;
          if (cInfo == null)
          {
            SdtdConsole.Instance.Output("Unable to get your position");

            return;
          }

          var player = GameManager.Instance.World.Players.dict[cInfo.entityId];
          if (player == null)
          {
            SdtdConsole.Instance.Output("Unable to get your position");

            return;
          }

          if (!StoredLocations.ContainsKey(cInfo.entityId))
          {
            SdtdConsole.Instance.Output("There isn't any stored location.");

            return;
          }

          if (!StoredLocations.TryGetValue(cInfo.entityId, out var storedPos))
          {
            return;
          }

          var pos = player.GetBlockPosition();
          ResetChunks(storedPos.x, storedPos.z, pos.x, pos.z);
          StoredLocations.Remove(cInfo.entityId);

          return;
        }
        else
        {
          var cInfo = ConsoleHelper.ParseParamIdOrName(_params[0], true, true);
          if (cInfo == null)
          {
            SdtdConsole.Instance.Output("Unable to find player");

            return;
          }

          var player = GameManager.Instance.World.Players.dict[cInfo.entityId];
          if (player == null)
          {
            SdtdConsole.Instance.Output("Unable to find player");

            return;
          }

          var pos = player.GetBlockPosition();
          ResetChunks(pos.x, pos.z, pos.x, pos.z);
        }

        return;
      }

      if (!int.TryParse(_params[0], out var x1))
      {
        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"~Botman~ Wrong parameter for xstart {_params[0]}");

        return;
      }

      if (!int.TryParse(_params[1], out var z1))
      {
        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"~Botman~ Wrong parameter for zstart {_params[1]}");

        return;
      }

      if (!int.TryParse(_params[2], out var x2))
      {
        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"~Botman~ Wrong parameter for xend {_params[2]}");

        return;
      }

      if (!int.TryParse(_params[3], out var z2))
      {
        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"~Botman~ Wrong parameter for zend {_params[3]}");

        return;
      }

      ResetChunks(x1, z1, x2, z2);
    }

    public static void ResetChunks(int x1, int z1, int x2, int z2)
    {
      var chunkCache = GameManager.Instance.World.ChunkCache;

      if (!(chunkCache.ChunkProvider is ChunkProviderGenerateWorld chunkProvider) || GameManager.Instance.World == null)
      {
        return;
      }

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

      var chunkKeys = new HashSetLong();
      var chunks = new Dictionary<long, Chunk>();
      for (var k = x1; k <= x2; k++)
      {
        for (var l = z1; l <= z2; l++)
        {
          if (!(GameManager.Instance.World.GetChunkFromWorldPos(k, -1, l) is Chunk chunk)) { continue; }

          var chunkX = World.toChunkXZ(k);
          var chunkZ = World.toChunkXZ(l);
          if (!chunkKeys.Contains(WorldChunkCache.MakeChunkKey(chunkX, chunkZ)))
          {
            chunkKeys.Add(WorldChunkCache.MakeChunkKey(chunkX, chunkZ));
            chunkProvider.GenerateSingleChunk(chunkCache, chunk.Key, true);
          }
          if (!chunks.ContainsKey(chunk.Key))
          {
            chunks.Add(chunk.Key, chunk);
          }
        }
      }

      BMReload.ReloadForClients(chunks);
      SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"~Botman~ Reset area {x1},{z1} to {x2},{z2}");
    }
  }
}
