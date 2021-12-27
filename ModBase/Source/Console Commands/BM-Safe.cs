using System;
using System.Collections.Generic;
using System.Threading;

namespace Botman.Commands
{
  public class BMSafe : BMCmdAbstract
  {
    private static readonly List<string> SafeZones = new List<string>();
    private static readonly Dictionary<int, Vector3i> StoredLocations = new Dictionary<int, Vector3i>();

    public override string GetDescription() => "~Botman~ Set protection to a location in same way as trader.";

    public override string[] GetCommands() => new[] { "bm-safe" };

    public override string GetHelp() =>
      "Usage:\n" +
      "1. bm-safe <add/del> <x1> <z1> <x2> <z2>\n" +
      "2. bm-safe <add/del> <x>@<qnt> <y>@<qnt> <z>@<qnt>\n " +
      "3. bm-safe <add/del> <qnt> <qnt> <qnt>\n" +
      "4. bm-safe p1\n  " +
      "5. bm-safe p2 <add/del>\n" +
      "   1. protect or unprotect blocks from x1,y1,z1 to x2,y2,z2\n" +
      "   2. protect or unprotect blocks from x,y,z each quantity. Quantity can be positive or negative.\n" +
      "   3. protect or unprotect blocks from your position each quantity. Quantity can be positive or negative.\n" +
      "   4. Store your position to be used on method 5. p1 store player position\n" +
      "   5. protect or unprotect block from position stored on method 4 until your current location.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params.Count != 1 && _params.Count != 2 && _params.Count != 4 && _params.Count != 5 && _params.Count != 7)
      {
        SdtdConsole.Instance.Output($"Wrong number of arguments, expected 1 or 2 or 4 or 5 or 7, found {_params.Count}.");
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      var x1 = int.MinValue;
      var y1 = int.MinValue;
      var z1 = int.MinValue;
      var x2 = int.MinValue;
      var y2 = int.MinValue;
      var z2 = int.MinValue;
      var setProtected = true;
      var cInfo = _senderInfo.RemoteClientInfo;

      switch (_params.Count)
      {
        case 1 when _params[0].Equals("p1", StringComparison.InvariantCultureIgnoreCase):
          {
            //var cInfo = _senderInfo.RemoteClientInfo;
            if (cInfo == null)
            {
              SdtdConsole.Instance.Output("Unable to get your position");

              return;
            }

            var entityPlayer = GameManager.Instance.World.Players.dict[cInfo.entityId];
            if (entityPlayer == null)
            {
              SdtdConsole.Instance.Output("Unable to get your position");

              return;
            }

            if (StoredLocations.ContainsKey(cInfo.entityId))
            {
              StoredLocations.Remove(cInfo.entityId);
            }
            StoredLocations.Add(cInfo.entityId, entityPlayer.GetBlockPosition());
            SdtdConsole.Instance.Output($"Stored position: {entityPlayer.GetBlockPosition()}");

            return;
          }

        case 2 when !_params[0].Equals("p2", StringComparison.InvariantCultureIgnoreCase):
          SdtdConsole.Instance.Output("Invalid request. param 0 must be p2");
          SdtdConsole.Instance.Output(GetHelp());
          return;

        case 2:
          {
//            var cInfo = _senderInfo.RemoteClientInfo;
            if (cInfo == null)
            {
              SdtdConsole.Instance.Output("Unable to get your position");

              return;
            }

            var entityPlayer = GameManager.Instance.World.Players.dict[cInfo.entityId];
            if (entityPlayer == null)
            {
              SdtdConsole.Instance.Output("Unable to get your position");

              return;
            }

            if (!StoredLocations.TryGetValue(cInfo.entityId, out var position))
            {
              SdtdConsole.Instance.Output("There isn't any stored location. Use method 5. to store a position.");
              SdtdConsole.Instance.Output(GetHelp());

              return;
            }

            x1 = position.x;
            y1 = position.y;
            z1 = position.z;
            x2 = entityPlayer.GetBlockPosition().x;
            y2 = entityPlayer.GetBlockPosition().y;
            z2 = entityPlayer.GetBlockPosition().z;

            if (_params[1].Equals("add", StringComparison.InvariantCultureIgnoreCase))
            {
              setProtected = true;
            }
            else if (_params[1].Equals("del", StringComparison.InvariantCultureIgnoreCase))
            {
              setProtected = false;
            }
            else
            {
              SdtdConsole.Instance.Output("Invalid type of protection. If must be add or del.");
              SdtdConsole.Instance.Output(GetHelp());

              return;
            }

            break;
          }

        case 7:
        case 5:
          {
            switch (_params.Count)
            {
              case 7:
                if (!int.TryParse(_params[1], out x1) ||
                    !int.TryParse(_params[2], out x2) ||
                    !int.TryParse(_params[3], out y1) ||
                    !int.TryParse(_params[4], out y2) ||
                    !int.TryParse(_params[5], out z1) ||
                    !int.TryParse(_params[6], out z2))
                {
                  SdtdConsole.Instance.Output("Unable to parse some coordinates as numbers");

                  return;
                }

                break;
              case 5:
                if (!int.TryParse(_params[1], out x1) ||
                    !int.TryParse(_params[3], out x2) ||
                    !int.TryParse(_params[2], out z1) ||
                    !int.TryParse(_params[4], out z2))
                {
                  SdtdConsole.Instance.Output("Unable to parse some coordinates as numbers");

                  return;
                }

                y1 = 1;
                y2 = 10;

                break;
            }

            if (_params[0].Equals("add", StringComparison.InvariantCultureIgnoreCase))
            {
              setProtected = true;
            }
            else
            {
              if (!_params[0].Equals("del", StringComparison.InvariantCultureIgnoreCase))
              {
                SdtdConsole.Instance.Output("Invalid type of protection. If must be add or del.");
                SdtdConsole.Instance.Output(GetHelp());

                return;
              }

              setProtected = false;
            }

            break;
          }

        case 4:
          {
            var coordsX = _params[1].Split('@');
            var coordsY = _params[2].Split('@');
            var coordsZ = _params[3].Split('@');

            if (coordsX.Length == 1 && coordsY.Length == 1 && coordsZ.Length == 1)
            {
              //var cInfo = _senderInfo.RemoteClientInfo;
              if (cInfo == null)
              {
                SdtdConsole.Instance.Output("Unable to get your position");

                return;
              }

              var entityPlayer = GameManager.Instance.World.Players.dict[cInfo.entityId];
              if (entityPlayer == null)
              {
                SdtdConsole.Instance.Output("Unable to get your position");

                return;
              }

              x1 = entityPlayer.GetBlockPosition().x;
              y1 = entityPlayer.GetBlockPosition().y;
              z1 = entityPlayer.GetBlockPosition().z;

              if (!int.TryParse(_params[1], out x2) ||
                  !int.TryParse(_params[2], out y2) ||
                  !int.TryParse(_params[3], out z2) ||
                  x2 == 0 || y2 == 0 || z2 == 0)
              {
                SdtdConsole.Instance.Output("Quantity can not be 0.");

                return;
              }

              if (x2 == -1) { x2 = 1; }
              if (y2 == -1) { y2 = 1; }
              if (z2 == -1) { z2 = 1; }
              x2 = x2 < 0 ? x1 + x2 + 1 : x1 + x2 - 1;
              y2 = y2 < 0 ? y1 + y2 + 1 : y1 + y2 - 1;
              z2 = z2 < 0 ? z1 + z2 + 1 : z1 + z2 - 1;
            }
            else
            {
              if (coordsX.Length != 2 || coordsY.Length != 2 || coordsZ.Length != 2)
              {
                SdtdConsole.Instance.Output("Invalid Parameters.");
                SdtdConsole.Instance.Output(GetHelp());

                return;
              }

              if (!int.TryParse(coordsX[0], out x1) ||
                  !int.TryParse(coordsY[0], out y1) ||
                  !int.TryParse(coordsZ[0], out z1) ||
                  !int.TryParse(coordsX[1], out x2) ||
                  !int.TryParse(coordsY[1], out y2) ||
                  !int.TryParse(coordsZ[1], out z2) ||
                  x2 == 0 || y2 == 0 || z2 == 0)
              {
                SdtdConsole.Instance.Output("Invalid params, Quantity can not be 0.");

                return;
              }

              if (x2 == -1) { x2 = 1; }
              if (y2 == -1) { y2 = 1; }
              if (z2 == -1) { z2 = 1; }
              x2 = x2 < 0 ? x1 + x2 + 1 : x1 + x2 - 1;
              y2 = y2 < 0 ? y1 + y2 + 1 : y1 + y2 - 1;
              z2 = z2 < 0 ? z1 + z2 + 1 : z1 + z2 - 1;
            }
            if (_params[0].Equals("add", StringComparison.InvariantCultureIgnoreCase))
            {
              setProtected = true;
            }
            else
            {
              if (!_params[0].Equals("del", StringComparison.InvariantCultureIgnoreCase))
              {
                SdtdConsole.Instance.Output("Invalid type of protection. If must be add or del.");
                SdtdConsole.Instance.Output(GetHelp());

                return;
              }

              setProtected = false;
            }

            break;
          }
      }

      if (x1 == int.MinValue || y1 == int.MinValue || z1 == int.MinValue || x2 == int.MinValue || y2 == int.MinValue || z2 == int.MinValue)
      {
        SdtdConsole.Instance.Output("At least one of the given coordinates is not a valid integer");
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (x2 < x1)
      {
        var x = x1;
        x1 = x2;
        x2 = x;
      }

      if (y2 < y1)
      {
        var y = y1;
        y1 = y2;
        y2 = y;
      }

      if (z2 < z1)
      {
        var z = z1;
        z1 = z2;
        z2 = z;
      }

      var chunks = new Dictionary<long, Chunk>();
      for (var i = y1; i <= y2; i++)
      {
        for (var j = x1; j <= x2; j++)
        {
          for (var k = z1; k <= z2; k++)
          {
            if (!GameManager.Instance.World.IsChunkAreaLoaded(j, i, k) ||
                !(GameManager.Instance.World.GetChunkFromWorldPos(j, i, k) is Chunk chunk))
            {
              SdtdConsole.Instance.Output($"The filling block are to far away. Chunk not loaded at {j}, {k}");

              return;
            }

            if (!chunks.ContainsKey(chunk.Key))
            {
              chunks.Add(chunk.Key, chunk);
            }

            var bounds = chunk.GetAABB();
            chunk.SetTraderArea(j - (int)bounds.min.x, k - (int)bounds.min.z, setProtected);
          }
        }
      }

      Thread.Sleep(50);

      BMReload.ReloadForClients(chunks, cInfo.CrossplatformId);

      if (setProtected)
      {
        SdtdConsole.Instance.Output($"Blocks protected from {x1} {z1} to {x2} {z2}");

        if (!SafeZones.Contains($"{x1},{z1},{x2},{z2}"))
        {
          SafeZones.Add($"{x1},{z1},{x2},{z2}");
        }
      }
      else
      {
        SdtdConsole.Instance.Output($"Blocks unprotected from {x1} {z1} to {x2} {z2}");

        if (SafeZones.Contains($"{x1},{z1},{x2},{z2}"))
        {
          SafeZones.Remove($"{x1},{z1},{x2},{z2}");
        }
      }

      Config.UpdateXml();
    }
  }
}
