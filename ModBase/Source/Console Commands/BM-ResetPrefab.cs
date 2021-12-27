using System;
using System.Collections.Generic;

namespace Botman.Commands
{
  class BMResetPrefab : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman- Resets a prefab";

    public override string[] GetCommands() => new[] { "bm-resetprefab" };

    public override string GetHelp() =>
      "Usage:\n" +
      " 1. bm-resetprefab <playername/id>\n" +
      " 2. bm-resetprefab <playername/id> check\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0 || _params.Count > 2)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (!CmdHelpers.GetEntityPlayer(_params[0], out var entityPlayer)) { return; }

      var pos = entityPlayer.GetBlockPosition();

      switch (_params.Count)
      {
        case 2 when _params[1].Equals("check", StringComparison.InvariantCultureIgnoreCase):
          PrefabReset.Check(pos.x, pos.y, pos.z);
          break;

        case 1:
          PrefabReset.ResetAtCoords(pos.x, pos.y, pos.z);
          break;
        
        default:
          SdtdConsole.Instance.Output(GetHelp());
          break;
      }
    }
  }
}
