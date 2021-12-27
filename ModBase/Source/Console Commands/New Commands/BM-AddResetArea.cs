using System;
using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMAreaReset : BMCmdAbstract
  {
    public static bool Enabled = false;
    public static bool MarkedForReset = false;
    public static int DaysBetweenReset = 3;
    public static readonly Dictionary<string, string> ResetAreas = new Dictionary<string, string>();
    private static readonly Dictionary<int, Vector3i> StoredLocations = new Dictionary<int, Vector3i>();

    public override string GetDescription() => "~Botman~ Adds areas to reset during reboots.";

    public override string[] GetCommands() => new[] { "bm-addresetarea" };

    public override string GetHelp() =>
      "Adds zones to be reset.\n" +
      "Usage:\n" +
      "1. bm-addresetarea enable/disable \n" +
      "2. bm-addresetarea [name] [xstart] [zstart] [xend] [zend] \n" +
      "3. bm-addresetarea delay [int] \n" +
      "  1. Turns tool on/off \n" +
      "  2. Resets terrain of the given world coordinate range. \n" +
      "  3. Sets days between resets. 0 = Every reset.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      switch (_params[0].ToLower())
      {
        case "enable":
          Enabled = true;
          Config.MapUpdateRequired = true;
          Config.UpdateXml();
          SdtdConsole.Instance.Output("Tool enabled");
          return;

        case "disable":
          Enabled = false;
          Config.MapUpdateRequired = true;
          Config.UpdateXml();
          SdtdConsole.Instance.Output("Tool disabled");
          return;

        case "delay" when _params.Count != 2:
          SdtdConsole.Instance.Output(GetHelp());
          return;

        case "delay":
          {
            if (!int.TryParse(_params[1], out var delay) || delay < 0 || delay > 100)
            {
              SdtdConsole.Instance.Output("Delay must be a number between 0-100");

              return;
            }

            DaysBetweenReset = delay;
            Config.UpdateXml();
            SdtdConsole.Instance.Output($"Delay between area reset has been set to {DaysBetweenReset}");
            CheckForReset();

            return;
          }

        case "p2" when _params.Count != 2:
          SdtdConsole.Instance.Output("Don't forget to name your reset area. \"bm-addresetarea p2 [name]\"");
          return;

        case "p2":
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
              SdtdConsole.Instance.Output("You must first begin with p1.");

              return;
            }

            var ppos = player.GetBlockPosition();
            var spos = StoredLocations[cInfo.entityId];

            AddToList(_params[1], spos.x, spos.z, ppos.x, ppos.z);
            Config.MapUpdateRequired = true;
            Config.UpdateXml();

            return;
          }

        case "p1" when _params.Count == 1:
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
            StoredLocations.Add(cInfo.entityId, player.GetBlockPosition());
            SdtdConsole.Instance.Output($"Stored position 1: {player.GetBlockPosition()}");

            return;
          }
      }

      if (_params.Count == 5)
      {
        if (!int.TryParse(_params[1], out var xStart))
        {
          SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"~Botman~ Wrong parameter for xstart: : {_params[1]}");

          return;
        }

        if (!int.TryParse(_params[2], out var zStart))
        {
          SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"~Botman~ Wrong parameter for zstart: {_params[2]}");

          return;
        }

        if (!int.TryParse(_params[3], out var xEnd))
        {
          SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"~Botman~ Wrong parameter for xend: {_params[3]}");

          return;
        }

        if (!int.TryParse(_params[4], out var zEnd))
        {
          SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"~Botman~ Wrong parameter for zend: {_params[4]}");

          return;
        }

        AddToList(_params[0], xStart, zStart, xEnd, zEnd);
        Config.MapUpdateRequired = true;
        Config.UpdateXml();

        return;
      }

      SdtdConsole.Instance.Output(GetHelp());
    }

    public static void AddToList(string areaName, int xStart, int zStart, int xEnd, int zEnd)
    {
      var area = $"x{xStart} z{zStart} xx{xEnd} zz{zEnd}";
      if (ResetAreas.ContainsKey(areaName))
      {
        SdtdConsole.Instance.Output($"{areaName} is already on the reset list.");

        return;
      }

      ResetAreas.Add(areaName, area);
      SdtdConsole.Instance.Output($"Added {areaName} as a reset area.");
    }

    public static void Run()
    {
      if (ResetAreas.Count == 0) { return; }

      foreach (var area in ResetAreas.Values)
      {
        var coords = area.Split(' ');
        if (coords.Length != 4 ||
            !int.TryParse(coords[0].Replace("x", ""), out var xStart) ||
            !int.TryParse(coords[1].Replace("z", ""), out var zStart) ||
            !int.TryParse(coords[2].Replace("xx", ""), out var xEnd) ||
            !int.TryParse(coords[3].Replace("zz", ""), out var zEnd))
        {
          return;
        }

        PrefabReset.ResetChunks(xStart, zStart, xEnd, zEnd);
      }
    }

    //public static void ResetChunks(int num, int num2, int num3, int num4)
    //{

    //  var x1 = num;
    //  var z1 = num2;
    //  var x2 = num3;
    //  var z2 = num4;

    //  if (x2 < x1)
    //  {
    //    var val = x1;
    //    x1 = x2;
    //    x2 = val;
    //  }
    //  if (z2 < z1)
    //  {
    //    var val = z1;
    //    z1 = z2;
    //    z2 = val;
    //  }
    //  var hashSetLong2 = new HashSetLong();
    //  var dic = new Dictionary<long, Chunk>();
    //  for (var k = x1; k <= x2; k++)
    //  {
    //    for (var l = z1; l <= z2; l++)
    //    {
    //      var chunk = GameManager.Instance.World.GetChunkFromWorldPos(k, -1, l) as Chunk;
    //      var chunkx = World.toChunkXZ(k);
    //      var chunkz = World.toChunkXZ(l);
    //      if (!hashSetLong2.Contains(WorldChunkCache.MakeChunkKey(chunkx, chunkz)))
    //      {
    //        hashSetLong2.Add(WorldChunkCache.MakeChunkKey(chunkx, chunkz));

    //      }

    //    }
    //  }
    //  var chunkCache = GameManager.Instance.World.ChunkCache;
    //  var chunkProviderGenerateWorld = GameManager.Instance.World.ChunkCache.ChunkProvider as ChunkProviderGenerateWorld;
    //  if (chunkProviderGenerateWorld != null)
    //  {
    //    chunkProviderGenerateWorld.RemoveChunks(hashSetLong2);
    //    foreach (var key in hashSetLong2)
    //    {
    //      if (!chunkProviderGenerateWorld.GenerateSingleChunk(chunkCache, key, true))
    //      {
    //        //SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Failed regenerating chunk at position {0}/{1}", WorldChunkCache.extractX(key) << 4, WorldChunkCache.extractZ(key) << 4));
    //      }
    //    }
    //    GameManager.Instance.World.m_ChunkManager.ResendChunksToClients(hashSetLong2);
    //  }
    //  Log.Out(string.Concat(new object[]
    //{
    //    "~Botman~ Reset area ",
    //    num,
    //    ",",
    //    num2,
    //    " to ",
    //    num3,
    //    ",",
    //    num4
    //}));
    //  return;
    //}

    public static void CheckForReset()
    {
      if (!Enabled)
      {
        MarkedForReset = false;

        return;
      }

      if (DaysBetweenReset == 0)
      {
        MarkedForReset = true;
        Log.Warning("~Botman~ Area Resets will be reset at end of this session.");

        return;
      }

      if (PersistentContainer.Instance.LastAreaReset == DateTime.MinValue)
      {
        PersistentContainer.Instance.LastAreaReset = DateTime.Now;
        PersistentContainer.Instance.Save();

        return;
      }

      if (DateTime.Now < PersistentContainer.Instance.LastAreaReset.AddDays(DaysBetweenReset)) { return; }

      MarkedForReset = true;
      Log.Warning("~Botman~ Areas will be reset at end of this session.");
      PersistentContainer.Instance.LastAreaReset = DateTime.Now;
      PersistentContainer.Instance.Save();
    }

    //private static void RegenerateChunk(Chunk chunk)
    //{
    //  if (GameManager.Instance.World == null)
    //  {
    //    Log.Warning("World not loaded!");

    //    return;
    //  }

    //  GameManager.Instance.World.RebuildTerrain(new HashSetLong { chunk.Key }, Vector3i.zero, Vector3i.zero, false, true, true);
    //}

    public static bool IsLCBWInResetArea(Vector3i position)
    {
      if (!Enabled) { return false; }

      foreach (var area in ResetAreas.Values)
      {
        var _params = area.Split(' ');
        if (_params.Length != 4 ||
            !int.TryParse(_params[0].Replace("x", ""), out var x1) ||
            !int.TryParse(_params[1].Replace("z", ""), out var z1) ||
            !int.TryParse(_params[2].Replace("xx", ""), out var x2) ||
            !int.TryParse(_params[3].Replace("zz", ""), out var z2))
        {
          return false;
        }

        Log.Out($"Placed @ x: {position.x} z: {position.z} Low x: {Math.Min(x1, x2)} Low z: { Math.Min(z1, z2)} High x: {Math.Max(x1, x2)} High z: {Math.Max(z1, z2)} ");

        if (position.x < Math.Min(x1, x2)) { continue; }

        if (position.x > Math.Max(x1, x2)) { continue; }

        if (position.z < Math.Min(z1, z2)) { continue; }

        if (position.z <= Math.Max(z1, z2)) { return true; }
      }

      return false;
    }
  }
}
