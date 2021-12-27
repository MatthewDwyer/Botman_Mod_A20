using System;
using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMSanctuaries : BMCmdAbstract
  {
    public static bool Enabled = false;

    // todo: this could be optimized by making a class instance and only allowing adding via method that does the min and max of the corners
    public static readonly Dictionary<string, SanctuaryData> Sanctuaries = new Dictionary<string, SanctuaryData>();
    public static readonly Queue<int> DespawnQueue = new Queue<int>();

    private static readonly Dictionary<string, Vector3i> StoredLocations = new Dictionary<string, Vector3i>();

    public override string GetDescription() => "~Botman~ Add, Remove or List sanctuaries";

    public override string[] GetCommands() => new[] { "bm-sanctuary", "bm-zone" };

    public override string GetHelp() =>
      "Control Sanctuaries -- These sanctuaries are only used to instantly kill zombies--\n" +
      "Usage:\n" +
      "1. bm-sanctuary p1\n" +
      "2. bm-sanctuary p2 <name>\n" +
      "3. bm-sanctuary add <name> <x1> <y1> <z1> <x2> <y2> <z2>\n" +
      "4. bm-sanctuary list\n" +
      "5. bm-sanctuary remove <name>\n" +
      "6. bm-sanctuary enable/disable\n" +
      "  1. store the first corner of sanctuary being created\n" +
      "  2. creates a sanctuary with specified name\n" +
      "  3. creates a sanctuary with specified name w/out having to move around\n" +
      "  4. lists all sanctuaries\n" +
      "  5. remove sanctuary with specified name\n" +
      "  5. turns sanctuaries on/off\n" +
      "*If adding a sanctuary from control panel/telnet, use command 3.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params.Count == 1)
      {
        switch (_params[0].ToLower())
        {
          case "enable":
            {
              Enabled = true;
              Config.ReloadConfigs();
              SdtdConsole.Instance.Output("Sanctuaries have been enabled.");
              return;
            }

          case "disable":
            {
              Enabled = false;
              Config.ReloadConfigs();
              SdtdConsole.Instance.Output("Sanctuaries have been disabled.");
              return;
            }

          case "p1":
            {
              if (_senderInfo.RemoteClientInfo == null)
              {
                SdtdConsole.Instance.Output("~Botman~ Unable to get your position");

                return;
              }

              var entityPlayer = GameManager.Instance.World.Players.dict[_senderInfo.RemoteClientInfo.entityId];
              if (entityPlayer == null)
              {
                SdtdConsole.Instance.Output("~Botman~ You must be in game to use this parameter.");

                return;
              }

              var pos = entityPlayer.GetBlockPosition();
              var playerId = _senderInfo.RemoteClientInfo.CrossplatformId.CombinedString;

              if (StoredLocations.ContainsKey(playerId))
              {
                StoredLocations.Remove(playerId);
              }

              StoredLocations.Add(playerId, pos);

              SdtdConsole.Instance.Output($"~Botman~ Position 1 saved at {pos.x} {pos.y} {pos.z}");

              return;
            }

          case "list":
            {
              if (Sanctuaries.Count > 0)
              {
                foreach (var kvp in Sanctuaries)
                {

                  SdtdConsole.Instance.Output($" Sanctuary Name: {kvp.Key} Point A: {kvp.Value.Corner1} Point B: {kvp.Value.Corner2}");
                }

                return;
              }

              SdtdConsole.Instance.Output("~Botman~ The are no sanctuaries established.");

              return;
            }

          case "p2":
            {
              SdtdConsole.Instance.Output("~Botman~ You must include a name for your sanctuary.");
              return;
            }

          default:
            SdtdConsole.Instance.Output($"Unknown param: {_params[0]}");
            SdtdConsole.Instance.Output(GetHelp());
            return;
        }
      }

      if (_params.Count == 2)
      {
        switch (_params[0].ToLower())
        {
          case "remove":
            {
              if (!Sanctuaries.ContainsKey(_params[1]))
              {
                SdtdConsole.Instance.Output($"~Botman~ Could not find {_params[1]} on the sanctuaries list");

                return;
              }

              Sanctuaries.Remove(_params[1]);
              Config.UpdateXml();

              SdtdConsole.Instance.Output($"~Botman~ Removed {_params[1]} from the sanctuaries list.");

              return;
            }

          case "p2":
            {
              if (_senderInfo.RemoteClientInfo == null)
              {
                SdtdConsole.Instance.Output("~Botman~ Unable to get your position");

                return;
              }

              var entityPlayer = GameManager.Instance.World.Players.dict[_senderInfo.RemoteClientInfo.entityId];
              if (entityPlayer == null)
              {
                SdtdConsole.Instance.Output("~Botman~ You must be in game to use this parameter.");

                return;
              }

              var playerId = _senderInfo.RemoteClientInfo.CrossplatformId.CombinedString;

              if (!StoredLocations.TryGetValue(playerId, out var corner1))
              {
                SdtdConsole.Instance.Output("~Botman~ Please set p1 before using p2.");

                return;
              }

              if (Sanctuaries.ContainsKey(_params[1]))
              {
                SdtdConsole.Instance.Output($"~Botman~ Cannot use {_params[1]} as a sanctuary name because it is already on the sanctuary list. Please try again with a different name");

                return;
              }

              Sanctuaries.Add(_params[1], new SanctuaryData
              {
                Corner1 = corner1,
                Corner2 = entityPlayer.GetBlockPosition()
              });

              Config.UpdateXml();
              StoredLocations.Remove(playerId);

              SdtdConsole.Instance.Output("~Botman~ Sanctuary added");

              return;
            }

          default:
            SdtdConsole.Instance.Output($"Unknown param: {_params[0]}");
            SdtdConsole.Instance.Output(GetHelp());
            return;
        }
      }

      if (_params.Count > 2)
      {
        switch (_params[0].ToLower())
        {
          case "p2":
            {
              SdtdConsole.Instance.Output("~Botman~ Names with spaces must include quotes around name specified.");
              return;
            }

          case "add":
            {
              if (_params.Count != 8)
              {
                SdtdConsole.Instance.Output(GetHelp());
                SdtdConsole.Instance.Output("~Botman~ You did not enter a correct parameter. Please try again.");

                return;
              }

              if (Sanctuaries.ContainsKey(_params[1]))
              {
                SdtdConsole.Instance.Output($"~Botman~ Cannot use {_params[1]} as a sanctuary name because it is already on the sanctuary list. Please try again with a different name");

                return;
              }

              if (!CmdHelpers.GetVector3i(_params[2], _params[3], _params[4], out var corner1)) { return; }
              if (!CmdHelpers.GetVector3i(_params[5], _params[6], _params[7], out var corner2)) { return; }

              Sanctuaries.Add(_params[1], new SanctuaryData
              {
                Corner1 = corner1,
                Corner2 = corner2
              });

              Config.UpdateXml();

              SdtdConsole.Instance.Output("~Botman~ Sanctuary added");

              return;
            }

          default:
            SdtdConsole.Instance.Output(GetHelp());
            SdtdConsole.Instance.Output("~Botman~ You did not enter a correct parameter. Please try again.");
            return;
        }
      }
    }

    public static void Update()
    {
      if (!Enabled) { return; }

      foreach (var entity in GameManager.Instance.World.Entities.list)
      {
        DespawnEntityInActiveSanctuaries(entity);
      }
    }

    public static void DespawnEntityInActiveSanctuaries(Entity entity)
    {
      if (!Enabled || Sanctuaries.Count <= 0 || entity == null || !(entity is EntityZombie)) { return; }

      var position = new Vector3i(entity.position);
      var entityId = entity.entityId;

      foreach (var sanctuary in Sanctuaries)
      {
        if (!IsPositionInSanctuary(position, sanctuary.Value)) { continue; }

        Log.Out($"~Botman~ Entity in active sanctuary queued for despawn {entityId} @ {entity.position} by sanctuary {sanctuary.Key}");

        DespawnQueue.Enqueue(entityId);

        return;
      }
    }

    private static bool IsPositionInSanctuary(Vector3i position, SanctuaryData sanctuary)
    {
      var xMin = Math.Min(sanctuary.Corner1.x, sanctuary.Corner2.x);
      var yMin = Math.Min(sanctuary.Corner1.y, sanctuary.Corner2.y);
      var zMin = Math.Min(sanctuary.Corner1.z, sanctuary.Corner2.z);

      var xMax = Math.Max(sanctuary.Corner1.x, sanctuary.Corner2.x);
      var yMax = Math.Max(sanctuary.Corner1.y, sanctuary.Corner2.y);
      var zMax = Math.Max(sanctuary.Corner1.z, sanctuary.Corner2.z);

      return position.x >= xMin &&
             position.y >= yMin &&
             position.z >= zMin &&
             position.x <= xMax &&
             position.y <= yMax &&
             position.z <= zMax;
    }
  }

  public class SanctuaryData
  {
    public Vector3i Corner1;
    public Vector3i Corner2;
  }
}
