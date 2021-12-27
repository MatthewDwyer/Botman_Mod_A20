using System;
using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMFallingBlocks : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Enables/Disables Falling Block Removal Tool";

    public override string[] GetCommands() => new[] { "bm-fallingblocksremoval" };

    public override string GetHelp() =>
      "Enables/Disables Falling Block Removal Tool.\n" +
      "Usage:\n" +
      "1. bm-fallingblocksremoval [enable/disable]\n" +
      "   1. Disabled allows blocks to fall as normal (default). Enabled deletes all falling blocks.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      switch (_params.Count)
      {
        case 1 when _params[0].Equals("enable", StringComparison.InvariantCultureIgnoreCase):
          {
            Patches.FallingBlocks.enabled = true;
            Config.UpdateXml();
            SdtdConsole.Instance.Output("~Botman~ Tool enabled. Falling blocks will be removed.");

            return;
          }

        case 1 when _params[0].Equals("disable", StringComparison.InvariantCultureIgnoreCase):
          {
            Patches.FallingBlocks.enabled = false;
            Config.UpdateXml();
            SdtdConsole.Instance.Output("~Botman~ Tool disabled. Falling blocks will fall as normal.");

            return;
          }

        default:
          SdtdConsole.Instance.Output(GetHelp());
          break;
      }
    }
  }
}
