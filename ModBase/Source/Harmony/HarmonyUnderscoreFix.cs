using HarmonyLib;
using JetBrains.Annotations;

namespace Botman.Patches
{
    [HarmonyPatch(typeof(ClientInfoCollection), "GetForNameOrId")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]

    class HarmonyUnderscoreFix
    {
        public static bool Prefix([NotNull] ref ClientInfo __result, string _nameOrId)
        {
            ClientInfo clientInfo = null;
            int entityId;
            if (int.TryParse(_nameOrId, out entityId))
            {
                clientInfo = ClientCollection.GetClientInfoFromEntityId(entityId);
                if (clientInfo != null)
                {
                    __result = clientInfo;
                    return false;
                }
            }
            clientInfo = ClientCollection.GetClientInfoFromName(_nameOrId);
            if (clientInfo != null)
            {
                __result = clientInfo;
                return false;
            }
            if (_nameOrId.Contains("Local_") || _nameOrId.Contains("EOS_") || _nameOrId.Contains("Steam_") || _nameOrId.Contains("XBL_") || _nameOrId.Contains("PSN_") || _nameOrId.Contains("EGS_"))
            {
                PlatformUserIdentifierAbs userIdentifier;
                if (PlatformUserIdentifierAbs.TryFromCombinedString(_nameOrId, out userIdentifier))
                {
                    ClientInfo clientInfo3 = ClientCollection.GetClientInfoFromUId(userIdentifier);
                    if (clientInfo3 != null)
                    {
                        __result = clientInfo3;
                        return false;
                    }
                }
            }

            return false;
        }
    }
}