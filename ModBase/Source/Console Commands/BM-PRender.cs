using System.Collections.Generic;
using System.Threading;

namespace Botman.Commands
{
  public class BMPRender : BMCmdAbstract
  {

    public override string GetDescription() => "~Botman~ Renders a Prefab on given location";

    public override string[] GetCommands() => new[] { "bm-prender" };

    public override string GetHelp() =>
      "Usage:\n" +
      "1. bm-prender <prefab_file_name> <x> <y> <z> <rot>\n" +
      "2. bm-prender <prefab_file_name> <rot>\n" +
      "3. bm-prender <prefab_file_name> <rot> <depth>\n" +
      "   1. Render prefab on <x> <y> <z> location\n" +
      "   2. Render prefab on your position\n" +
      "   3. Render prefab on your position with y deslocated <depth blocks>\n" +
      "*<rot> prefab rotation -> needs to be 0,1,2 or 3\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count != 5 && _params.Count != 4 && _params.Count != 2 && _params.Count != 3)
      {
        SdtdConsole.Instance.Output($"Wrong number of arguments, expected 1 to 5, found {_params.Count}.");
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      var x = int.MinValue;
      var y = int.MinValue;
      var z = int.MinValue;
      var rot = int.MinValue;

      if (_params.Count == 5)
      {
        if (!int.TryParse(_params[1], out x) ||
            !int.TryParse(_params[2], out y) ||
            !int.TryParse(_params[3], out z) ||
            !int.TryParse(_params[4], out rot))
        {
          SdtdConsole.Instance.Output("At least one of the given coordinates is not a valid integer");

          return;
        }
      }

      if (_params.Count == 4)
      {
        if (!int.TryParse(_params[1], out x) ||
            !int.TryParse(_params[2], out y) ||
            !int.TryParse(_params[3], out z))
        {
          SdtdConsole.Instance.Output("At least one of the given coordinates is not a valid integer");

          return;
        }

        rot = 0;
      }
      else if (_params.Count == 1 || _params.Count == 2 || _params.Count == 3)
      {
        var cInfo = _senderInfo.RemoteClientInfo;
        if (cInfo == null)
        {
          SdtdConsole.Instance.Output("This command can be only sent by player in game.");

          return;
        }

        var entityPlayer = GameManager.Instance.World.Players.dict[cInfo.entityId];
        if (entityPlayer == null)
        {
          SdtdConsole.Instance.Output("Unable to get your position");

          return;
        }

        var pos = entityPlayer.GetBlockPosition();
        x = pos.x;
        y = pos.y;
        z = pos.z;

        if (_params.Count == 1)
        {
          rot = 0;
        }
        else
        {
          if (!int.TryParse(_params[1], out rot))
          {
            SdtdConsole.Instance.Output("Unable to parse rot as a number");

            return;
          }
        }

        if (_params.Count == 3)
        {
          if (!int.TryParse(_params[2], out var depth))
          {
            SdtdConsole.Instance.Output("Unable to parse depth as a number");
          }

          y += depth;
        }
      }

      if (rot < 0 || rot > 3)
      {
        SdtdConsole.Instance.Output("Invalid rotation parameter. It need to be 0, 1, 2 or 3");

        return;
      }

      var prefab = new Prefab();

      if (!prefab.Load(_params[0]))
      {
        SdtdConsole.Instance.Output($"Unable to load prefab {_params[0]}");

        return;
      }

      y += prefab.yOffset;

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

      var undo = new Prefab(prefab.size);
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
            if (block.type == BlockValue.Air.type) { continue; }

            stabCalc.BlockPlacedAt(new Vector3i(x + j, y + m, z + k), false);
          }
        }
      }
      stabCalc.Cleanup();

      SdtdConsole.Instance.Output($"Prefab {_params[0]} loaded at {x} {y} {z}");
    }
  }
}
