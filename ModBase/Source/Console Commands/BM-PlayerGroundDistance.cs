using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMPlayerGroundDistance : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Check player ground distance";

    public override string[] GetCommands() => new[] { "bm-playergrounddistance" };

    public override string GetHelp() =>
      "Usage:\n" +
      "1. bm-playergrounddistance <steam id / player name / entity id> \n" +
      "2. bm-playergrounddistance \n" +
      "   1. Lists the specified players ground distance\n" +
      "   2. List all online players ground distance\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count > 1)
      {
        SdtdConsole.Instance.Output($"Wrong number of arguments, expected 0 or 1, found {_params.Count}.");

        return;
      }

      var inVehicle = false;

      if (_params.Count == 1)
      {
        var cInfo = ConsoleHelper.ParseParamIdOrName(_params[0]);
        if (cInfo == null)
        {
          SdtdConsole.Instance.Output("Playername or entity/steamid id not found.");

          return;
        }

        var entityPlayer = GameManager.Instance.World.Players.dict[cInfo.entityId];
        if (entityPlayer.AttachedToEntity is EntityVehicle)
        {
          inVehicle = true;
        }

        SdtdConsole.Instance.Output($"PGD: entity_id={entityPlayer.entityId} dist={GetPlayerGroundDistance(entityPlayer)} vehicle={inVehicle}");

        return;
      }

      using (var enumerator = GameManager.Instance.World.Players.list.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          var entityPlayer = enumerator.Current;
          if (entityPlayer == null) { continue; }

          if (entityPlayer.AttachedToEntity is EntityVehicle)
          {
            inVehicle = true;
          }

          SdtdConsole.Instance.Output($"PGD: entity_id={entityPlayer.entityId} dist={GetPlayerGroundDistance(entityPlayer)} vehicle={inVehicle}");
        }
      }
    }

    public int GetPlayerGroundDistance(EntityPlayer player)
    {
      var height = 0;

      var pos = player.GetBlockPosition();

      for (var k = pos.y; k > 0; k--)
      {
        var groundFound = false;

        for (var i = pos.x - 2; i <= pos.x + 2; i++)
        {
          for (var j = pos.z - 2; j <= pos.z + 2; j++)
          {
            var value2 = GameManager.Instance.World.GetBlock(new Vector3i(i, k, j));
            if (value2.type == BlockValue.Air.type) { continue; }

            groundFound = true;
            break;
          }

          if (groundFound) { break; }
        }

        if (groundFound) { break; }

        height++;
      }

      return height;
    }
  }
}
