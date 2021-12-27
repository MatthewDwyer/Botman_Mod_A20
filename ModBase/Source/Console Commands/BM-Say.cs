using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMSay : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Custom bot public chat responses";

    public override string[] GetCommands() => new[] { "bm-say" };

    public override string GetHelp()
    {
      return "Usage:\n" +
             "1. bm-say Hello World";
    }

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());
        
        return;
      }

      SendMessage.Public(string.Join(" ", _params));
    }
  }
}