using HarmonyLib;
using JetBrains.Annotations;

namespace Botman.Patches
{
  [HarmonyPatch(typeof(ConnectionManager), "ServerConsoleCommand")]
  [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]

  class MonitorConsole
  {
    public static void Postfix([NotNull] ConnectionManager __instance, ClientInfo _cInfo, string _cmd)
    {
      if (AntiCheat.Enabled)
      {
        AntiCheat.CheckCommand(_cInfo, _cmd);
      }
    }
  }
}
