using Botman.Commands;
using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Botman.Patches
{
  [HarmonyPatch(typeof(GameManager), "SaveAndCleanupWorld")]
  [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
  public static class HarmonyServerShutdown
  {
    private static string filepath = string.Format("{0}/Region", GameIO.GetSaveGameDir());
    public static List<Vector3i> LcbsToRemove = new List<Vector3i>();
    public static void Prefix(GameManager __instance)
    {
      if (BMResetAllPrefabs.Enabled && BMResetAllPrefabs.MarkedForReset)
      {
        Log.Out("~Botman~ ================================");
        Log.Out("~Botman~ =======RESETTING ALL PREFABS=====");
        Log.Out("~Botman~ ================================");
        PrefabReset.ResetAll();
        Log.Out("ALL PREFABS HAVE BEEN RESET.");
        return;
      }

      if (BMAreaReset.Enabled && BMAreaReset.MarkedForReset)
      {
        Log.Out("~Botman~ ================================");
        Log.Out("~Botman~ =======RESETTING RESET AREAS=====");
        Log.Out("~Botman~ ================================");
        BMAreaReset.Run();
        Log.Out("ALL RESET AREAS HAVE BEEN RESET.");
        return;
      }

      if (BMResetRegions.Enabled && BMResetRegions.MarkedForReset)
      {
        if (BMResetRegions.PrefabsOnly)
        {

          Log.Out("~Botman~ ============================");
          Log.Out("~Botman~ =======RESETTING REGIONS====");
          Log.Out("~Botman~ ============================");
          BMResetRegions.ResetPrefabs();
          Log.Out("ALL RESET REGIONS HAVE BEEN RESET.");
        }
        else
        {
          foreach (var lcb in from lcb in GameManager.Instance.persistentPlayers.m_lpBlockMap let pos = BMResetRegions.OnlineChunk(lcb.Key) where BMResetRegions.ManualResetRegions.Contains(pos) select lcb)
          {
            LcbsToRemove.Add(lcb.Key);
          }
        }
      }

      if (LcbsToRemove.Count >= 1)
      {
        foreach (var lcb in LcbsToRemove)
        {
          PlaceLcbPatch.RemoveLCB(lcb);
        }
      }
    }

    public static void Postfix(GameManager __instance)
    {
      Log.Out("Save World Done.");
      if (BMResetRegions.Enabled && !BMResetRegions.PrefabsOnly)
      {
        if (BMResetRegions.MarkedForReset)
        {
          Log.Out("~Botman~ ============================");
          Log.Out("~Botman~ =======RESETTING REGIONS=====");
          Log.Out("~Botman~ ============================");


          if (BMResetRegions.ManualResetRegions.Count > 0)
          {
            foreach (var region in BMResetRegions.ManualResetRegions)
            {
              if (region == "Empty")
              {
                continue;
              }
              try
              {

                if (!File.Exists($"{filepath}/{region}.7rg"))
                {
                  Log.Out($"~Botman~ {region}.7rg not found. Already Reset.");
                  continue;
                }
                else
                {
                  Log.Out($"~Botman~ Region {region} RESET");
                  File.Delete($"{filepath}/{region}.7rg");
                }
              }
              catch (Exception e)
              {
                Log.Out($"~Botman Notice~ Error Resetting Region {region}: {e}");
                continue;
              }
            }
          }
        }
      }
      if (BMVehicleFileDelete.Enabled)
      {
        var vehiclesFile = string.Format("{0}/vehicles.dat", GameIO.GetSaveGameDir());
        if (!File.Exists(vehiclesFile))
        {
          Log.Out("~Botman~ Could not locate vehicles file.");

          return;
        }

        File.Delete(vehiclesFile);
        Log.Out("~Botman~ VEHICLES HAVE BEEN RESET.");
      }
    }
  }
}
