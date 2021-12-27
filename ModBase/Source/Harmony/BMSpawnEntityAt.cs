using HarmonyLib;
using JetBrains.Annotations;

namespace Botman.Patches
{
  [HarmonyPatch(typeof(World), "SpawnEntityInWorld")]
  [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]

  class SpawnEntityInWorld
  {
    public static bool enabled = true;

    public static bool Prefix([NotNull] World __instance, Entity _entity)
    {
      if (!FallingBlocks.enabled) { return true; }
      if (_entity is EntityFallingBlock)
      {
        return false;
      }

      return true;
    }
  }
}
