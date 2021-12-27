using System.Collections.Generic;
using UnityEngine;

namespace Botman.Commands
{
  public class BMSpawnHorde : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Spawn horde near a player or coordinate";

    public override string[] GetCommands() => new[] { "bm-spawnhorde" };

    public override string GetHelp() =>
      "Spawn horde near a player/location.\n" +
      "Usage:\n" +
      "1. bm-spawnhorde <steam id/player name/entity id> <qty>\n" +
      "2. bm-spawnhorde <x> <y> <z> <qty>\n" +
      "   1. Spawns a horde near specified player\n" +
      "   1. Spawns a horde near specified location\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      switch (_params.Count)
      {
        case 2:
          {
            if (!CmdHelpers.GetEntityPlayer(_params[0], out var entityPlayer)) { return; }

            if (!GetSize(_params[1], out var size)) { return; }

            SpawnHorde(entityPlayer.GetPosition(), size);

            break;
          }

        case 4:
          {
            if (!CmdHelpers.GetVector3(_params[0], _params[1], _params[2], out var position)) { return; }

            if (!GetSize(_params[3], out var size)) { return; }

            SpawnHorde(position, size);

            break;
          }

        default:
          SdtdConsole.Instance.Output($"Wrong number of arguments, expected 2 or 4, found {_params.Count}.");
          SdtdConsole.Instance.Output(GetHelp());

          return;
      }
    }

    private static bool GetSize(string sizeString, out int size)
    {
      if (int.TryParse(sizeString, out size) && size >= 1) { return true; }

      SdtdConsole.Instance.Output($"The value of qty is not a valid number or is less than 1: {sizeString}");

      return false;
    }

    private static void SpawnHorde(Vector3 startPosition, int size)
    {
      if (GameManager.Instance.World == null)
      {
        SdtdConsole.Instance.Output("World not awake.");

        return;
      }

      var chunkComponent = GameManager.Instance.World.aiDirector.GetComponent<AIDirectorChunkEventComponent>();
      if (chunkComponent == null)
      {
        SdtdConsole.Instance.Output("No AIDirectorChunkEventComponent Component found");

        return;
      }

      var horde = chunkComponent.CreateHorde(startPosition);

      horde.SpawnMore(size);

      SdtdConsole.Instance.Output($"Horde spawning at {startPosition}");
    }
  }
}
