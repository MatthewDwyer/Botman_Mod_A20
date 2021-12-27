using System;
using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMPlayerUnderground : BMCmdAbstract
  {
    private Block waterBlock = null;

    public override string GetDescription() => "~Botman~ Check if a player is underground with no clip or bugged";

    public override string[] GetCommands() => new[] { "bm-playerunderground" };

    public override string GetHelp() =>
      "Usage:\n" +
      "1. bm-playerunderground <steam id / player name / entity id> \n" +
      "2. bm-playerunderground \n" +
      "   1. Returns True/False. True = The specified player is underground \n" +
      "   2. Returns True/False for all players online. True =  the player is underground\n" +
      "* Not to be confused with a player being below ground level. If a result shows True, player is being naughty.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count > 1)
      {
        SdtdConsole.Instance.Output($"Wrong number of arguments, expected 0 or 1, found {_params.Count}.");

        return;
      }

      //Gets water block to check when player is in Water
      foreach (var block in Block.list)
      {
        if (block?.GetBlockName() == null ||
            !block.GetBlockName().Equals("water", StringComparison.InvariantCultureIgnoreCase))
        {
          continue;
        }

        waterBlock = block;

        break;
      }

      if (_params.Count == 1)
      {
        var cInfo = ConsoleHelper.ParseParamIdOrName(_params[0]);
        if (cInfo == null)
        {
          SdtdConsole.Instance.Output("Playername or entity/steamid id not found.");

          return;
        }

        var entityPlayer = GameManager.Instance.World.Players.dict[cInfo.entityId];

        SdtdConsole.Instance.Output($"PUG: entity_id={entityPlayer.entityId} isUnderGround={GetPlayerUnderground(entityPlayer)}");

        return;
      }

      using (var enumerator = GameManager.Instance.World.Players.list.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          var entityPlayer = enumerator.Current;
          if (entityPlayer == null) { continue; }

          SdtdConsole.Instance.Output($"PUG: entity_id={entityPlayer.entityId} isUnderGround={GetPlayerUnderground(entityPlayer)}");
        }
      }
    }

    public bool GetPlayerUnderground(EntityPlayer player)
    {
      var pos = player.GetBlockPosition();

      for (var i = pos.x - 2; i <= pos.x + 2; i++)
      {
        for (var j = pos.z - 2; j <= pos.z + 2; j++)
        {
          for (var k = pos.y - 2; k <= pos.y + 2; k++)
          {
            var block = GameManager.Instance.World.GetBlock(new Vector3i(i, k, j));
            if (block.type == BlockValue.Air.type ||
                waterBlock != null && waterBlock.blockID == block.type ||
                player.IsInElevator())
            {
              return false;
            }
          }
        }
      }

      return true;
    }
  }
}
