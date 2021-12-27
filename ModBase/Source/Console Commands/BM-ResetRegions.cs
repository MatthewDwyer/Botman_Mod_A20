using System;
using System.Collections.Generic;
using System.IO;

namespace Botman.Commands
{
  class BMResetRegions : BMCmdAbstract
  {
    public static bool Enabled = false;
    public static bool PrefabsOnly = false;
    public static bool RemoveLCBs = true;
    public static int DaysBetweenReset = 3;
    public static bool MarkedForReset;

    public static readonly Dictionary<string, Dictionary<string, int>> PrefabsInRegion = new Dictionary<string, Dictionary<string, int>>();
    public static readonly Dictionary<string, Dictionary<string, int>> ManuallyAddedPrefabsInRegion = new Dictionary<string, Dictionary<string, int>>();
    public static readonly List<PrefabInstance> PrefabResetList = new List<PrefabInstance>();
    //public static readonly List<string> AutoResetRegions = new List<string>();
    public static readonly List<string> ManualResetRegions = new List<string>();
    //public static readonly List<string> ExemptRegions = new List<string>();
    public static readonly List<Vector3i> RespawnablePrefabs = new List<Vector3i>();

    private const string File = "DetailedList.xml";
    private static readonly string FilePath = Path.Combine(API.BotmanPath, File);

    public override string GetDescription() => "~Botman~ Reset Regions";

    public override string[] GetCommands() => new[] { "bm-resetregions" };

    public override string GetHelp() =>
      "Usage:\n" +
      "1. bm-resetregions enable/disable \n" +
      "2. bm-resetregions delay <num> \n" +
      "3. bm-resetregions list \n" +
      "4. bm-resetregions list detailed\n" +
      "5. bm-resetregions add \n" +
      "6. bm-resetregions add <x.z> \n" +
      "7. bm-resetregions add <playername/steamid> \n" +
      "8. bm-resetregions remove \n" +
      "9. bm-resetregions remove <x.z> \n" +
      "10. bm-resetregions remove <playername/steamid>\n" +
      "11. bm-resetregions clearall \n" +
      "12. bm-resetregions now \n" +
      "13. bm-resetregions removelcbs <true/false> \n" +
      "14. bm-resetregions prefabsonly <true/false> \n" +
      "   1. Enables/Disables Reset Regions. \n" +
      "   2. Sets the delay of days between resets. 0 for every reboot. \n" +
      "   3. Lists all reset regions. Filename only. \n" +
      "   4. Lists all reset regions and prefab names + count in region. \n" +
      "   5. Manually adds a region where the player is standing \n" +
      "   6. Manually adds a region based on the region number added. *Check webmap* i.e. r.0.0 = \"0.0\" \n" +
      "   7. Manually adds a regions based on where the specified player is standing. \n" +
      "   8. Removes region from list where player is standing. \n" +
      "   9. Removes region from list based on the region number added. *Check webmap* i.e. r.0.0 = \"0.0\" \n" +
      "   10. Removes region from list based on where specified player is standing. \n" +
      "   11. Removes all regions from the list. \n" +
      "   12. Forces a region reset immediately.\n" +
      "   13. If True Removes LCBs as they are placed and returns to player.\n" +
      "   14. If True Only prefabs reset. If False, entire region resets.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      var json = false;
      if (_params.Count > 0 && _params.Contains("/json"))
      {
        json = true;
        _params.Remove("/json");
      }

      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params[0].Equals("delay", StringComparison.InvariantCultureIgnoreCase))
      {
        if (_params.Count < 2)
        {
          SdtdConsole.Instance.Output("Delay is required and must be a number between 0-100");

          return;
        }

        if (!int.TryParse(_params[1], out var delay) || delay < 0 || delay > 100)
        {
          SdtdConsole.Instance.Output("Delay must be a number between 0-100");

          return;
        }

        DaysBetweenReset = delay;
        Config.UpdateXml();
        SdtdConsole.Instance.Output($"Delay between reset has been set to {DaysBetweenReset}");
        CheckForReset();

        return;
      }

