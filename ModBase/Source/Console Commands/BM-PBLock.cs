using System;
using System.Collections.Generic;
using System.Threading;

namespace Botman.Commands
{
  public class BMPBlock : BMCmdAbstract
  {
    private static readonly Dictionary<int, Vector3i> StoredLocations = new Dictionary<int, Vector3i>();

    public override string GetDescription() => "~Botman~ Fill a defined location with a specific block";

    public override string[] GetCommands() => new[] { "bm-pblock" };

    public override string GetHelp() =>
      "Usage:\n" +
      "1. bm-pblock <block_name> <x1> <x2> <y1> <y2> <z1> <z2> <rot>\n" +
      "2. bm-pblock <block_name> <x>@<qnt> <y>@<qnt> <z>@<qnt> <rot>\n" +
      "3. bm-pblock <block_name> <qnt> <qnt> <qnt> <rot>\n" +
      "4. bm-pblock <block_name>\n" +
      "5. bm-pblock p1      or    pblock L1\n" +
      "6. bm-pblock p2 <block_name>\n" +
      "   1. fill blocks with block_name from x1,y1,z1 to x2,y2,z2\n" +
      "   2. fill blocks with block_name from x,y,z each quantity. Quantity can be 0, positive or negative.\n" +
      "   2. fill blocks with block_name from your position each quantity. Quantity can be 0, positive or negative.\n" +
      "   4. Search for block names. Fill with * to list all.\n" +
      "   5. Store your position to be used on method 6. p1 store player position,  L1 store the position where player is looking for\n" +
      "   6. Place blocks with block_name from position stored on method 5 until your current location.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params.Count != 1 && _params.Count != 2 && _params.Count != 5 && _params.Count != 8)
      {
        SdtdConsole.Instance.Output($"Wrong number of arguments, expected 1 or 2 or 5 or 8, found {_params.Count}.");
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      var x1 = int.MinValue;
      var y1 = int.MinValue;
      var z1 = int.MinValue;

      var x2 = int.MinValue;
      var y2 = int.MinValue;
      var z2 = int.MinValue;

      var rot = int.MinValue;

      var nextStoreX = 0;
      var nextStoreY = 0;
      var nextStoreZ = 0;

      var blockName = "";
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

          if (StoredLocations.ContainsKey(cInfo.entityId))
          {
            StoredLocations.Remove(cInfo.entityId);
          }
          StoredLocations.Add(cInfo.entityId, entityPlayer.GetBlockPosition());

          SdtdConsole.Instance.Output($"Stored position: {entityPlayer.GetBlockPosition()}");

          return;
        }

        if (_params[0].Equals("l1", StringComparison.InvariantCultureIgnoreCase))
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

          for (var i = 0; i < 1000; i++)
          {
            var look = entityPlayer.GetLookRay().GetPoint(i);
            var blockPos = World.worldToBlockPos(look);
            var lookBlock = GameManager.Instance.World.GetBlock(blockPos);
            if (lookBlock.type == BlockValue.Air.type) { continue; }

            if (StoredLocations.ContainsKey(cInfo.entityId))
            {
              StoredLocations.Remove(cInfo.entityId);
            }
            StoredLocations.Add(cInfo.entityId, blockPos);

            SdtdConsole.Instance.Output($"Stored position: {blockPos}");

            return;
          }

          SdtdConsole.Instance.Output("Unable to find the looking block. Is it to far?");

          return;
        }

        ShowListOfBlocks(_params[0]);

