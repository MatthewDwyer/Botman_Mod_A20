using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMListPlayerBed : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ List bed locations of all players or a specific player";

    public override string[] GetCommands() => new[] { "bm-listplayerbed" };

    public override string GetHelp() =>
      "Usage:\n" +
      "1. bm-listplayerbed <steam id / player name / entity id>\n" +
      "2. bm-listplayerbed  *List bedroll locations of all players online\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (GameManager.Instance.World == null)
      {
        SdtdConsole.Instance.Output("World isn't loaded.");

        return;
      }

      switch (_params.Count)
      {
        case 0:
          {
            foreach (var player in GameManager.Instance.World.Players.dict.Values)
            {
              var beds = player.SpawnPoints;
              if (beds.Count == 0)
              {
                SdtdConsole.Instance.Output($"The player {player.EntityName} does not have any bed");

                return;
              }

              for (var x = 0; x < beds.Count; x++)
              {
                SdtdConsole.Instance.Output($"{player.EntityName}: {beds[x].x}, {beds[x].y}, {beds[x].z}");
              }
            }

            break;
          }

        case 1:
          {
            var cInfo = ConsoleHelper.ParseParamIdOrName(_params[0]);
            if (cInfo == null)
            {
              SdtdConsole.Instance.Output("Playername or entity/steamid id not found.");

              return;
            }

            var player = GameManager.Instance.World.Players.dict[cInfo.entityId];
            var beds = player.SpawnPoints;

            if (beds.Count == 0)
            {
              SdtdConsole.Instance.Output("The player does not have a bed");

              return;
            }

            for (var x = 0; x < beds.Count; x++)
            {
              SdtdConsole.Instance.Output($"PlayerBed: {player.EntityName} at {beds[x].x}, {beds[x].y}, {beds[x].z}");
            }

            break;
          }

        default:
          SdtdConsole.Instance.Output(GetHelp());
          break;
      }
    }
  }
}
