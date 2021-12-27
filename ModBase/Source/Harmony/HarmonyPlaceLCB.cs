using Botman.Commands;
using HarmonyLib;
using JetBrains.Annotations;
using System.Collections.Generic;
using System;

namespace Botman.Patches
{
  [HarmonyPatch(typeof(PersistentPlayerList), "PlaceLandProtectionBlock")]
  [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]

  class PlaceLcbPatch
  {
    public static bool isEnabled = true;
    public static int TraderRadius = 275;
    public static int PrefabRadius = 75;
    public static List<Vector3i> BlocksToRemove = new List<Vector3i>();
    public static Vector3i LastPos;

    public static bool Prefix([NotNull] PersistentPlayerList __instance, Vector3i pos, PersistentPlayerData owner)
    {
      if (BMResetRegions.Enabled)
      {
        if (BMResetRegions.RemoveLCBs)
        {
          var _cInfo = ConnectionManager.Instance.Clients.ForEntityId(owner.EntityId);
          var _player = GameManager.Instance.World.Players.dict[_cInfo.entityId];
          //var _playerpos = new Vector3i(_player.position);

          if (GameManager.Instance.adminTools.IsAdmin(_cInfo)) { return false; }

          if (pos != LastPos)
          {
            if (LCBPlacement.IsWithinResetRegion(_player))
            {
              RemoveLCB(pos);
              SdtdConsole.Instance.ExecuteSync(string.Format("bm-give {0} keystoneBlock 1 1", _cInfo.entityId), null);
              SendMessage.Private(_cInfo, "LCB's are not allowed in reset regions.");
              LastPos = pos;
              return true;
            }

          }
        }
      }

      if (BMAreaReset.IsLCBWInResetArea(pos))
      {
        Log.Out($"Debug: LCB Placed in reset area.");
        var _cInfo = ConnectionManager.Instance.Clients.ForEntityId(owner.EntityId);
        if (GameManager.Instance.adminTools.IsAdmin(_cInfo)) { return false; }

        RemoveLCB(pos);
        SdtdConsole.Instance.ExecuteSync(string.Format("bm-give {0} keystoneBlock 1 1", _cInfo.entityId), null);
        SendMessage.Private(_cInfo, "LCB's are not allowed in reset areas.");
        return true;
      }

      if (LCBPlacement.PrefabRangeEnabled)
      {
        if (LCBPlacement.WithinPrefabRange(pos.x, pos.z))
        {
          var _cInfo = ConsoleHelper.ParseParamEntityIdToClientInfo(owner.EntityId.ToString());
          if (GameManager.Instance.adminTools.IsAdmin(_cInfo)) { return false; }
          SendMessage.Private(_cInfo, $"[f57b42]Land claims cannot be placed within {LCBPlacement.PrefabRangeSearch} blocks of a prefab.[-]");
          RemoveLCB(pos);
          SdtdConsole.Instance.ExecuteSync(string.Format("bm-give {0} keystoneBlock 1 1", _cInfo.entityId), null);
          Log.Out($"Returned LCB to {_cInfo.playerName}/{_cInfo.CrossplatformId}. Placed to close to a prefab.");
        }
      }
      return true;
    }


    public static void RemoveLCB(Vector3i _pos)
    {
      API.rlpQueue.Add(_pos, DateTime.UtcNow.Ticks + (TimeSpan.TicksPerSecond));
    }
  }
}
