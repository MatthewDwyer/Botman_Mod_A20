using System;
using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMBlockScan : BMCmdAbstract
  {
    public static Dictionary<string, Vector3i> p1 = new Dictionary<string, Vector3i>();

    public override string GetDescription() => "~Botman~ Block Scan";

    public override string[] GetCommands() => new[] { "bm-blockscan" };

    public override string GetHelp() =>
      "Usage:\n" +
      "1. bm-blockscan p1\n" +
      "2. bm-blockscan p2\n" +
      "3. bm-blockscan [playername]\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0 || _params.Count > 2)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params[0].Equals("p1", StringComparison.InvariantCultureIgnoreCase))
      {
        var player = GameManager.Instance.World.Players.dict[_senderInfo.RemoteClientInfo.entityId];
        if (player == null)
        {
          SdtdConsole.Instance.Output("~Botman~ You must be logged in to use this command");

          return;
        }

        if (p1.ContainsKey(_senderInfo.RemoteClientInfo.CrossplatformId.CombinedString))
        {
          p1.Remove(_senderInfo.RemoteClientInfo.CrossplatformId.CombinedString);
        }

        p1.Add(_senderInfo.RemoteClientInfo.CrossplatformId.CombinedString, player.GetBlockPosition());

        SdtdConsole.Instance.Output("Position saved.");

        return;
      }

      if (_params[0].Equals("p2", StringComparison.InvariantCultureIgnoreCase))
      {
        var player = GameManager.Instance.World.Players.dict[_senderInfo.RemoteClientInfo.entityId];
        {
          if (player == null)
          {
            SdtdConsole.Instance.Output("~Botman~ You must be logged in to use this command");

            return;
          }

          if (!p1.ContainsKey(_senderInfo.RemoteClientInfo.CrossplatformId.CombinedString))
          {
            SdtdConsole.Instance.Output("~Botman~  No starting point created. Use P1 first.");

            return;
          }

          var position1 = p1[_senderInfo.RemoteClientInfo.CrossplatformId.CombinedString];
          var position2 = player.GetBlockPosition();

          ScanArea(_senderInfo, position1, position2, _params.Count > 1 ? _params[1] : null);
        }

        return;
      }

      var cInfo = ConsoleHelper.ParseParamIdOrName(_params[0]);

      if (cInfo == null)
      {
        SdtdConsole.Instance.Output($"Could not find player {_params[0]}");

        return;
      }

      var entityPlayer = GameManager.Instance.World.Players.dict[_senderInfo.RemoteClientInfo.entityId];
      if (entityPlayer == null)
      {
        SdtdConsole.Instance.Output($"Could not find player {_params[0]}");

        return;
      }

      if (!(GameManager.Instance.World.GetChunkFromWorldPos(entityPlayer.GetBlockPosition()) is Chunk chunk)) { return; }

      ScanChunk(_senderInfo, chunk.GetWorldPos(), _params.Count > 1 ? _params[1] : null);
    }

    public static void ScanChunk(CommandSenderInfo sender, Vector3i startingPoint, string filter)
    {
      var blocks = new Dictionary<string, int>();
      var stopwatch = new MicroStopwatch();
      stopwatch.Start();

      for (var a = startingPoint.x; a <= (startingPoint.x + 16); a++)
      {
        for (var b = startingPoint.z; b <= (startingPoint.z + 16); b++)
        {
          for (var c = 3; c <= 194; c++)
          {
            var blockValue = GameManager.Instance.World.GetBlock(new Vector3i(a, c, b));
            if (filter != null)
            {
              if (blockValue.type == BlockValue.Air.type) { continue; }

              if (!blockValue.Block.GetBlockName().ToLower().Contains(filter.ToLower())) { continue; }

              if (blocks.ContainsKey(blockValue.Block.GetBlockName()))
              {
                blocks[blockValue.Block.GetBlockName()]++;
              }
              else
              {
                blocks.Add(blockValue.Block.GetBlockName(), 1);
              }
            }
            else
            {
              if (blockValue.type == BlockValue.Air.type) { continue; }

              if (blocks.ContainsKey(blockValue.Block.GetBlockName()))
              {
                blocks[blockValue.Block.GetBlockName()]++;
              }
              else
              {
                blocks.Add(blockValue.Block.GetBlockName(), 1);
              }
            }
          }
        }
      }

      foreach (var kvp in blocks)
      {
        SdtdConsole.Instance.Output($" {kvp.Key} = {kvp.Value}");
      }

      p1.Remove(sender.RemoteClientInfo.CrossplatformId.CombinedString);

      stopwatch.Stop();
      SdtdConsole.Instance.Output($" Check took {stopwatch.ElapsedMilliseconds} ms");
    }

    public static void ScanArea(CommandSenderInfo sender, Vector3i point1, Vector3i point2, string filter)
    {
      int x1;
      int x2;
      int z1;
      int z2;
      if (point1.x < point2.x)
      {
        x1 = point1.x;
        x2 = point2.x;
      }
      else
      {
        x1 = point2.x;
        x2 = point1.x;
      }
      if (point1.z < point2.z)
      {
        z1 = point1.z;
        z2 = point2.z;
      }
      else
      {
        z1 = point2.z;
        z2 = point1.z;
      }

      var blocks = new Dictionary<string, int>();

      var stopwatch = new MicroStopwatch();
      stopwatch.Start();

      for (var a = x1; a <= x2; a++)
      {
        for (var b = z1; b <= z2; b++)
        {
          for (var c = 3; c <= 194; c++)
          {
            if (GameManager.Instance.World.GetChunkSync(World.toChunkXZ(a), World.toChunkXZ(b)) == null)
            {
              SdtdConsole.Instance.Output("Some areas of scan are not loaded. Please try scanning a smaller area.");

              return;
            }

            var blockValue = GameManager.Instance.World.GetBlock(new Vector3i(a, c, b));
            if (filter != null)
            {
              if (blockValue.type == BlockValue.Air.type) { continue; }

              if (!blockValue.Block.GetBlockName().ToLower().Contains(filter.ToLower())) { continue; }

              if (blocks.ContainsKey(blockValue.Block.GetBlockName()))
              {
                blocks[blockValue.Block.GetBlockName()]++;
              }
              else
              {
                blocks.Add(blockValue.Block.GetBlockName(), 1);
              }
            }
            else
            {
              if (blockValue.type == BlockValue.Air.type) { continue; }

              if (blocks.ContainsKey(blockValue.Block.GetBlockName()))
              {
                blocks[blockValue.Block.GetBlockName()]++;
              }
              else
              {
                blocks.Add(blockValue.Block.GetBlockName(), 1);
              }
            }
          }
        }
      }

      foreach (var kvp in blocks)
      {
        SdtdConsole.Instance.Output($" {kvp.Key} = {kvp.Value}");
      }

      stopwatch.Stop();

      p1.Remove(sender.RemoteClientInfo.CrossplatformId.CombinedString);
      SdtdConsole.Instance.Output($" Check took {stopwatch.ElapsedMilliseconds} ms");
    }
  }
}
