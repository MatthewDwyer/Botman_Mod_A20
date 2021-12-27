using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Botman.Commands;
using UnityEngine;
using Harmony;
using JetBrains.Annotations;

namespace Botman.Patches
{

  [HarmonyPatch(typeof(GameManager), "SaveAndCleanupWorld")]
  [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
  public static class HarmonyResetRegions
  {
    private static string filepath = string.Format("{0}/Region", GameUtils.GetSaveGameDir());
    public static void Prefix(GameManager __instance)
    {
      if (BMResetAllPrefabs.enabled)
      {
        Log.Out("~Botman~ ================================");
        Log.Out("~Botman~ =======RESETING ALL PREFABS=====");
        Log.Out("~Botman~ ================================");
        BMResetAllPrefabs.Run();
        Log.Out("ALL PREFABS HAVE BEEN RESET.");
        return;
      }
      if (BMPrefabToRegion.Enabled && BMPrefabToRegion.PrefabsOnly)
      {
        if (BMPrefabToRegion.MarkedForReset)
        {
          Log.Out("~Botman~ ============================");
          Log.Out("~Botman~ =======RESETING PREFABS=====");
          Log.Out("~Botman~ ============================");
          BMPrefabToRegion.ResetPrefabs();
          Log.Out("Reset Prefabs complete.");
          return;
        }
      }
    }

    public static void Postfix(GameManager __instance)
    {
      Log.Out("Save World Done.");
      if (BMPrefabToRegion.Enabled && !BMPrefabToRegion.PrefabsOnly)
      {
        if (BMPrefabToRegion.MarkedForReset)
        {
          Log.Out("~Botman~ ============================");
          Log.Out("~Botman~ =======RESETING REGIONS=====");
          Log.Out("~Botman~ ============================");


          if (BMPrefabToRegion.ManualResetRegions.Count > 0)
          {
            foreach (string region in BMPrefabToRegion.ManualResetRegions)
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
    }
  }
}
