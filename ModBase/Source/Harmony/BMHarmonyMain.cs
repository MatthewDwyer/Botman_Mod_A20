using HarmonyLib;
using System;

namespace Botman
{
  class BMHarmonyMain
  {
    public static void ApplyPatches()
    {
      try
      {
        var harmony = new Harmony("com.Botman");
        harmony.PatchAll();
      }
      catch (Exception e)
      {
        Log.Out("~Botman~ Error applying harmony patches " + e);
      }
    }
  }
}