      if (_params[0].Equals("enable", StringComparison.InvariantCultureIgnoreCase))
      {
        Enabled = true;
        Config.MapUpdateRequired = true;
        Config.UpdateXml();
        CheckForReset();
        SdtdConsole.Instance.Output("Reset Regions enabled. ");
        SdtdConsole.Instance.Output("Next reset will happen after: ");
        if (DaysBetweenReset <= 0 || MarkedForReset || PersistentContainer.Instance.LastReset.AddDays(DaysBetweenReset) < DateTime.Now)
        {
          SdtdConsole.Instance.Output("==This reboot==");
          return;
        }
        SdtdConsole.Instance.Output($"{PersistentContainer.Instance.LastReset.AddDays(DaysBetweenReset)}");
        return;
      }

      if (_params[0].Equals("disable", StringComparison.InvariantCultureIgnoreCase))
      {
        Enabled = false;
        Config.MapUpdateRequired = true;
        PersistentContainer.Instance.LastReset = DateTime.MinValue;
        PersistentContainer.Instance.Save();
        Config.UpdateXml();
        SdtdConsole.Instance.Output("Reset Regions disabled.");
        return;
      }

      if (_params[0].Equals("add", StringComparison.InvariantCultureIgnoreCase))
      {
        switch (_params.Count)
        {
          case 1:
            {
              var cInfo = ConsoleHelper.ParseParamIdOrName(_senderInfo.RemoteClientInfo.CrossplatformId.CombinedString);
              if (cInfo == null)
              {
                SdtdConsole.Instance.Output("You must be logged in to use this command.");

                return;
              }

              var player = GameManager.Instance.World.Players.dict[_senderInfo.RemoteClientInfo.entityId];
              if (player == null)
              {
                SdtdConsole.Instance.Output("Unable to find player entity.");

                return;
              }

              var region = OnlineChunk(player.GetBlockPosition());
              if (ManualResetRegions.Contains(region))
              {
                SdtdConsole.Instance.Output($"{region} is already marked as a reset region.");

                return;
              }

              ManualResetRegions.Add(region);
              ManualScanPrefabs(region);
              Config.MapUpdateRequired = true;
              Config.UpdateXml();
              SdtdConsole.Instance.Output($"Added {region} as a reset region.");

              return;
            }

          case 2:
            {
              PlatformUserIdentifierAbs steamId = PersistentContainer.Instance.Players.GetSteamId(_params[1]);
              if (steamId != null)
              {
                var cInfo = ConsoleHelper.ParseParamIdOrName(steamId.CombinedString, true, false);
                if (cInfo == null)
                {
                  SdtdConsole.Instance.Output($"Player with steam id {steamId} is not online");

                  return;
                }

                var player = GameManager.Instance.World.Players.dict[cInfo.entityId];
                if (player == null)
                {
                  SdtdConsole.Instance.Output("Unable to find player entity.");

                  return;
                }

                var region = OnlineChunk(player.GetBlockPosition());
                if (ManualResetRegions.Contains(region))
                {
                  SdtdConsole.Instance.Output($"{region} is already marked as a reset region.");

                  return;
                }

                ManualResetRegions.Add(region);
                Config.MapUpdateRequired = true;
                Config.UpdateXml();
                SdtdConsole.Instance.Output($"Added {region} as a reset region.");
                ManualScanPrefabs(region);

                return;
              }

              var reg = _params[1].Split('.');
              if (reg.Length != 2)
              {
                SdtdConsole.Instance.Output("Region must be in the form of 2 numbers i.e. \"1.0\"");

                return;
              }

              if (!int.TryParse(reg[0], out var x))
              {
                SdtdConsole.Instance.Output($"Unable to parse first parameter as a number: {reg[0]}");

                return;
              }

              if (!int.TryParse(reg[1], out var z))
              {
                SdtdConsole.Instance.Output($"Unable to parse second parameter as a number: {reg[1]}");

                return;
              }

              var region2 = $"r.{x}.{z}";
              if (ManualResetRegions.Contains(region2))
              {
                SdtdConsole.Instance.Output($"{region2} is already marked as a reset region.");

                return;
              }

              ManualResetRegions.Add(region2);
              ManualScanPrefabs(region2);
              Config.MapUpdateRequired = true;
              Config.UpdateXml();
              SdtdConsole.Instance.Output($"Added {region2} as a reset region.");

              return;
            }

          default:
            SdtdConsole.Instance.Output(GetHelp());
            return;
        }
      }

