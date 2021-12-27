using System;
using System.Collections.Generic;
using System.Threading;

namespace Botman.Commands
{
  public class BMPrefabDupe : BMCmdAbstract
  {
    private static Dictionary<int, Vector3i> location1 = new Dictionary<int, Vector3i>();
    private static Dictionary<int, Vector3i> location2 = new Dictionary<int, Vector3i>();

    public override string GetDescription() => "~Botman~ Copy an Area to another location";

    public override string[] GetCommands() => new[] { "bm-pdup" };

    public override string GetHelp() =>
      "Usage:\n" +
      "  1. bm-pdup <x1> <x2> <y1> <y2> <z1> <z2> <x> <y> <z> <rot>\n" +
      "  2. bm-pdup p1\n" +
      "  3. bm-pdup p2\n" +
      "  4. bm-pdup <x> <y> <z> <rot>\n" +
      "  5. bm-pdup <rot>\n" +
      "1. duplicate the defined area on x,y,z\n" +
      "2. Store on position 1 your current location\n" +
      "3. Store on position 2 your current location\n" +
      "4. use stored position 1 and 2 to duplicate on x,y,z\n" +
      "5. use stored position 1 and 2 to duplicate on your current location\n" +
      "<rot> prefab rotation -> need to be equal 0,1,2 or 3\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      var x1 = int.MinValue;
      var y1 = int.MinValue;
      var z1 = int.MinValue;

      var x2 = int.MinValue;
      var y2 = int.MinValue;
      var z2 = int.MinValue;

      var x = int.MinValue;
      var y = int.MinValue;
      var z = int.MinValue;

      var rot = int.MinValue;

      if (_params.Count != 10 && _params.Count != 1 && _params.Count != 4)
      {
        SdtdConsole.Instance.Output($"Wrong number of arguments, expected 10, found {_params.Count}.");
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params.Count == 1)
      {
        if (_params[0].Equals("p1", StringComparison.InvariantCultureIgnoreCase))
        {
          var cInfo = _senderInfo.RemoteClientInfo;
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

          if (location1.ContainsKey(cInfo.entityId))
          {
            location1.Remove(cInfo.entityId);
          }

          location1.Add(cInfo.entityId, entityPlayer.GetBlockPosition());
          SdtdConsole.Instance.Output($"Stored position 1: {entityPlayer.GetBlockPosition()}");

          return;
        }

        if (_params[0].Equals("p2", StringComparison.InvariantCultureIgnoreCase))
        {
          var cInfo = _senderInfo.RemoteClientInfo;
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

          if (location2.ContainsKey(cInfo.entityId))
          {
            location2.Remove(cInfo.entityId);
          }

          location2.Add(cInfo.entityId, entityPlayer.GetBlockPosition());
          SdtdConsole.Instance.Output($"Stored position 2: {entityPlayer.GetBlockPosition()}");

          return;
        }

        var cInfo2 = _senderInfo.RemoteClientInfo;
        if (cInfo2 == null)
        {
          SdtdConsole.Instance.Output("Unable to get your position");

          return;
        }

        var entityPlayer2 = GameManager.Instance.World.Players.dict[cInfo2.entityId];
        if (entityPlayer2 == null)
        {
          SdtdConsole.Instance.Output("Unable to get your position");

          return;
        }

        if (!location1.ContainsKey(cInfo2.entityId))
        {
          SdtdConsole.Instance.Output("There isn't any stored location 1. Use method 2. to store a position.");
          SdtdConsole.Instance.Output(GetHelp());

          return;
        }

        if (!location2.ContainsKey(cInfo2.entityId))
        {
          SdtdConsole.Instance.Output("There isn't any stored location 2. Use method 3. to store a position.");
          SdtdConsole.Instance.Output(GetHelp());

          return;
        }

        if (!int.TryParse(_params[0], out rot))
        {
          SdtdConsole.Instance.Output("Unable to parse rot as a number.");

          return;
        }

        location1.TryGetValue(cInfo2.entityId, out var storedPos1);

        x1 = storedPos1.x;
        y1 = storedPos1.y;
        z1 = storedPos1.z;

        location2.TryGetValue(cInfo2.entityId, out var storedPos2);

        x2 = storedPos2.x;
        y2 = storedPos2.y;
        z2 = storedPos2.z;

        var pos = entityPlayer2.GetBlockPosition();
        x = pos.x;
        y = pos.y;
        z = pos.z;
      }
      else if (_params.Count == 4)
      {
        var cInfo = _senderInfo.RemoteClientInfo;
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

        if (!location1.ContainsKey(cInfo.entityId))
        {
          SdtdConsole.Instance.Output("There isn't any stored location 1. Use method 2. to store a position.");
          SdtdConsole.Instance.Output(GetHelp());

          return;
        }

        if (!location2.ContainsKey(cInfo.entityId))
        {
          SdtdConsole.Instance.Output("There isn't any stored location 2. Use method 3. to store a position.");
          SdtdConsole.Instance.Output(GetHelp());

          return;
        }

        if (!int.TryParse(_params[1], out x) ||
            !int.TryParse(_params[2], out y) ||
            !int.TryParse(_params[3], out z) ||
            !int.TryParse(_params[4], out rot))
        {
          SdtdConsole.Instance.Output("Unable to parse x y z or rot as a number");

          return;
        }

        location1.TryGetValue(cInfo.entityId, out var storedPos1);
        
        x1 = storedPos1.x;
        y1 = storedPos1.y;
        z1 = storedPos1.z;

        location2.TryGetValue(cInfo.entityId, out var storedPos2);
        
        x2 = storedPos2.x;
        y2 = storedPos2.y;
        z2 = storedPos2.z;
      }
      else if (_params.Count == 10)
      {
        if (!int.TryParse(_params[0], out x1) ||
            !int.TryParse(_params[2], out y1) ||
            !int.TryParse(_params[4], out z1))
        {
          SdtdConsole.Instance.Output("Unable to parse x y z as a number");

          return;
        }

        if (!int.TryParse(_params[1], out x2) ||
            !int.TryParse(_params[3], out y2) ||
            !int.TryParse(_params[5], out z2))
        {
          SdtdConsole.Instance.Output("Unable to parse x2 y2 z2 as a number");

          return;
        }

        if (!int.TryParse(_params[9], out rot))
        {
          SdtdConsole.Instance.Output("Unable to parse rot as a number");

          return;
        }
      }

