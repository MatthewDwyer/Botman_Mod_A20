using System;
using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMSettings : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Block Scan";

    public override string GetHelp() =>
      "Usage:\n" +
      "1. bm-settings physics";

    public override string[] GetCommands() => new[] { "bm-settings" };

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output($"Physics is {(GameManager.bPhysicsActive ? "on" : "off")}.");

        return;
      }

      if (_params[0].Equals("physics", StringComparison.InvariantCultureIgnoreCase))
      {
        GameManager.bPhysicsActive = !GameManager.bPhysicsActive;

        GameManager.Instance.SaveWorld();
        SdtdConsole.Instance.Output($"Physics have been toggled {(GameManager.bPhysicsActive ? "ON" : "OFF")}.");
      }

      SdtdConsole.Instance.Output(GetHelp());
    }
  }
}
