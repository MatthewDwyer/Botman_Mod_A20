using HarmonyLib;
using JetBrains.Annotations;
using System;

namespace Botman.Patches
{

  [HarmonyPatch(typeof(EntityBuffs), "FireEvent", new Type[] { typeof(MinEventTypes), typeof(BuffClass), typeof(MinEventParams) })]
  [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
  class BuffMonitor
  {
    public static void Prefix(EntityBuffs __instance, MinEventTypes _eventType, BuffClass _buffClass, MinEventParams _params)
    {
      if (!AntiCheat.Enabled)
      {
        return;
      }

      if (__instance == null)
      {
        return;
      }

      var _cInfo = ConnectionManager.Instance.Clients.ForEntityId(__instance.parent.entityId);
      
      if (_cInfo == null)
      {
        return;
      }

      var _player = GameManager.Instance.World.Players.dict[_cInfo.entityId];
      
      if (_player == null || !_player.IsSpawned() || !_player.IsAlive())
      {
        return;
      }

      try
      {
        var adminLevel = GameManager.Instance.adminTools.GetUserPermissionLevel(_cInfo);

        if (GameManager.Instance.adminTools.IsAdmin(_cInfo)) { return; }

        if (_player.IsSpectator)
        {
          if (!AntiCheat.Spectators.Contains($"--NAME:{_cInfo.playerName} --ID:{_cInfo.CrossplatformId} --LVL:{adminLevel}"))
          {
            AntiCheat.Spectators.Add($"--NAME:{_cInfo.playerName} --ID:{_cInfo.CrossplatformId} --LVL:{adminLevel}");
            Log.Out($"~Botman AntiCheat~ --NAME:{_cInfo.playerName} --ID:{_cInfo.CrossplatformId} --LVL:{adminLevel} has entered spectator mode.");
          }
        }
        else
        {
          if (AntiCheat.Spectators.Count > 0 && AntiCheat.Spectators.Contains($"NAME:{_cInfo.playerName} --ID:{_cInfo.CrossplatformId} --LVL:{adminLevel}"))
          {
            AntiCheat.Spectators.Remove($"--NAME:{_cInfo.playerName} --ID:{_cInfo.CrossplatformId} --LVL:{adminLevel}");
            Log.Out($"~Botman AntiCheat~ --NAME:{_cInfo.playerName} --ID:{_cInfo.CrossplatformId} --LVL:{adminLevel} has exited spectator mode.");
          }
        }

        if (_eventType == MinEventTypes.onSelfBuffUpdate)
        {
          if (_buffClass.Name == "god")
          {
            if (HasBuff(_cInfo, "god"))
            {
              if (!AntiCheat.GodMode.Contains($"--NAME:{_cInfo.playerName} --ID:{_cInfo.CrossplatformId} --LVL:{adminLevel}"))
              {
                AntiCheat.GodMode.Add($"--NAME:{_cInfo.playerName} --ID:{_cInfo.CrossplatformId} --LVL:{adminLevel}");
                Log.Out($"~Botman AntiCheat~ --NAME:{_cInfo.playerName} --ID:{_cInfo.CrossplatformId} --LVL:{adminLevel} has entered god mode.");
              }
            }
          }
        }

        if (_eventType == MinEventTypes.onSelfBuffRemove && _eventType != MinEventTypes.onSelfBuffUpdate)
        {
          if (_buffClass.Name == "god")
          {
            if (AntiCheat.GodMode.Count > 0 && AntiCheat.GodMode.Contains($"NAME:{_cInfo.playerName} --ID:{_cInfo.CrossplatformId} --LVL:{adminLevel}"))
            {
              AntiCheat.GodMode.Remove($"--NAME:{_cInfo.playerName} --ID:{_cInfo.CrossplatformId} --LVL:{adminLevel}");
              Log.Out($"~Botman AntiCheat~ --NAME:{_cInfo.playerName} --ID:{_cInfo.CrossplatformId} --LVL:{adminLevel} has exited god mode.");
            }
          }
        }
      }
      catch
      {

      }
    }

    public static bool HasBuff(ClientInfo _cInfo, string _buff)
    {
      //Strictly for verifying no false positives/
      var _player = GameManager.Instance.World.Players.dict[_cInfo.entityId];
      var buffs = _player.Buffs.ActiveBuffs;

      if (buffs != null)
      {
        foreach (var buff in buffs)
        {
          if (buff.BuffName.ToLower().Contains(_buff))
          {
            return true;
          }
        }
      }

      return false;
    }
  }
}
