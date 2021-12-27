using System;
using System.Collections.Generic;

namespace Botman.Commands
{
  class BMResetAllPrefabs : BMCmdAbstract
  {
    public static bool Enabled = false;
    public static int DaysBetweenReset = 0;
    public static bool MarkedForReset;

    public override string GetDescription() => "~Botman- Resets all prefab";

    public override string[] GetCommands() => new[] { "bm-resetallprefabs" };

    public override string GetHelp() =>
      "Usage:\n" +
      " 1. bm-resetallprefabs <enable/disable>\n" +
      " 2. bm-resetallprefabs delay <num>\n" +
      "   1. If enabled, all prefabs will be reset during reboot.\n" +
      "   2. Resets prefabs every x days at/after time of next scheduled reboot.\n" +
      " ** Delay by default is 0, Every reboot.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params[0].Equals("enable", StringComparison.InvariantCultureIgnoreCase))
      {
        SdtdConsole.Instance.Output("~Botman~ Auto Prefab reset has been enabled.");

        Enabled = true;
        CheckForReset();

        Config.UpdateXml();

        return;
      }
      if (_params[0].Equals("disable", StringComparison.InvariantCultureIgnoreCase))
      {
        //todo
        SdtdConsole.Instance.Output("~Botman~ Auto Prefab reset has been disabled.");

        Enabled = false;
        CheckForReset();

        PersistentContainer.Instance.LastPrefabReset = DateTime.MinValue;
        PersistentContainer.Instance.Save();

        Config.UpdateXml();

        return;
      }

      if (_params[0].ToLower().Equals("delay"))
      {
        if (_params.Count < 2)
        {
          SdtdConsole.Instance.Output("Delay param is required, must be a number between 0-100");

          return;
        }

        if (!int.TryParse(_params[1], out var delay) || delay < 0 || delay > 100)
        {
          SdtdConsole.Instance.Output("Delay must be a number between 0-100");
        }

        DaysBetweenReset = delay;

        SdtdConsole.Instance.Output($"Delay between prefab reset has been set to {DaysBetweenReset}");

        Config.UpdateXml();
        CheckForReset();

        return;
      }

      SdtdConsole.Instance.Output(GetHelp());
    }

    public static void Run()
    {
      foreach (var prefab in GameManager.Instance.GetDynamicPrefabDecorator().GetDynamicPrefabs())
      {
        try
        {
          if (PrefabReset.ExemptList.ContainsWithComparer(prefab.name, StringComparer.InvariantCultureIgnoreCase))
          {
            continue;
          }

          var center = Vectors.PrefabCenter(prefab);

          if (!GameManager.Instance.World.IsChunkAreaLoaded(center.x, center.y, center.z))
          {
            ResetChunks(
              prefab.boundingBoxPosition.x,
              prefab.boundingBoxPosition.z,
              prefab.boundingBoxPosition.x + prefab.boundingBoxSize.x + 1,
              prefab.boundingBoxPosition.z + prefab.boundingBoxSize.z + 1);
          }
          else
          {
            prefab.ResetBlocksAndRebuild(GameManager.Instance.World, 0);
          }
        }
        catch (Exception e)
        {
          SdtdConsole.Instance.Output("Could not reset prefab: " + e);
        }
      }

      SdtdConsole.Instance.Output("ALL PREFABS HAVE BEEN RESET");
    }

    public static void CheckForReset()
    {
      if (!Enabled)
      {
        MarkedForReset = false;
        return;
      }

	  if (DaysBetweenReset <= 0)
      {
        MarkedForReset = true;
        Log.Warning("~Botman~ Prefabs will be reset at end of this session.");

        return;
      }

      if (PersistentContainer.Instance.LastPrefabReset == DateTime.MinValue)
      {
        PersistentContainer.Instance.LastPrefabReset = DateTime.Now;
        PersistentContainer.Instance.Save();

        return;
      }

      if (DateTime.Now <= PersistentContainer.Instance.LastPrefabReset.AddDays(DaysBetweenReset)) { return; }
      MarkedForReset = true;
      Log.Warning("~Botman~ Prefabs will be reset at end of this session.");
      PersistentContainer.Instance.LastPrefabReset = DateTime.Now;
      PersistentContainer.Instance.Save();
    }

    public static void ResetChunks(int x1, int z1, int x2, int z2)
    {
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
      for (var k = x1; k <= x2; k++)
      {
        for (var l = z1; l <= z2; l++)
        {
          var chunkX = World.toChunkXZ(k);
          var chunkZ = World.toChunkXZ(l);
          if (!chunkKeys.Contains(WorldChunkCache.MakeChunkKey(chunkX, chunkZ)))
          {
            chunkKeys.Add(WorldChunkCache.MakeChunkKey(chunkX, chunkZ));
          }

        }
      }

      if (!(GameManager.Instance.World.ChunkCache.ChunkProvider is ChunkProviderGenerateWorld chunkProviderGenerateWorld)) { return; }

      chunkProviderGenerateWorld.RemoveChunks(chunkKeys);
      GameManager.Instance.World.m_ChunkManager.ResendChunksToClients(chunkKeys);
    }
  }
}
