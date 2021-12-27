using Botman.Source.BotModEvents;
using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMSetChatPrefix : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Set Chat Prefix";

    public override string[] GetCommands() => new[] { "bm-setchatprefix", "bm-scp" };

    public override string GetHelp() =>
      "Set Chat Prefix\n" +
      "Usage:\n" +
      "1. bm-setchatprefix [prefix]\n" +
      "   1. ex: bm-setchatprefix /\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params.Count != 1)
      {
        SdtdConsole.Instance.Output($"Too many arguments. Expected 1. Found {_params.Count}");

        return;
      }

      ChatMessage.CommandPrefix = _params[0];

      SdtdConsole.Instance.Output($"Chat prefix has been set to {ChatMessage.CommandPrefix}");

      Config.UpdateXml();
    }
  }
}
