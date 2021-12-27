using System;
using System.Collections.Generic;

namespace Botman.Commands
{
  class BMExemptList : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman- ExemptLists";

    public override string[] GetCommands() => new[] { "bm-exemptlist" };

    public override string GetHelp() =>
      "Usage:\n" +
      " 1. bm-exemptlist add \"prefab_name\"\n" +
      " 2. bm-exemptlist remove \"prefab_name\"\n" +
      " 3. bm-exemptlist check\n" +
      "   1. Adding a prefab will render it exempt from any resets.\n" +
      "   2. Removing a prefab will allow it to be reset during any scheduled/live resets.\n" +
      "   3. Checks current list of exempt prefabs.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      switch (_params.Count)
      {
        case 0:
          SdtdConsole.Instance.Output(GetHelp());
          return;

        case 1 when _params[0].Equals("check", StringComparison.InvariantCultureIgnoreCase):
          {
            if (PrefabReset.ExemptList.Count == 0)
            {
              SdtdConsole.Instance.Output("~Botman~ There are no prefabs on the exempt list. All prefabs are fair game.");

              return;
            }

            foreach (var name in PrefabReset.ExemptList)
            {
              SdtdConsole.Instance.Output($" - {name}");
            }

            return;
          }

        case 2 when _params[0].Equals("add", StringComparison.InvariantCultureIgnoreCase):
          {
            if (PrefabReset.ExemptList.ContainsWithComparer(_params[1], StringComparer.InvariantCultureIgnoreCase))
            {
              SdtdConsole.Instance.Output($"~Botman~ {_params[1]} is already on the exempt list");

              return;
            }

            PrefabReset.ExemptList.Add(_params[1]);
            SdtdConsole.Instance.Output($"~Botman~ Added {_params[1]} to the exempt list");
            Config.UpdateXml();

            return;
          }

        case 2 when _params[0].Equals("remove", StringComparison.InvariantCultureIgnoreCase):
          {
            if (!PrefabReset.ExemptList.ContainsWithComparer(_params[1], StringComparer.InvariantCultureIgnoreCase))
            {
              SdtdConsole.Instance.Output($"~Botman~ {_params[1]} is not on the exempt list");

              return;
            }

            PrefabReset.ExemptList.Remove(_params[1]);
            SdtdConsole.Instance.Output($"~Botman~ Removed {_params[1]} from the exempt list");
            Config.UpdateXml();

            return;
          }

        default:
          SdtdConsole.Instance.Output(GetHelp());
          return;
      }
    }
  }
}