using HarmonyLib;
using JetBrains.Annotations;

namespace Botman.Patches
{
  [HarmonyPatch(typeof(GameManager), "TELockServer")]
  [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]

  class BMOpenTileEntity
  {
    public static void Postfix([NotNull] GameManager __instance, int _clrIdx, Vector3i _blockPos, int _lootEntityId, int _entityIdThatOpenedIt)
    {
      if (AntiCheat.Enabled)
      {
        //Log.Out("indx: " + _clrIdx + " blockpos: " + _blockPos.ToString() + " lootid: " + _lootEntityId + " opened: " + _entityIdThatOpenedIt);
        var _cInfo = ConsoleHelper.ParseParamEntityIdToClientInfo(_entityIdThatOpenedIt.ToString());
        if (_cInfo == null)
        {
          return;
        }

        TileEntity tileEntity;
        if (_lootEntityId == -1)
        {
          tileEntity = __instance.World.GetTileEntity(_clrIdx, _blockPos);
        }
        else
        {
          tileEntity = __instance.World.GetTileEntity(_lootEntityId);
        }

        if (tileEntity == null)
        {
          return;
        }

        if (tileEntity is TileEntitySecureLootContainer)
        {
          //HandleOpenedSecureContainer((TileEntitySecureLootContainer)tileEntity, _cInfo);
          return;
        }

        if (tileEntity is TileEntitySign)
        {
          HandleOpenedSign((TileEntitySign)tileEntity, _cInfo);
        }
      }
    }

    public static void HandleOpenedSecureContainer(TileEntitySecureLootContainer _te, ClientInfo _cInfo)
    {
      if (_te.IsLocked())
      {
        var adminLevel = GameManager.Instance.adminTools.GetUserPermissionLevel(_cInfo);
        if (!_te.IsUserAllowed(_cInfo.CrossplatformId))
        {
          Log.Out($"~Botman AntiCheat~ --NAME:{_cInfo.playerName} --ID:{_cInfo.CrossplatformId} --LVL:{adminLevel} has opened an unauthorized locked container.");
        }
      }
    }

    public static void HandleOpenedSign(TileEntitySign _te, ClientInfo _cInfo)
    {
      if (_te.IsLocked())
      {
        var adminLevel = GameManager.Instance.adminTools.GetUserPermissionLevel(_cInfo);
        if (!_te.IsUserAllowed(_cInfo.CrossplatformId))
        {
          Log.Out($"~Botman AntiCheat~ --NAME:{_cInfo.playerName} --ID:{_cInfo.CrossplatformId} --LVL:{adminLevel} has opened an unauthorized locked sign.");
        }
      }
    }
  }
}
