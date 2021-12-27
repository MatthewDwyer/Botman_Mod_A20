using System.Collections.Generic;
using UnityEngine;

namespace Botman
{
  class AntiCheat
  {
    public static List<string> Spectators = new List<string>();
    public static List<string> GodMode = new List<string>();

    public static bool Enabled = false;

    public static void Check(ClientInfo _cInfo)
    {
      //var _player = GameManager.Instance.World.Players.dict[_cInfo.entityId];

      //if (_player.IsSpectator)
      //{
      //  if (!Spectators.Contains($"NAME:{_cInfo.playerName} --ID:{_cInfo.playerId} --LVL:{_Admin.PermissionLevel.ToString()}"))
      //  {
      //    Spectators.Add($"NAME:{_cInfo.playerName} --ID:{_cInfo.playerId} --LVL:{_Admin.PermissionLevel.ToString()}");
      //    Log.Out($"~Botman AntiCheat~ --NAME:{_cInfo.playerName} --ID:{_cInfo.playerId} --LVL:{_Admin.PermissionLevel.ToString()} has entered spectator mode.");

      //  }
      //}
      //else
      //{
      //  if (Spectators.Contains($"NAME:{_cInfo.playerName} --ID:{_cInfo.playerId} --LVL:{_Admin.PermissionLevel.ToString()}"))
      //  {
      //    Spectators.Remove($"NAME:{_cInfo.playerName} --ID:{_cInfo.playerId} --LVL:{_Admin.PermissionLevel.ToString()}");
      //    Log.Out($"~Botman AntiCheat~ --NAME:{_cInfo.playerName} --ID:{_cInfo.playerId} --LVL:{_Admin.PermissionLevel.ToString()} has exited spectator mode.");
      //  }
      //}
      //if (IsGodMode(_cInfo))
      //{
      //  if (!GodMode.Contains($"NAME:{_cInfo.playerName} --ID:{_cInfo.playerId} --LVL:{_Admin.PermissionLevel.ToString()}"))
      //  {
      //    GodMode.Add($"NAME:{_cInfo.playerName} --ID:{_cInfo.playerId} --LVL:{_Admin.PermissionLevel.ToString()}");
      //    Log.Out($"~Botman AntiCheat~ --NAME:{_cInfo.playerName} --ID:{_cInfo.playerId} --LVL:{_Admin.PermissionLevel.ToString()} has entered god mode.");

      //  }
      //}
      //else
      //{
      //  if (GodMode.Contains(_cInfo.playerName))
      //  {
      //    GodMode.Remove($"NAME:{_cInfo.playerName} --ID:{_cInfo.playerId} --LVL:{_Admin.PermissionLevel.ToString()}");
      //    Log.Out($"~Botman AntiCheat~ --NAME:{_cInfo.playerName} --ID:{_cInfo.playerId} --LVL:{_Admin.PermissionLevel.ToString()} has exited god mode.");

      //  }
      //}
    }
    
    public static void Report()
    {
      if (Spectators.Count > 0)
      {
        foreach (var name in Spectators)
        {
          SdtdConsole.Instance.Output(name + " currently in spectator mode");
        }
      }

      if (GodMode.Count > 0)
      {
        foreach (var name in GodMode)
        {
          SdtdConsole.Instance.Output(name + " currently in god mode");
        }
      }

      SdtdConsole.Instance.Output("End Report");
    }

    public static bool IsGodMode(ClientInfo _cInfo)
    {
      var _player = GameManager.Instance.World.Players.dict[_cInfo.entityId];
      var buffs = _player.Buffs.ActiveBuffs;
      if (buffs != null)
      {
        foreach (var buff in buffs)
        {
          if (buff.BuffName.ToLower().Contains("god"))
          {
            return true;
          }
        }
      }

      return false;
    }

    public static void CheckCommand(ClientInfo _cInfo, string _cmd)
    {
      var _Admin = GameManager.Instance.adminTools.GetUserPermissionLevel(_cInfo);

      if (GameManager.Instance.adminTools.IsAdmin(_cInfo)) { return; }
      //var command = SingletonMonoBehaviour<SdtdConsole>.Instance.GetCommand(_cmd, false);
      //Log.Out("~Botman~ Console Monitor: " + _cInfo.playerName + " Ran Console Command: " + _cmd);
      if (_cmd.ToLower().StartsWith("spawnentityat"))
      {
        var flag = false;
        var _params = _cmd.Split(' ');
        if (_params.Length == 13)
        {
          // Command format: spawnentityat <entityidx> < x > < y > < z > < count > < rotX > < rotY > < rotZ > < stepX > < stepY > < stepZ > < spawnerType >
          var entityName = _params[1];


          float.TryParse(_params[2], out var x);
          float.TryParse(_params[3], out var y);
          float.TryParse(_params[4], out var z);
          var pos = new Vector3(x, y, z);
          int.TryParse(_params[5], out var count);
          var _entities = GameManager.Instance.World.Entities.list;
          foreach (var _entity in _entities)
          {
            if (entityName.ToLower().Contains(_entity.EntityClass.entityClassName.ToLower()))
            {
              if (_entity.position == pos || new Vector3i(_entity.position) == new Vector3i(pos))
              {
                flag = true;
              }
            }
          }
          if (flag)
          {
            Log.Out($"~Botman AntiCheat~ --NAME:{_cInfo.playerName} --ID:{_cInfo.CrossplatformId} --LVL:{_Admin.ToString()} spawned {count} {entityName}@{(int)x}, {(int)y}, {(int)z}.");
          }
        }
      }
    }
  }
}