      if (_params[0].Equals("remove", StringComparison.InvariantCultureIgnoreCase))
      {
        switch (_params.Count)
        {
          case 1:
            {
              var cInfo = ConsoleHelper.ParseParamIdOrName(_senderInfo.RemoteClientInfo.CrossplatformId.CombinedString);
              if (cInfo == null)
              {
                SdtdConsole.Instance.Output("You must be logged in to use this command.");

                return;
              }

              var player = GameManager.Instance.World.Players.dict[_senderInfo.RemoteClientInfo.entityId];
              if (player == null)
              {
                SdtdConsole.Instance.Output("Unable to find player entity.");

                return;
              }

              var region = OnlineChunk(player.GetBlockPosition());
              if (ManualResetRegions.Contains(region))
              {
                ManualResetRegions.Remove(region);
                ManuallyAddedPrefabsInRegion.Remove(region);
                SdtdConsole.Instance.Output($"Removed {region} from the reset list.");
                Config.MapUpdateRequired = true;
                Config.UpdateXml();

                return;

              }

              SdtdConsole.Instance.Output($"{region} is not on the region reset list");
              return;
            }

          case 2:
            {
              PlatformUserIdentifierAbs steamId = PersistentContainer.Instance.Players.GetSteamId(_params[1]);
              if (steamId != null)
              {
                var cInfo = ConsoleHelper.ParseParamIdOrName(steamId.CombinedString, true, false);
                if (cInfo == null)
                {
                  SdtdConsole.Instance.Output($"Player with steam id {steamId} is not online");

                  return;
                }

                var player = GameManager.Instance.World.Players.dict[cInfo.entityId];
                if (player == null)
                {
                  SdtdConsole.Instance.Output("Unable to find player entity.");

                  return;
                }

                var region = OnlineChunk(player.GetBlockPosition());
                if (ManualResetRegions.Contains(region))
                {
                  ManualResetRegions.Remove(region);
                  ManuallyAddedPrefabsInRegion.Remove(region);
                  Config.MapUpdateRequired = true;
                  SdtdConsole.Instance.Output($"Removed {region} from the reset list.");
                  Config.UpdateXml();

                  return;
                }

                SdtdConsole.Instance.Output($"{region} is not on the region reset list");

                return;
              }

              var reg = _params[1].Split('.');
              if (reg.Length != 2)
              {
                SdtdConsole.Instance.Output("Region must be in the form of 2 numbers i.e. \"1.0\"");

                return;
              }

              if (!int.TryParse(reg[0], out var x))
              {
                SdtdConsole.Instance.Output($"Unable to parse first parameter as a number: {reg[0]}");

                return;
              }

              if (!int.TryParse(reg[1], out var z))
              {
                SdtdConsole.Instance.Output($"Unable to parse second parameter as a number: {reg[1]}");

                return;
              }

              var region2 = $"r.{x}.{z}";
              if (ManualResetRegions.Contains(region2))
              {
                ManualResetRegions.Remove(region2);
                ManuallyAddedPrefabsInRegion.Remove(region2);
                Config.MapUpdateRequired = true;
                SdtdConsole.Instance.Output($"Removed {region2} from the reset list.");
                Config.UpdateXml();

                return;
              }

              SdtdConsole.Instance.Output($"{region2} is not on the region reset list");

              return;
            }

          default:
            SdtdConsole.Instance.Output(GetHelp());
            return;
        }
      }

