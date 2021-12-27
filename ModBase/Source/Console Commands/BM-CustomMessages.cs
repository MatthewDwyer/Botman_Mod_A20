using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMCustomMessages : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Custom Messages";

    public override string[] GetCommands() => new[] { "bm-custommessages" };

    public override string GetHelp() =>
      "Usage:\n" +
      "1. bm-custommessages enable/disable\n" +
      "2. bm-custommessages login [message]\n" +
      "3. bm-custommessages logout [message]\n" +
      "4. bm-custommessages died [message]\n" +
      "5. bm-custommessages killed [message]\n" +
      "6. bm-custommessages list\n" +
      "   1. Enables/Disables Custom Game Messages\n" +
      "   2. Changes the player logged in message\n" +
      "   3. Changes the player logged out message\n" +
      "   4. changes the player died message\n" +
      "   5. Changes the player killed player message\n" +
      "   6. Lists the current messages\n" +
      "   6. For login, logout, died use \"[name]\" where you would like player name to display.\n" +
      "   6. For killed message, use \"[killer]\" and \"[victim]\" where you would like names.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      switch (_params[0].ToLower())
      {
        case "enable":
          GameMessage.Enabled = true;
          Config.UpdateXml();
          SdtdConsole.Instance.Output("Custom messages have been enabled.");
          break;

        case "disable":
          GameMessage.Enabled = false;
          Config.UpdateXml();
          SdtdConsole.Instance.Output("Custom messages have been disabled.");
          break;

        case "login":
          SdtdConsole.Instance.Output("Not implemented yet.");
          break;

        case "logout":
          SdtdConsole.Instance.Output("Not implemented yet.");
          break;

        case "died":
          SdtdConsole.Instance.Output("Not implemented yet.");
          break;

        case "killed":
          SdtdConsole.Instance.Output("Not implemented yet.");
          break;

        case "list":
          SdtdConsole.Instance.Output("Not implemented yet.");
          break;

        default:
          SdtdConsole.Instance.Output($"Unknown sub command {_params[0]}.");
          break;
      }
    }
  }
}
