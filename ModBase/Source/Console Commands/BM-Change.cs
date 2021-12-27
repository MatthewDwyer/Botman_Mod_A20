using Botman.Source.BotModEvents;
using System.Collections.Generic;
using System.Linq;

namespace Botman.Commands
{
  public class BMChange : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Custom bot chat responses";

    public override string[] GetCommands() => new[] { "bm-change" };

    public override string GetHelp() =>
      "Changes Bot name and/or color responses:\n" +
      "Usage:\n" +
      "1. bm-change botname <name> \n" +
      "2. bm-change public-color <color> \n" +
      "3. bm-change private-color <color> \n" +
      "   1. Changes Botname. To add colors, use rgb color codes. ex. \"bm-change botname [00ff00]botman\" for green botman name. \n" +
      "   2. Changes public response color.\n" +
      "   3. Changes private response color\n" +
      "*For options 2 and 3, only a 6 digit rgb color code needs to be used.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count < 2)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      switch (_params[0].ToLower())
      {
        case "botname":
          {
            // Join params if more than 2 provided
            Config.BotName = string.Join(" ", _params.Skip(1));
            SdtdConsole.Instance.Output($"Bot name has been changed to {Config.BotName}");
            break;
          }

        case "public-color":
          {
            // Ignore extra params if more than 2 provided
            if (!CmdHelpers.GetColor(_params[1], out var color)) { return; }

            ChatMessage.PublicTextColor = color;
            SdtdConsole.Instance.Output($"Bot public chat color has been changed to {ChatMessage.PublicTextColor}");
            break;
          }

        case "private-color":
          {
            // Ignore extra params if more than 2 provided
            if (!CmdHelpers.GetColor(_params[1], out var color)) { return; }

            ChatMessage.PrivateTextColor = color;
            SdtdConsole.Instance.Output($"Bot private chat color has been changed to {ChatMessage.PrivateTextColor}");
            break;
          }

        default:
          SdtdConsole.Instance.Output("Command not recognized. Try 'help bm-change' for more info.");
          return;
      }

      Config.UpdateXml();
    }
  }
}
