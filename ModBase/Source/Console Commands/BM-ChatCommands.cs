using System.Collections.Generic;
using Botman.Source.BotModEvents;

namespace Botman.Commands
{
  public class BMChatCommands : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Custom bot public chat responses";

    public override string[] GetCommands() => new[] { "bm-chatcommands" };

    public override string GetHelp() =>
      "Usage:\n" +
      "1: bm-chatcommands\n" +
      "2: bm-chatcommands hide <true/false>\n" +
      "3: bm-chatcommands prefix <prefix> \n" +
      "   1. Returns current chat command prefix and current hidden status. \n" +
      "   2. Enables/Disables suppression of all commands beginning with assigned prefix in public chat. \n" +
      "   3. Assigns command prefixes.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());
        SdtdConsole.Instance.Output($"Current command prefix is {ChatMessage.CommandPrefix}\nHidden: {ChatMessage.Hide}");

        return;
      }

      if (_params.Count != 2)
      {
        SdtdConsole.Instance.Output($"Wrong number of arguments, expected 2, found {_params.Count}");

        return;
      }

      switch (_params[0].ToLower())
      {
        case "prefix":
          {
            ChatMessage.CommandPrefix = _params[1];
            SdtdConsole.Instance.Output($"Custom command prefix set to: {ChatMessage.CommandPrefix}");
            break;
          }

        case "hide":
          {
            switch (_params[1])
            {
              case "true":
                ChatMessage.Hide = true;
                break;

              case "false":
                ChatMessage.Hide = false;
                break;

              default:
                SdtdConsole.Instance.Output("Arguments for hide can only be 'true' or 'false'");
                return;
            }

            SdtdConsole.Instance.Output($"Commands beginning with {ChatMessage.CommandPrefix} will now be {(ChatMessage.Hide ? "hidden to public" : "shown publicly")}");

            break;
          }

        default:
          SdtdConsole.Instance.Output("1st parameter must be either prefix, followed by desired prefix. Or hide, followed by true or false.");
          return;
      }

      Config.UpdateXml();
    }
  }
}