      if (_params[0].Equals("now", StringComparison.InvariantCultureIgnoreCase))
      {
        if (!Enabled)
        {
          SdtdConsole.Instance.Output("Reset Regions are currently disabled");

          return;
        }

        if (ManualResetRegions.Count == 0)
        {
          SdtdConsole.Instance.Output("There are currently no reset regions established");
          return;
        }

        MarkedForReset = true;
        SdtdConsole.Instance.ExecuteSync("kickall \"An admin has initiated a region reset. The server will be back momentarily\"", null);
        SdtdConsole.Instance.ExecuteSync("shutdown", null);

        return;
      }

      if (_params[0].Equals("list", StringComparison.InvariantCultureIgnoreCase))
      {
        if (_params.Count >= 2)
        {
          if (_params[1].Equals("detailed", StringComparison.InvariantCultureIgnoreCase))
          {
            if (json)
            {
              ListJsonDetailed();

              return;
            }

            if (_params.Count == 3)
            {
              if (_params[2].Equals("output", StringComparison.InvariantCultureIgnoreCase))
              {
                ListOff(true, true);

                return;
              }

              SdtdConsole.Instance.Output(GetHelp());

              return;
            }

            ListOff(true, false);

            return;
          }

          SdtdConsole.Instance.Output(GetHelp());

          return;
        }

        if (json)
        {
          ListJson();

          return;
        }

        ListOff(false, false);

        return;
      }

      if (_params[0].Equals("clearall", StringComparison.InvariantCultureIgnoreCase))
      {
        DumpPrefabs();
        Config.MapUpdateRequired = true;
        SdtdConsole.Instance.Output("All regions have been cleared.");
        Config.UpdateXml();

        return;
      }

      if (_params[0].Equals("removelcbs", StringComparison.InvariantCultureIgnoreCase))
      {
        if (_params.Count > 1 && _params[1].Equals("true", StringComparison.InvariantCultureIgnoreCase))
        {
          RemoveLCBs = true;
          SdtdConsole.Instance.Output("Enabled");
          Config.UpdateXml();

          return;
        }

        if (_params.Count > 1 && _params[1].Equals("false", StringComparison.InvariantCultureIgnoreCase))
        {
          RemoveLCBs = false;
          SdtdConsole.Instance.Output("Disabled");
          Config.UpdateXml();

          return;
        }

        SdtdConsole.Instance.Output("Subcommand removelcbs requires second param to be true or false");

        return;
      }

      if (_params[0].Equals("prefabsonly", StringComparison.InvariantCultureIgnoreCase))
      {
        if (_params.Count > 1 && _params[1].Equals("true", StringComparison.InvariantCultureIgnoreCase))
        {
          PrefabsOnly = true;
          SdtdConsole.Instance.Output("Only prefabs will be reset at the scheduled time.");

          return;
        }

        if (_params.Count > 1 && _params[1].Equals("false", StringComparison.InvariantCultureIgnoreCase))
        {
          PrefabsOnly = false;
          SdtdConsole.Instance.Output("Entire region will be reset during scheduled time.");

          return;
        }

        SdtdConsole.Instance.Output("Subcommand prefabsonly requires second param to be true or false");

        return;
      }

