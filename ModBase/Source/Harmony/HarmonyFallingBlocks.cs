using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace Botman.Patches
{
  [HarmonyPatch(typeof(World), "AddFallingBlock")]
  [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]

  class FallingBlocks
  {
    public static bool enabled;
    public static int BlocksToTrigger = 100;
    public static int reset = 5;
    public static Dictionary<string, double> Drops = new Dictionary<string, double>();
    public static Dictionary<string, DateTime> PlayerDropHistory = new Dictionary<string, DateTime>();

    public static bool Prefix([NotNull] World __instance, Vector3i _block)
    {
      if (!enabled) { return true; }
      
      var bv = GameManager.Instance.World.GetBlock(_block);
      
      if (bv.Block.GetBlockName() == "bedroll") { return true; }
      
      if (!bv.Block.StabilitySupport)
      {
        return true;
      }

      var item = new BlockChangeInfo(_block, new BlockValue(0u), true, false);
      var list = new List<BlockChangeInfo>();
      list.Add(item);
      GameManager.Instance.SetBlocksRPC(list);

      return false;
    }
  }
}
