using HarmonyLib;
using JetBrains.Annotations;

namespace Botman.Patches
{
  [HarmonyPatch(typeof(Block), "OnBlockAdded")]
  [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]

  class AddChilds
  {
    public static bool Prefix(Block __instance, WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
    {
      if (!PrefabReset.ResettingPrefabs) return true;
      return !__instance.isMultiBlock;
    }
  }
}