      SdtdConsole.Instance.Output(GetHelp());
    }

    public static void DumpPrefabs()
    {
      PrefabsInRegion.Clear();
      ManuallyAddedPrefabsInRegion.Clear();
      ManualResetRegions.Clear();
      Config.UpdateXml();
    }

    public static void CheckForReset()
    {
      if (!Enabled) { return; }

      if (DaysBetweenReset <= 0)
      {
        MarkedForReset = true;
        Log.Warning("~Botman~ Reset Regions will be reset at end of this session.");

        return;
      }

      if (PersistentContainer.Instance.LastReset == DateTime.MinValue)
      {
        PersistentContainer.Instance.LastReset = DateTime.Now;
        PersistentContainer.Instance.Save();

        return;
      }

      if (DateTime.Now < PersistentContainer.Instance.LastReset.AddDays(DaysBetweenReset)) { return; }

      MarkedForReset = true;
      Log.Warning("~Botman~ Reset Regions will be reset at end of this session.");
      PersistentContainer.Instance.LastReset = DateTime.Now;
      PersistentContainer.Instance.Save();
    }

    public static void ReloadPrefabsList()
    {
      foreach (var prefab in GameManager.Instance.GetDynamicPrefabDecorator().GetDynamicPrefabs())
      {
        if (prefab == null) { return; }

        var prefabName = prefab.name.Substring(0, prefab.name.IndexOf("."));
        var region = Vectors.GetPrefabRegion(Vectors.PrefabCenter(prefab));

        if (!ManualResetRegions.Contains(region)) { continue; }

        if (!ManuallyAddedPrefabsInRegion.ContainsKey(region))
        {
          var regionPrefabs = new Dictionary<string, int> { { prefabName, 1 } };
          ManuallyAddedPrefabsInRegion.Add(region, regionPrefabs);
          RespawnablePrefabs.Add(Vectors.PrefabCenter(prefab)); SdtdConsole.Instance.Output($"Added {prefab.name} at {Vectors.PrefabCenter(prefab)}");
        }
        else
        {
          var regionPrefabs = ManuallyAddedPrefabsInRegion[region];
          if (regionPrefabs.ContainsKey(prefab.name.Substring(0, prefab.name.IndexOf("."))))
          {
            regionPrefabs[prefab.name.Substring(0, prefab.name.IndexOf("."))]++;
            RespawnablePrefabs.Add(Vectors.PrefabCenter(prefab)); SdtdConsole.Instance.Output($"Added {prefab.name} at {Vectors.PrefabCenter(prefab)}");
          }
          else
          {
            regionPrefabs.Add(prefab.name.Substring(0, prefab.name.IndexOf(".")), 1);
            RespawnablePrefabs.Add(Vectors.PrefabCenter(prefab)); SdtdConsole.Instance.Output($"Added {prefab.name} at {Vectors.PrefabCenter(prefab)}");
          }
        }
      }
    }

    public static void ManualScanPrefabs(string scanRegion)
    {
      var prefabs = 0;
      foreach (var prefab in GameManager.Instance.GetDynamicPrefabDecorator().GetDynamicPrefabs())
      {
        var prefabName = prefab.name.Substring(0, prefab.name.IndexOf("."));
        var region = Vectors.GetPrefabRegion(Vectors.PrefabCenter(prefab));

        if (region != scanRegion) { continue; }

        if (!ManuallyAddedPrefabsInRegion.ContainsKey(region))
        {
          prefabs++;
          var regionPrefabs = new Dictionary<string, int> { { prefabName, 1 } };
          ManuallyAddedPrefabsInRegion.Add(region, regionPrefabs);
          RespawnablePrefabs.Add(Vectors.PrefabCenter(prefab));
        }
        else
        {
          var regionPrefabs = ManuallyAddedPrefabsInRegion[region];
          if (regionPrefabs.ContainsKey(prefab.name.Substring(0, prefab.name.IndexOf("."))))
          {
            prefabs++;
            regionPrefabs[prefab.name.Substring(0, prefab.name.IndexOf("."))]++;
            RespawnablePrefabs.Add(Vectors.PrefabCenter(prefab));
          }
          else
          {
            prefabs++;
            regionPrefabs.Add(prefab.name.Substring(0, prefab.name.IndexOf(".")), 1);
            RespawnablePrefabs.Add(Vectors.PrefabCenter(prefab));
          }
        }

        SdtdConsole.Instance.Output($"Added {prefabName} at {Vectors.PrefabCenter(prefab)}");
      }

      SdtdConsole.Instance.Output($"{prefabs} prefabs in added region.");
    }

    public static void ListOff(bool detailed, bool output)
    {
      var noRegionFound = true;
      if (output)
      {
        using (var sw = new StreamWriter(FilePath))
        {
          foreach (var region in ManualResetRegions)
          {

            if (region == "Empty") { continue; }

            sw.WriteLine(region);

            if (!detailed) { continue; }

            var prefabs = 0;

            if (!ManuallyAddedPrefabsInRegion.TryGetValue(region, out var regionPrefabs) ||
                ManuallyAddedPrefabsInRegion.Count <= 0) { continue; }

            foreach (var kvp in regionPrefabs)
            {
              sw.WriteLine($"  {kvp.Key},{kvp.Value}");
              prefabs += kvp.Value;
            }

            sw.WriteLine($"Total number of prefabs {prefabs}");
          }

          sw.Flush();
          sw.Close();
        }

        SdtdConsole.Instance.Output($"Output saved to {FilePath}");

        return;
      }

      foreach (var region in ManualResetRegions)
      {
        var prefabs = 0;

        if (region == "Empty") { continue; }

        SdtdConsole.Instance.Output(region);
        if (detailed)
        {
          if (ManuallyAddedPrefabsInRegion.Count > 0)
          {
            if (ManuallyAddedPrefabsInRegion.TryGetValue(region, out var regionPrefabs))
            {
              foreach (var kvp in regionPrefabs)
              {
                SdtdConsole.Instance.Output($"  {kvp.Key},{kvp.Value}");
                prefabs += kvp.Value;
              }
            }

            SdtdConsole.Instance.Output($"Total number of prefabs {prefabs}");
          }
        }
        noRegionFound = false;
      }

      if (noRegionFound)
      {
        SdtdConsole.Instance.Output("There are currently no reset regions.");

        return;
      }

      SdtdConsole.Instance.Output(!MarkedForReset
        ? $"Next Reset Scheduled for: {PersistentContainer.Instance.LastReset.AddDays(DaysBetweenReset)}"
        : "Next Reset Scheduled for: End of current session");
    }

    //todo: this will break if the chunk isn't loaded?
    public static string OnlineChunk(Vector3i position)
    {
      var region = RegionFile.ChunkXzToRegionXz(new Vector2i(World.toChunkXZ(position.x), World.toChunkXZ(position.z)));
      return $"r.{region.x}.{region.y}";
    }

    public static void ListJson()
    {
      var output = "{ ";
      output += $"\"count\" : \"{ManualResetRegions.Count}\" , \"regions\" : [ ";

      foreach (var region in ManualResetRegions)
      {
        output += $"\"{region}\" ,";
      }

      output = output.Substring(0, output.Length - 1);
      output += " ] }";

      SdtdConsole.Instance.Output(output);
    }

    public static void ListJsonDetailed()
    {
      var output = "{ ";
      output += $"\"count\" : \"{ManualResetRegions.Count}\" , \"regions\" : {{ ";

      foreach (var region in ManualResetRegions)
      {
        output += $"\"{region}\" : [ ";

        foreach (var dict in ManuallyAddedPrefabsInRegion[region])
        {
          output += $"\"{dict.Key} x{dict.Value}\" ,";
        }

        output += "] ,";
      }

      output = output.Substring(0, output.Length - 1);
      output += " } }";

      SdtdConsole.Instance.Output(output);
    }

    public static void ResetPrefabs()
    {
      if (!Enabled || !PrefabsOnly) { return; }

      foreach (var loc in RespawnablePrefabs)
      {
        var x = loc.x;
        var y = loc.y;
        var z = loc.z;

        var prefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();

        if (prefabDecorator == null)
        {
          Log.Out("~Botman Prefab Decorator is null");

          return;
        }

        var prefab = prefabDecorator.GetPrefabFromWorldPosInside(x, y, z);

        if (prefab == null)
        {
          Log.Out($"~Botman Reset prefab not found at {x} {y} {z}");
        }
        else
        {
          PrefabReset.ResetAtCoords(x, y, z);
          //prefab.Reset(GameManager.Instance.World);
          Log.Out($"~Botman~ Reset prefab at {x},{y},{z}");
        }
      }
    }

    public static void MarkMap()
    {
      foreach (var zone in ManualResetRegions)
      {

      }
    }

    public static void OnConfigChange()
    {

    }
  }
}