      if (rot < 0 || rot > 3)
      {
        SdtdConsole.Instance.Output("Invalid rotation parameter. It need to be 0,1,2 or 3");
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      //Fix the order of xyz1 xyz2
      if (x2 < x1)
      {
        var val = x1;
        x1 = x2;
        x2 = val;
      }

      if (y2 < y1)
      {
        var val = y1;
        y1 = y2;
        y2 = val;
      }

      if (z2 < z1)
      {
        var val = z1;
        z1 = z2;
        z2 = val;
      }

      if (x1 == int.MinValue || y1 == int.MinValue || z1 == int.MinValue || x2 == int.MinValue || y2 == int.MinValue || z2 == int.MinValue || x == int.MinValue || y == int.MinValue || z == int.MinValue)
      {
        SdtdConsole.Instance.Output("At least one of the given coordinates is not a valid integer");

        return;
      }

      var prefab = new Prefab(new Vector3i(x2 - x1 + 1, y2 - y1 + 1, z2 - z1 + 1));
      prefab.copyFromWorld(GameManager.Instance.World, new Vector3i(x1, y1, z1), new Vector3i(x2, y2, z2));

      SdtdConsole.Instance.Output($"Area duplicated from {x1} {y1} {z1} to {x2} {y2} {z2}");

      prefab.RotateY(false, rot);

      var chunks = new Dictionary<long, Chunk>();
      for (var j = 0; j < prefab.size.x; j++)
      {
        for (var k = 0; k < prefab.size.z; k++)
        {
          for (var m = 0; m < prefab.size.y; m++)
          {
            if (GameManager.Instance.World.IsChunkAreaLoaded(x + j, y + m, z + k) &&
                GameManager.Instance.World.GetChunkFromWorldPos(x + j, y + m, z + k) is Chunk chunk)
            {
              if (!chunks.ContainsKey(chunk.Key))
              {
                chunks.Add(chunk.Key, chunk);
              }
            }
            else
            {
              SdtdConsole.Instance.Output("The render prefab is far away. Chunk not loaded on that area");

              return;
            }
          }
        }
      }

      var undo = new Prefab(new Vector3i(prefab.size.x, prefab.size.y, prefab.size.z));
      undo.copyFromWorld(GameManager.Instance.World, new Vector3i(x, y, z), new Vector3i(x + prefab.size.x, y + prefab.size.y, z + prefab.size.z));

      BMPUndo.SetUndo(_senderInfo.RemoteClientInfo != null 
          ? $"{_senderInfo.RemoteClientInfo.entityId}" 
          : "server_",
        undo, new Vector3i(x, y, z));

      var pyState = GameManager.bPhysicsActive;

      GameManager.bPhysicsActive = false;
      
      prefab.CopyIntoLocal(GameManager.Instance.World.ChunkCache, new Vector3i(x, y, z), true, true);
      
      GameManager.bPhysicsActive = pyState;

      Thread.Sleep(50);

      BMReload.ReloadForClients(chunks);

      var stabCalc = new StabilityCalculator();
      stabCalc.Init(GameManager.Instance.World);

      for (var j = 0; j < prefab.size.x; j++)
      {
        for (var k = 0; k < prefab.size.z; k++)
        {
          for (var m = 0; m < prefab.size.y; m++)
          {
            var block = GameManager.Instance.World.GetBlock(x + j, y + m, z + k);
            if (block.type == BlockValue.Air.type){continue;}

            var position = new Vector3i(x + j, y + m, z + k);
            stabCalc.BlockPlacedAt(position, false);
          }
        }
      }
      stabCalc.Cleanup();

      SdtdConsole.Instance.Output($"Duplicated Area at {x} {y} {z}");
    }
  }
}
