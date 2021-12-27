using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Botman.Commands
{
  public class BMRepBlock : BMCmdAbstract
  {
    private static Dictionary<int, Vector3i> location = new Dictionary<int, Vector3i>();

    public override string GetDescription() => "~Botman~ Replace blocks from a defined location";

    public override string[] GetCommands() => new[] { "bm-repblock" };

    public override string GetHelp() =>
      "Usage:\n" +
      "  1. bm-repblock <block_to_be_replaced> <block_name> <x1> <x2> <y1> <y2> <z1> <z2> <rot>\n" +
      "  2. bm-repblock <block_to_be_replaced> <block_name> <x>@<qnt> <y>@<qnt> <z>@<qnt> <rot>\n" +
      "  3. bm-repblock <block_to_be_replaced> <block_name> <qnt> <qnt> <qnt> <rot>\n" +
      "  4. bm-repblock <block_name>\n" +
      "  5. bm-repblock p1\n" +
      "  6. bm-repblock p2 <block_to_be_replaced_name or id> <block_name>\n" +
      "1. replace blocks block_to_be_replaced with block_name from x1,y1,z1 to x2,y2,z2\n" +
      "2. replace blocks block_to_be_replaced with block_name from x,y,z each quantity. Quantity can be 0, positive or negative.\n" +
      "2. replace blocks block_to_be_replaced with block_name from your position each quantity. Quantity can be 0, positive or negative.\n" +
      "4. Search for block names. Fill with * to list all.\n" +
      "5. Store your position to be used on method 6.\n" +
      "6. replace blocks block_to_be_replaced with block_name from position stored on method 5 until your current location.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      switch (_params.Count)
      {
        case 0:
          SdtdConsole.Instance.Output(GetHelp());
          return;

        case 1:
          Do1Params(_params[0], _senderInfo);
          return;

        case 3:
          Do3Params(_params, _senderInfo);
          return;

        case 6:
          Do6Params(_params, _senderInfo);
          break;

        case 9:
          Do9Params(_params, _senderInfo);
          break;

        default:
          SdtdConsole.Instance.Output($"Wrong number of arguments, expected 1 or 3 or 6 or 9, found {_params.Count}.");
          SdtdConsole.Instance.Output(GetHelp());
          break;
      }
    }

    private void Do1Params(string option, CommandSenderInfo _senderInfo)
    {
      if (option.Equals("p1", StringComparison.InvariantCultureIgnoreCase))
      {
        StoreP1(_senderInfo);

        return;
      }

      ShowListOfBlocks(option);
    }

    private void Do3Params(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (!_params[0].Equals("p2", StringComparison.InvariantCultureIgnoreCase))
      {
        SdtdConsole.Instance.Output("Invalid request. The first param must be p2 if 3 params are provided.");
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (!CmdHelpers.GetClientAndEntity(_senderInfo.RemoteClientInfo, out var entityPlayer))
      {
        SdtdConsole.Instance.Output($"Unable to find player entity for {_senderInfo.RemoteClientInfo.playerName}.");

        return;
      }

      if (!location.TryGetValue(_senderInfo.RemoteClientInfo.entityId, out var storedPosition))
      {
        SdtdConsole.Instance.Output("There isn't any stored location. Use method 5 to store a position.");
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      var entityPosition = entityPlayer.GetBlockPosition();

      location.Remove(_senderInfo.RemoteClientInfo.entityId);
      location.Add(_senderInfo.RemoteClientInfo.entityId, entityPosition);

      SdtdConsole.Instance.Output($"New stored position: {entityPosition.x} {storedPosition.y} {entityPosition.z}");

      ReplaceBlocks(
        undoEntityTag: GetUndoTag(_senderInfo.RemoteClientInfo),
        min: new Vector3i(Math.Min(storedPosition.x, entityPosition.x), Math.Min(storedPosition.y, entityPosition.y), Math.Min(storedPosition.z, entityPosition.z)),
        max: new Vector3i(Math.Max(storedPosition.x, entityPosition.x), Math.Max(storedPosition.y, entityPosition.y), Math.Max(storedPosition.z, entityPosition.z)),
        rot: 0,
        replacementBlock: _params[2].ToLower(),
        blockToReplace: _params[1].ToLower(),
        _senderInfo);
    }

    private void Do6Params(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      Vector3i position;
      Vector3i position2;

      var xParts = _params[2].Split('@');
      var yParts = _params[3].Split('@');
      var zParts = _params[4].Split('@');
      if (xParts.Length == 1 && yParts.Length == 1 && zParts.Length == 1)
      {
        if (!CmdHelpers.GetClientAndEntity(_senderInfo.RemoteClientInfo, out var entityPlayer))
        {
          SdtdConsole.Instance.Output($"Unable to find player entity for {_senderInfo.RemoteClientInfo.playerName}.");

          return;
        }

        position = entityPlayer.GetBlockPosition();

        if (!CmdHelpers.GetVector3i(_params[2], _params[3], _params[4], out position2)) { return; }

        position2.x += position.x;
        position2.y += position.y;
        position2.z += position.z;
      }
      else if (xParts.Length == 2 && yParts.Length == 2 && zParts.Length == 2)
      {
        if (!CmdHelpers.GetVector3i(xParts[0], yParts[0], zParts[0], out position)) { return; }

        if (!CmdHelpers.GetVector3i(xParts[1], yParts[1], zParts[1], out position2)) { return; }

        position2.x += position.x;
        position2.y += position.y;
        position2.z += position.z;
      }
      else
      {
        SdtdConsole.Instance.Output("Invalid Parameters.");
        SdtdConsole.Instance.Output(GetHelp());
        return;
      }

      if (!int.TryParse(_params[5], out var rot) || rot < 0 || rot > 3)
      {
        SdtdConsole.Instance.Output("Invalid rotation parameter. It need to be 0, 1, 2 or 3");

        return;
      }

      ReplaceBlocks(
        undoEntityTag: GetUndoTag(_senderInfo.RemoteClientInfo),
        min: new Vector3i(Math.Min(position.x, position2.x), Math.Min(position.y, position2.y), Math.Min(position.z, position2.z)),
        max: new Vector3i(Math.Max(position.x, position2.x), Math.Max(position.y, position2.y), Math.Max(position.z, position2.z)),
        rot: rot,
        replacementBlock: _params[1].ToLower(),
        blockToReplace: _params[0].ToLower(),
        _senderInfo);
    }

    private void Do9Params(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (!CmdHelpers.GetXyz(_params[2], _params[4], _params[6], out var x1, out var y1, out var z1)) { return; }

      if (!CmdHelpers.GetXyz(_params[3], _params[5], _params[7], out var x2, out var y2, out var z2)) { return; }

      if (!int.TryParse(_params[8], out var rot) || rot < 0 || rot > 3)
      {
        SdtdConsole.Instance.Output("Invalid rotation parameter. It need to be 0, 1, 2 or 3");

        return;
      }

      ReplaceBlocks(
        undoEntityTag: GetUndoTag(_senderInfo.RemoteClientInfo),
        min: new Vector3i(Math.Min(x1, x2), Math.Min(y1, y2), Math.Min(z1, z2)),
        max: new Vector3i(Math.Max(x1, x2), Math.Max(y1, y2), Math.Max(z1, z2)),
        rot: rot,
        replacementBlock: _params[1].ToLower(),
        blockToReplace: _params[0].ToLower(),
        _senderInfo);
    }

    private static bool GetBlockFromNameOrId(string searchText, out int blockId, out BlockValue blockValue)
    {
      blockValue = BlockValue.Air;

      if (int.TryParse(searchText, out blockId) && Block.list[blockId] != default) { return true; }

      var block = Block.list.Where(b => b != null).FirstOrDefault(b => b.GetBlockName().Equals(searchText, StringComparison.InvariantCultureIgnoreCase));

      if (block == default) { return false; }

      blockValue = Block.GetBlockValue(block.GetBlockName());

      blockId = block.blockID;
      return true;
    }

    private static void ReplaceBlocks(string undoEntityTag, Vector3i min, Vector3i max, int rot, string replacementBlock, string blockToReplace, CommandSenderInfo _senderInfo)
    {
      if (!GetBlockFromNameOrId(blockToReplace, out var blockIdToBeReplaced, out _))
      {
        SdtdConsole.Instance.Output($"Invalid replacement block name or ID {blockToReplace}");
        //ShowListOfBlocks("*");

        return;
      }

      if (!GetBlockFromNameOrId(replacementBlock, out _, out var replacementBlockValue))
      {
        SdtdConsole.Instance.Output($"Invalid search block name or ID {replacementBlock}");
        //ShowListOfBlocks("*");

        return;
      }

      var prefab = new Prefab(new Vector3i(max.x - min.x + 1, max.y - min.y + 1, max.z - min.z + 1)) { bCopyAirBlocks = true };

      var chunks = new Dictionary<long, Chunk>();

      for (var k = min.y; k <= max.y; k++)
      {
        for (var i = min.x; i <= max.x; i++)
        {
          for (var j = min.z; j <= max.z; j++)
          {
            var currentBlock = GameManager.Instance.World.GetBlock(new Vector3i(i, k, j));

            if (currentBlock.type == blockIdToBeReplaced)
            {
              prefab.SetBlock(i - min.x, k - min.y, j - min.z, replacementBlockValue);
            }

            if (GameManager.Instance.World.IsChunkAreaLoaded(i, k, j))
            {
              if (GameManager.Instance.World.GetChunkFromWorldPos(i, k, j) is Chunk chunk && !chunks.ContainsKey(chunk.Key))
              {
                chunks.Add(chunk.Key, chunk);
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

      // note: rot values other than 0 will give weird results as this rotates the entire prefab not each block within it.
      prefab.RotateY(false, rot);

      var undoPrefab = new Prefab(prefab.size) { bCopyAirBlocks = true };

      undoPrefab.copyFromWorld(GameManager.Instance.World, min, max);

      BMPUndo.SetUndo(undoEntityTag, undoPrefab, min);

      var pyState = GameManager.bPhysicsActive;
      GameManager.bPhysicsActive = false;
      prefab.CopyIntoLocal(GameManager.Instance.World.ChunkCache, min, true, true);
      GameManager.bPhysicsActive = pyState;

      Thread.Sleep(50);
      BMReload.ReloadForClients(chunks, _senderInfo.RemoteClientInfo.CrossplatformId);
      var stabCalc = new StabilityCalculator();
      stabCalc.Init(GameManager.Instance.World);

      for (var j = 0; j < prefab.size.x; j++)
      {
        for (var k = 0; k < prefab.size.z; k++)
        {
          for (var m = 0; m < prefab.size.y; m++)
          {
            var block = GameManager.Instance.World.GetBlock(min.x + j, min.y + m, min.z + k);
            if (block.type != BlockValue.Air.type)
            {
              stabCalc.BlockPlacedAt(new Vector3i(min.x + j, min.y + m, min.z + k), false);
            }
          }
        }
      }

      stabCalc.Cleanup();

      SdtdConsole.Instance.Output($"Block replaced from {min.x} {min.y} {min.z} to {max.x} {max.y} {max.z}");
    }

    private static string GetUndoTag(ClientInfo remoteClientInfo) =>
      remoteClientInfo != null ? remoteClientInfo.entityId.ToString() : "server_";

    private static void StoreP1(CommandSenderInfo _senderInfo)
    {
      if (!CmdHelpers.GetClientAndEntity(_senderInfo.RemoteClientInfo, out var entityPlayer))
      {
        SdtdConsole.Instance.Output($"Unable to find player entity for {_senderInfo.RemoteClientInfo.playerName}.");

        return;
      }

      if (location.ContainsKey(_senderInfo.RemoteClientInfo.entityId))
      {
        location.Remove(_senderInfo.RemoteClientInfo.entityId);
      }

      var position = entityPlayer.GetBlockPosition();

      location.Add(_senderInfo.RemoteClientInfo.entityId, position);

      SdtdConsole.Instance.Output($"Stored position: {position.x} {position.y} {position.z}");
    }

    private static void ShowListOfBlocks(string filter)
    {
      SdtdConsole.Instance.Output($"blockName:{filter}");

      filter = filter.ToLower();

      if (filter == "*")
      {
        foreach (var block in Block.list.Where(b => b != null))
        {
          SdtdConsole.Instance.Output($"BlockID:{block.blockID}   Name:{block.GetBlockName()}");
        }

        return;
      }

      foreach (var block in Block.list
        .Where(block => block != null &&
                    block.GetBlockName().IndexOf(filter, StringComparison.InvariantCultureIgnoreCase) > -1)
        .Select(block => new { Id = block.blockID, Name = block.GetBlockName() })
        )
      {
        SdtdConsole.Instance.Output($"BlockID:{block.Id}   Name:{block.Name}");
      }
    }
  }
}