        return;
      }

      if (_params.Count == 2)
      {
        if (!_params[0].Equals("p2", StringComparison.InvariantCultureIgnoreCase))
        {
          SdtdConsole.Instance.Output("Invalid request. first param must be p2");
          SdtdConsole.Instance.Output(GetHelp());

          return;
        }

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

        if (!StoredLocations.ContainsKey(cInfo.entityId))
        {
          SdtdConsole.Instance.Output("There isn't any stored location. Use method 5. to store a position.");
          SdtdConsole.Instance.Output(GetHelp());

          return;
        }

        StoredLocations.TryGetValue(cInfo.entityId, out var storedPos);

        x1 = storedPos.x;
        y1 = storedPos.y;
        z1 = storedPos.z;

        x2 = entityPlayer.GetBlockPosition().x;
        y2 = entityPlayer.GetBlockPosition().y;
        z2 = entityPlayer.GetBlockPosition().z;

        rot = 0;

        nextStoreX = x2;
        nextStoreY = y1;
        nextStoreZ = z2;
        blockName = _params[1].ToLower();
      }

      if (_params.Count == 8)
      {
        int.TryParse(_params[1], out x1);
        int.TryParse(_params[3], out y1);
        int.TryParse(_params[5], out z1);

        int.TryParse(_params[2], out x2);
        int.TryParse(_params[4], out y2);
        int.TryParse(_params[6], out z2);

        int.TryParse(_params[7], out rot);

        blockName = _params[0].ToLower();
      }

      if (_params.Count == 5)
      {
        var xParts = _params[1].Split('@');
        var yParts = _params[2].Split('@');
        var zParts = _params[3].Split('@');

        blockName = _params[0].ToLower();

        if (xParts.Length == 1 && yParts.Length == 1 && zParts.Length == 1)
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

          x1 = entityPlayer.GetBlockPosition().x;
          y1 = entityPlayer.GetBlockPosition().y;
          z1 = entityPlayer.GetBlockPosition().z;

          if (!int.TryParse(_params[1], out x2) ||
              !int.TryParse(_params[2], out y2) ||
              !int.TryParse(_params[3], out z2))
          {
            SdtdConsole.Instance.Output("One of x2, y2, or z2 was not a number.");

            return;
          }

          if (x2 > 0)
          {
            x2 -= 1;
          }
          else
          {
            x2 += 1;
          }

          if (y2 > 0)
          {
            y2 -= 1;
          }
          else
          {
            y2 += 1;
          }

          if (z2 > 0)
          {
            z2 -= 1;
          }
          else
          {
            z2 += 1;
          }

          x2 = x1 + x2;
          y2 = y1 + y2;
          z2 = z1 + z2;
        }
        else if (xParts.Length == 2 && yParts.Length == 2 && zParts.Length == 2)
        {
          if (!int.TryParse(xParts[0], out x1) ||
              !int.TryParse(yParts[0], out y1) ||
              !int.TryParse(zParts[0], out z1))
          {
            SdtdConsole.Instance.Output("One of x1, y1, or z1 was not a number.");

            return;
          }

          if (!int.TryParse(xParts[1], out x2) ||
              !int.TryParse(yParts[1], out y2) ||
              !int.TryParse(zParts[1], out z2))
          {
            SdtdConsole.Instance.Output("One of x2, y2, or z2 was not a number.");

            return;
          }

          x2 = x1 + x2;
          y2 = y1 + y2;
          z2 = z1 + z2;
        }
        else
        {
          SdtdConsole.Instance.Output("Invalid Parameters.");
          SdtdConsole.Instance.Output(GetHelp());

          return;
        }

        int.TryParse(_params[4], out rot);
      }

      if (x1 == int.MinValue || y1 == int.MinValue || z1 == int.MinValue || x2 == int.MinValue || y2 == int.MinValue || z2 == int.MinValue)
      {
        SdtdConsole.Instance.Output("At least one of the given coordinates is not a valid integer");
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

      if (rot < 0 || rot > 3)
      {
        SdtdConsole.Instance.Output("Invalid rotation parameter. It need to be 0,1,2 or 3");

        return;
      }

      var blockFound = false;
      var hasBlockId = int.TryParse(blockName, out var blockId);
      var blockV = BlockValue.Air;
      foreach (var block in Block.list)
      {
        if (block == null ||
            (block.GetBlockName() == null || !block.GetBlockName().Equals(blockName, StringComparison.InvariantCultureIgnoreCase)) &&
            (!hasBlockId || block.blockID != blockId))
        {
          continue;
        }

        blockFound = true;
        blockV = Block.GetBlockValue(block.GetBlockName());

        break;
      }

      if (!blockFound)
      {
        SdtdConsole.Instance.Output($"Invalid block name or ID {blockName}");
        ShowListOfBlocks("*");

        return;
      }

      var prefab = new Prefab(new Vector3i(x2 - x1 + 1, y2 - y1 + 1, z2 - z1 + 1));

      var chunks = new Dictionary<long, Chunk>();
      for (var k = y1; k <= y2; k++)
      {
        for (var i = x1; i <= x2; i++)
        {
          for (var j = z1; j <= z2; j++)
          {
            prefab.SetBlock(i - x1, k - y1, j - z1, blockV);
            if (GameManager.Instance.World.IsChunkAreaLoaded(i, k, j))
            {
              var c = GameManager.Instance.World.GetChunkFromWorldPos(i, k, j) as Chunk;
              if (!chunks.ContainsKey(c.Key))
              {
                chunks.Add(c.Key, c);
              }
            }
            else
            {
              SdtdConsole.Instance.Output("The filling block are to far away. Chunk not loaded on that area");

              return;
            }
          }
        }
      }

      prefab.RotateY(false, rot);

      var undo = new Prefab(prefab.size);
      undo.copyFromWorld(GameManager.Instance.World, new Vector3i(x1, y1, z1), new Vector3i(x2, y2, z2));

      BMPUndo.SetUndo(_senderInfo.RemoteClientInfo != null
          ? $"{_senderInfo.RemoteClientInfo.entityId}"
          : "server_",
        undo, new Vector3i(x1, y1, z1));

      var pyState = GameManager.bPhysicsActive;

      GameManager.bPhysicsActive = false;

      prefab.CopyIntoLocal(GameManager.Instance.World.ChunkCache, new Vector3i(x1, y1, z1), true, true);

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
            var block = GameManager.Instance.World.GetBlock(x1 + j, y1 + m, z1 + k);
            if (block.type == BlockValue.Air.type) { continue; }

            stabCalc.BlockPlacedAt(new Vector3i(x1 + j, y1 + m, z1 + k), false);
          }
        }
      }

      if (_params.Count == 2)
      {
        if (_senderInfo.RemoteClientInfo != null)
        {
          StoredLocations.Remove(_senderInfo.RemoteClientInfo.entityId);
          StoredLocations.Add(_senderInfo.RemoteClientInfo.entityId, new Vector3i(nextStoreX, nextStoreY, nextStoreZ));

          SdtdConsole.Instance.Output($"Stored position: {nextStoreX} {nextStoreY} {nextStoreZ}");
        }
      }

      SdtdConsole.Instance.Output($"Block loaded from {x1} {y1} {z1} to {x2} {y2} {z2}");
    }

    public void ShowListOfBlocks(string blockName)
    {
      SdtdConsole.Instance.Output($"blockName:{blockName}");

      blockName = blockName.ToLower();
      foreach (var block in Block.list)
      {
        if (blockName == "*" || block?.GetBlockName() != null && block.GetBlockName().ContainsCaseInsensitive(blockName))
        {
          SdtdConsole.Instance.Output($"BlockID:{block.blockID}   Name:{block.GetBlockName()}");
        }
      }
    }
  }
}
